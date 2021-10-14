// <copyright file="LogMessageProcessService.cs" company="WIMP">
// Copyright (c) WIMP. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace WIMP_IntelLog.Services
{
    using System;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using WIMP_IntelLog.Dtos;
    using WIMP_IntelLog.SynchronousDataServices;

    internal class LogMessageProcessService : ILogMessageProcessService
    {
        private readonly ILogger<LogMessageProcessService> logger;
        private readonly IReportIntelService reportIntelService;
        private readonly IUserDataService userDataService;

        public LogMessageProcessService(ILoggerFactory loggerFactory, IReportIntelService reportIntelService, IUserDataService userDataService)
        {
            this.logger = loggerFactory.CreateLogger<LogMessageProcessService>();
            this.reportIntelService = reportIntelService;
            this.userDataService = userDataService;
        }

        public async Task ProcessLogMessage(string messageLine)
        {
            var createIntelDto = ParseIntelLine(messageLine);
            if (createIntelDto == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(createIntelDto.ReportedBy))
            {
                return;
            }

            if (createIntelDto.ReportedBy == "EVE System")
            {
                return;
            }

            var intelDate = DateTime.Parse(createIntelDto.Timestamp);
            if (intelDate > this.userDataService.UserData.LastSubmittedIntelReportDate)
            {
                try
                {
                    await this.reportIntelService
                        .SendIntelReport(createIntelDto)
                        .ConfigureAwait(true);

                    this.userDataService.UserData.LastSubmittedIntelReportDate = intelDate;
                    this.userDataService.Save();
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Couldn't send intel report");
                    await Task.Delay(5000).ConfigureAwait(true);
                }
            }
        }

        private static CreateIntelDto ParseIntelLine(string line)
        {
            const string chatMessagePattern = @"\[\ ([0-9:.\ ]*?)\ \]\ (.*?)\ \>\ (.*)";

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
                }
                else
                {
                    return null;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return null;
            }

            return createIntelDto;
        }
    }
}