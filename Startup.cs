using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WIMP_IntelLog.Services;
using WIMP_IntelLog.SynchronousDataServices;

namespace WIMP_IntelLog
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(AppConfiguration.DefaultConfigurationStrings)
                .AddJsonFile("Config.json", true, true);

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(opts =>
            {
                opts.AddConsole();
            });

            services.AddSingleton<IConfigurationRoot>(Configuration);
            services.AddHttpClient<IReportIntelService, ReportIntelService>();
            services.AddSingleton<IUserDataService, UserDataService>();
            services.AddSingleton<ILogMessageProcessService, LogMessageProcessService>();
            services.AddSingleton<ILogWatcherService, LogWatcherService>();
        }
    }
}