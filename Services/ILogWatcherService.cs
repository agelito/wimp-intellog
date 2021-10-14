// <copyright file="ILogWatcherService.cs" company="WIMP">
// Copyright (c) WIMP. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace WIMP_IntelLog.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a service for watching log files.
    /// </summary>
    internal interface ILogWatcherService
    {
        /// <summary>
        /// Run this service.
        /// </summary>
        /// <returns>An asynchronous task.</returns>
        Task RunAsync();
    }
}