// <copyright file="IReportIntelService.cs" company="WIMP">
// Copyright (c) WIMP. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace WIMP_IntelLog.SynchronousDataServices
{
    using System.Threading.Tasks;
    using WIMP_IntelLog.Dtos;

    /// <summary>
    /// Provides methods for reporting intel.
    /// </summary>
    internal interface IReportIntelService
    {
        /// <summary>
        /// Send an intel report.
        /// </summary>
        /// <param name="createIntelDto">The intel report.</param>
        /// <returns>An asynchronous task with boolean indicating success.</returns>
        Task<bool> SendIntelReport(CreateIntelDto createIntelDto);
    }
}