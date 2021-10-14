// <copyright file="Startup.cs" company="WIMP">
// Copyright (c) WIMP. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace WIMP_IntelLog
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using WIMP_IntelLog.Services;
    using WIMP_IntelLog.SynchronousDataServices;

    internal class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(AppConfiguration.DefaultConfigurationStrings)
                .AddJsonFile("Config.json", true, true);

            this.Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(opts => opts.AddConsole());

            services.AddSingleton(this.Configuration);
            services.AddHttpClient<IReportIntelService, ReportIntelService>();
            services.AddSingleton<IUserDataService, UserDataService>();
            services.AddSingleton<ILogMessageProcessService, LogMessageProcessService>();
            services.AddSingleton<ILogWatcherService, LogWatcherService>();
        }
    }
}