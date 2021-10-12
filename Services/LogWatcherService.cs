using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WIMP_IntelLog.Dtos;
using WIMP_IntelLog.Models;
using WIMP_IntelLog.SynchronousDataServices;

namespace WIMP_IntelLog.Services
{
    public class LogWatcherService : ILogWatcherService
    {
        private readonly FileSystemWatcher _fileSystemWatcher;
        private readonly ILogger<LogWatcherService> _logger;
        private readonly string _logDirectory;
        private readonly string _intelChannelName;
        private readonly IUserDataService _userDataService;
        private readonly IReportIntelService _reportIntelService;
        private readonly string _logFileFilter;

        private readonly Dictionary<string, FileReference> _chatLogFiles;

        public LogWatcherService(ILoggerFactory loggerFactory, IConfigurationRoot config, IUserDataService userDataService, IReportIntelService reportIntelService)
        {
            _logger = loggerFactory.CreateLogger<LogWatcherService>();
            _logDirectory = config["EveLogDirectory"];
            _intelChannelName = config["IntelChannelName"];
            _userDataService = userDataService;
            _reportIntelService = reportIntelService;

            _logFileFilter = $"{_intelChannelName}_*.txt";

            _logger.LogInformation($"Watching chat logs in: {_logDirectory}");
            _logger.LogInformation($"Watching chat channel: {_intelChannelName}");

            _chatLogFiles = new Dictionary<string, FileReference>();

            if (!Directory.Exists(_logDirectory))
            {
                throw new ArgumentException($"{_logDirectory} doesn't exist");
            }

            _fileSystemWatcher = SetupFileSystemWatcher();
        }

        private FileSystemWatcher SetupFileSystemWatcher()
        {
            var fileSystemWatcher = new FileSystemWatcher
            {
                Path = _logDirectory,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite,
                Filter = _logFileFilter,
            };

            fileSystemWatcher.Created += OnChanged;
            fileSystemWatcher.Changed += OnChanged;

            return fileSystemWatcher;
        }

        private Dictionary<string, FileReference> FindLogFiles()
        {
            return Directory.GetFiles(_logDirectory, _logFileFilter)
                .Select(path => new FileReference { Path = path, LastWrite = File.GetLastWriteTime(path) })
                .ToDictionary(f => f.Path, f => f);
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            _logger.LogDebug($"new log file: {e.FullPath}");

            var fileReference = new FileReference
            {
                Path = e.FullPath,
                LastWrite = File.GetLastWriteTime(e.FullPath)
            };

            _chatLogFiles[e.FullPath] = fileReference;
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            _logger.LogDebug($"changed log file: {e.FullPath}");

            var fileReference = new FileReference
            {
                Path = e.FullPath,
                LastWrite = File.GetLastWriteTime(e.FullPath)
            };

            _chatLogFiles[e.FullPath] = fileReference;
        }

        public async Task RunAsync()
        {
            var existingFiles = FindLogFiles();
            foreach (var file in existingFiles)
            {
                _chatLogFiles.Add(file.Key, file.Value);
            }

            _logger.LogInformation($"found {_chatLogFiles.Count} log files");

            _fileSystemWatcher.EnableRaisingEvents = true;

            FileReference currentFile = null;

            FileStream fs = null;
            StreamReader sr = null;

            while (true)
            {
                var mostRecentFile = _chatLogFiles.Values
                    .OrderByDescending(f => f.LastWrite)
                    .FirstOrDefault();

                if (mostRecentFile != currentFile)
                {
                    _logger.LogInformation($"change log file from {currentFile?.Path ?? "(null)"} to {mostRecentFile?.Path}");

                    sr?.Dispose();
                    fs?.Dispose();

                    currentFile = mostRecentFile;

                    fs = new FileStream(currentFile.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    sr = new StreamReader(fs, Encoding.Unicode);
                }

                while (sr != null)
                {
                    var line = await sr.ReadLineAsync();
                    if (line == null) break;

                    _logger.LogDebug($"read line: {line}");

                    var createIntelDto = ParseIntelLine(line);
                    if (createIntelDto == null) continue;
                    if (string.IsNullOrWhiteSpace(createIntelDto.ReportedBy)) continue;
                    if (createIntelDto.ReportedBy == "EVE System") continue;

                    var intelDate = DateTime.Parse(createIntelDto.Timestamp);
                    if (intelDate > _userDataService.UserData.LastSubmittedIntelReportDate)
                    {
                        try
                        {
                            await _reportIntelService.SendIntelReport(createIntelDto);

                            _userDataService.UserData.LastSubmittedIntelReportDate = intelDate;
                            _userDataService.Save();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Couldn't send intel report");
                            await Task.Delay(5000);
                        }
                    }
                }

                await Task.Delay(20);
            }
        }

        private CreateIntelDto ParseIntelLine(string line)
        {
            var chatMessagePattern = @"\[\ ([0-9:.\ ]*?)\ \]\ (.*?)\ \>\ (.*)";

            var createIntelDto = new CreateIntelDto();

            try
            {
                var regexMatch = Regex.Match(line, chatMessagePattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                if (regexMatch.Success)
                {
                    var timestamp = regexMatch.Groups[1].Value;
                    var sender = regexMatch.Groups[2].Value;
                    var message = regexMatch.Groups[3].Value;

                    createIntelDto.Timestamp = timestamp;
                    createIntelDto.ReportedBy = sender;
                    createIntelDto.Message = message;

                    _logger.LogDebug($"timestamp: {timestamp}, sender: {sender}, message: {message}");
                }
                else
                {
                    return null;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                _logger.LogWarning("Timed out while parsing intel line.");

                return null;
            }

            return createIntelDto;
        }
    }
}