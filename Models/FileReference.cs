// <copyright file="FileReference.cs" company="WIMP">
// Copyright (c) WIMP. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace WIMP_IntelLog.Models
{
    using System;

    internal class FileReference
    {
        public string Path { get; set; }

        public DateTime LastWrite { get; set; }
    }
}