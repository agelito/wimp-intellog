using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WIMP_IntelLog.Dtos;

namespace WIMP_IntelLog.SynchronousDataServices
{
    public class ReportIntelService : IReportIntelService
    {
        private readonly IConfigurationRoot _configuration;
        private readonly ILogger<ReportIntelService> _logger;
        private readonly HttpClient _httpClient;

        public ReportIntelService(HttpClient httpClient, IConfigurationRoot configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<ReportIntelService>();
            _httpClient = httpClient;
        }

        public async Task<bool> SendIntelReport(CreateIntelDto createIntelDto)
        {
            var requestContent = new StringContent(JsonSerializer.Serialize(createIntelDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_configuration["IntelReportService"]}/intel/", requestContent);
            _logger.LogDebug($"CreateIntel request response: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"CreateIntel request failed with status: {response.StatusCode}");
            }

            return response.IsSuccessStatusCode;
        }
    }
}