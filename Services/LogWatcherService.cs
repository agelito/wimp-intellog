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
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using WIMP_IntelLog.Models;

    internal class LogWatcherService : ILogWatcherService
    {
        private readonly ILogger logger;
        private readonly string logDirectory;
        private readonly ILogMessageProcessService logMessageProcessService;
        private readonly IConfiguration config;

        private readonly Dictionary<string, FileReference> chatLogFiles;

        public LogWatcherService(ILogger<LogWatcherService> logger, IConfiguration config, ILogMessageProcessService logMessageProcessService)
        {
            this.config = config;
            this.logger = logger;
            this.logMessageProcessService = logMessageProcessService;

            this.logDirectory = config["EveLogDirectory"];

            this.chatLogFiles = new Dictionary<string, FileReference>();

            if (!Directory.Exists(this.logDirectory))
            {
                throw new ArgumentException($"{this.logDirectory} doesn't exist");
            }
        }

        public async Task RunAsync(string chatChannelName, CancellationToken cancellationToken)
        {
            var logFileFilter = $"{chatChannelName}_*.txt";

            this.logger.LogInformation($"Watching chat logs in: {this.logDirectory}");
            this.logger.LogInformation($"Watching chat channel: {chatChannelName}");

            foreach (var logFile in FindLogFiles(this.logDirectory, logFileFilter))
            {
                this.chatLogFiles[logFile.Key] = logFile.Value;
            }

            this.logger.LogInformation($"found {this.chatLogFiles.Count} log files");

            var fileSystemWatcher = SetupFileSystemWatcher(this.logDirectory, logFileFilter, this.OnFileCreated, this.OnFileChanged);

            fileSystemWatcher.EnableRaisingEvents = true;

            FileReference currentFile = null;

            FileStream fs = null;
            StreamReader sr = null;

            while (!cancellationToken.IsCancellationRequested)
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

                await Task.Delay(20, cancellationToken)
                    .ConfigureAwait(true);
            }

            fileSystemWatcher.EnableRaisingEvents = false;
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

        private static Dictionary<string, FileReference> FindLogFiles(string logDirectory, string logFileFilter)
        {
            return Directory.GetFiles(logDirectory, logFileFilter)
                .Select(path => new FileReference { Path = path, LastWrite = File.GetLastWriteTime(path) })
                .ToDictionary(f => f.Path, f => f);
        }

        private void OnFileCreated(object source, FileSystemEventArgs e)
        {
            this.logger.LogDebug($"new log file: {e.FullPath}");

            var fileReference = new FileReference
            {
                Path = e.FullPath,
                LastWrite = File.GetLastWriteTime(e.FullPath),
            };

            this.chatLogFiles[e.FullPath] = fileReference;
        }

        private void OnFileChanged(object source, FileSystemEventArgs e)
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