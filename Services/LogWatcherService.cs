// <copyright file="LogWatcherService.cs" company="WIMP">
// Copyright (c) WIMP. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace WIMP_IntelLog.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using WIMP_IntelLog.Models;

    internal class LogWatcherService : ILogWatcherService
    {
        private readonly FileSystemWatcher fileSystemWatcher;
        private readonly ILogger<LogWatcherService> logger;
        private readonly string logDirectory;
        private readonly string intelChannelName;
        private readonly ILogMessageProcessService logMessageProcessService;
        private readonly string logFileFilter;

        private readonly Dictionary<string, FileReference> chatLogFiles;

        public LogWatcherService(ILoggerFactory loggerFactory, IConfigurationRoot config, ILogMessageProcessService logMessageProcessService)
        {
            this.logger = loggerFactory.CreateLogger<LogWatcherService>();
            this.logDirectory = config["EveLogDirectory"];
            this.intelChannelName = config["IntelChannelName"];
            this.logMessageProcessService = logMessageProcessService;

            this.logFileFilter = $"{this.intelChannelName}_*.txt";

            this.logger.LogInformation($"Watching chat logs in: {this.logDirectory}");
            this.logger.LogInformation($"Watching chat channel: {this.intelChannelName}");

            this.chatLogFiles = new Dictionary<string, FileReference>();

            if (!Directory.Exists(this.logDirectory))
            {
                throw new ArgumentException($"{this.logDirectory} doesn't exist");
            }

            this.fileSystemWatcher = SetupFileSystemWatcher(this.logDirectory, this.logFileFilter, this.OnCreated, this.OnChanged);
        }

        public async Task RunAsync()
        {
            var existingFiles = this.FindLogFiles();
            foreach (var file in existingFiles)
            {
                this.chatLogFiles.Add(file.Key, file.Value);
            }

            this.logger.LogInformation($"found {this.chatLogFiles.Count} log files");

            this.fileSystemWatcher.EnableRaisingEvents = true;

            FileReference currentFile = null;

            FileStream fs = null;
            StreamReader sr = null;

            while (true)
            {
                var mostRecentFile = this.chatLogFiles.Values
                    .OrderByDescending(f => f.LastWrite)
                    .FirstOrDefault();

                if (mostRecentFile != currentFile)
                {
                    this.logger.LogInformation($"change log file from {currentFile?.Path ?? "(null)"} to {mostRecentFile?.Path}");

                    sr?.Dispose();
                    fs?.Dispose();

                    currentFile = mostRecentFile;

                    fs = new FileStream(currentFile.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    sr = new StreamReader(fs, Encoding.Unicode);
                }

                while (sr != null)
                {
                    var line = await sr.ReadLineAsync().ConfigureAwait(true);
                    if (line == null)
                    {
                        break;
                    }

                    this.logger.LogDebug($"read line: {line}");

                    await this.logMessageProcessService
                        .ProcessLogMessage(line)
                        .ConfigureAwait(true);
                }

                await Task.Delay(20)
                    .ConfigureAwait(true);
            }
        }

        private static FileSystemWatcher SetupFileSystemWatcher(string logDirectory, string logFileFilter, FileSystemEventHandler onCreated, FileSystemEventHandler onChanged)
        {
            var fileSystemWatcher = new FileSystemWatcher
            {
                Path = logDirectory,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite,
                Filter = logFileFilter,
            };

            fileSystemWatcher.Created += onCreated;
            fileSystemWatcher.Changed += onChanged;

            return fileSystemWatcher;
        }

        private Dictionary<string, FileReference> FindLogFiles()
        {
            return Directory.GetFiles(this.logDirectory, this.logFileFilter)
                .Select(path => new FileReference { Path = path, LastWrite = File.GetLastWriteTime(path) })
                .ToDictionary(f => f.Path, f => f);
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            this.logger.LogDebug($"new log file: {e.FullPath}");

            var fileReference = new FileReference
            {
                Path = e.FullPath,
                LastWrite = File.GetLastWriteTime(e.FullPath),
            };

            this.chatLogFiles[e.FullPath] = fileReference;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            this.logger.LogDebug($"changed log file: {e.FullPath}");

            var fileReference = new FileReference
            {
                Path = e.FullPath,
                LastWrite = File.GetLastWriteTime(e.FullPath),
            };

            this.chatLogFiles[e.FullPath] = fileReference;
        }
    }
}