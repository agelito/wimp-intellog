// <copyright file="LogWatcherServiceRunner.cs" company="WIMP">
// Copyright (c) WIMP. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace WIMP_IntelLog.Services
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    internal class LogWatcherServiceRunner : BackgroundService
    {
        private readonly ILogger<LogWatcherServiceRunner> logger;
        private readonly IOptionsMonitor<AppConfiguration> options;
        private readonly IServiceProvider serviceProvider;

        public LogWatcherServiceRunner(IServiceProvider serviceProvider, ILogger<LogWatcherServiceRunner> logger, IOptionsMonitor<AppConfiguration> options)
        {
            this.logger = logger;
            this.options = options;
            this.serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var configuredIntelChannels = this.options.CurrentValue.IntelChannelNames;
            var intelChannelNames = (configuredIntelChannels ?? Array.Empty<string>())
                .Append(this.options.CurrentValue.IntelChannelName)
                .Where(s => !string.IsNullOrEmpty(s))
                .Distinct();

            this.logger.LogInformation($"starting log watchers for {intelChannelNames.Count()} channels");

            var tasks = intelChannelNames.Select(s => Task.Run(
                async () =>
                {
                    using var scope = this.serviceProvider.CreateScope();

                    var logWatcherService =
                        scope.ServiceProvider.GetRequiredService<ILogWatcherService>();

                    await logWatcherService
                        .RunAsync(s, cancellationToken)
                        .ConfigureAwait(false);
                }, cancellationToken));

            return Task.WhenAll(tasks);
        }
    }
}