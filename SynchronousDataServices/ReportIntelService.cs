// <copyright file="ReportIntelService.cs" company="WIMP">
// Copyright (c) WIMP. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace WIMP_IntelLog.SynchronousDataServices
{
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using WIMP_IntelLog.Dtos;

    internal class ReportIntelService : IReportIntelService
    {
        private readonly IConfigurationRoot configuration;
        private readonly ILogger<ReportIntelService> logger;
        private readonly HttpClient httpClient;

        public ReportIntelService(HttpClient httpClient, IConfigurationRoot configuration, ILoggerFactory loggerFactory)
        {
            this.configuration = configuration;
            this.logger = loggerFactory.CreateLogger<ReportIntelService>();
            this.httpClient = httpClient;
        }

        public async Task<bool> SendIntelReport(CreateIntelDto createIntelDto)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(createIntelDto), Encoding.UTF8, "application/json");
            var response = await this.httpClient
                .PostAsync(this.configuration["IntelEndpoint"], requestContent)
                .ConfigureAwait(true);
            this.logger.LogDebug($"CreateIntel request response: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                this.logger.LogError($"CreateIntel request failed with status: {response.StatusCode}");
            }

            return response.IsSuccessStatusCode;
        }
    }
}