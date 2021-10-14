// <copyright file="IUserDataService.cs" company="WIMP">
// Copyright (c) WIMP. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace WIMP_IntelLog.Services
{
    using WIMP_IntelLog.Models;

    /// <summary>
    /// Provides methods for interacting with user data.
    /// </summary>
    internal interface IUserDataService
    {
        /// <summary>
        /// Gets or sets the user data.
        /// </summary>
        UserData UserData { get; set; }

        /// <summary>
        /// Save the user data.
        /// </summary>
        void Save();
    }
}