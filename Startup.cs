// <copyright file="Startup.cs" company="WIMP">
// Copyright (c) WIMP. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace WIMP_IntelLog
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using WIMP_IntelLog.Services;
    using WIMP_IntelLog.SynchronousDataServices;

    internal static class Startup
    {
        public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
        {
            services.Configure<AppConfiguration>(configuration);
            services.AddHttpClient<IReportIntelService, ReportIntelService>();

            services.AddSingleton<IUserDataService, UserDataService>();

            services.AddScoped<ILogMessageProcessService, LogMessageProcessService>();
            services.AddScoped<ILogWatcherService, LogWatcherService>();

            services.AddHostedService<LogWatcherServiceRunner>();
        }
    }
}