// <copyright file="ILogWatcherService.cs" company="WIMP">
// Copyright (c) WIMP. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace WIMP_IntelLog.Services
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a service for watching log files.
    /// </summary>
    internal interface ILogWatcherService
    {
        /// <summary>
        /// Run this service.
        /// </summary>
        /// <param name="chatChannelName">The chat channel name.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An asynchronous task.</returns>
        Task RunAsync(string chatChannelName, CancellationToken cancellationToken);
    }
}