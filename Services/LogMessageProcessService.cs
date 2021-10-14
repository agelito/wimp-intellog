using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WIMP_IntelLog.Dtos;
using WIMP_IntelLog.SynchronousDataServices;

namespace WIMP_IntelLog.Services
{
    public class LogMessageProcessService : ILogMessageProcessService
    {
        private readonly ILogger<LogMessageProcessService> _logger;
        private readonly IReportIntelService _reportIntelService;
        private readonly IUserDataService _userDataService;

        public LogMessageProcessService(ILoggerFactory loggerFactory, IReportIntelService reportIntelService, IUserDataService userDataService)
        {
            _logger = loggerFactory.CreateLogger<LogMessageProcessService>();
            _reportIntelService = reportIntelService;
            _userDataService = userDataService;
        }

        public async Task ProcessLogMessage(string messageLine)
        {
            var createIntelDto = ParseIntelLine(messageLine);
            if (createIntelDto == null) return;

            if (string.IsNullOrWhiteSpace(createIntelDto.ReportedBy)) return;
            if (createIntelDto.ReportedBy == "EVE System") return;

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