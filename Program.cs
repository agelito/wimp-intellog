// <copyright file="Program.cs" company="WIMP">
// Copyright (c) WIMP. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace WIMP_IntelLog
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using WIMP_IntelLog.Services;

    internal class Program
    {
        private static async Task Main()
        {
            IServiceCollection services = new ServiceCollection();

            var startup = new Startup();
            startup.ConfigureServices(services);

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            var service = serviceProvider.GetService<ILogWatcherService>();
            await service.RunAsync().ConfigureAwait(false);
        }
    }
}
