// <copyright file="UserData.cs" company="WIMP">
// Copyright (c) WIMP. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace WIMP_IntelLog.Models
{
    using System;
    using System.Collections.Generic;

    internal class UserData
    {
        public Dictionary<string, DateTime> LastSubmittedChatChannelDates { get; set; } = new Dictionary<string, DateTime>();
    }
}