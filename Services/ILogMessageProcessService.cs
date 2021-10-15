// <copyright file="ILogMessageProcessService.cs" company="WIMP">
// Copyright (c) WIMP. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace WIMP_IntelLog.Services
{
    using System.Threading.Tasks;

    /// <summary>
    /// Provides methods to process chat messages.
    /// </summary>
    internal interface ILogMessageProcessService
    {
        /// <summary>
        /// Process a chat message line.
        /// </summary>
        /// <param name="channelName">The chat channel name.</param>
        /// <param name="messageLine">The message line.</param>
        /// <returns>Asynchronous task.</returns>
        Task ProcessLogMessage(string channelName, string messageLine);
    }
}