// <copyright file="CreateIntelDto.cs" company="WIMP">
// Copyright (c) WIMP. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace WIMP_IntelLog.Dtos
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    /// <summary>
    /// Data used to report intel.
    /// </summary>
    internal class CreateIntelDto
    {
        /// <summary>
        /// Gets or sets the intel message text string.
        /// </summary>
        [Required]
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the name of player who reported the intel.
        /// </summary>
        [JsonPropertyName("reportedBy")]
        public string ReportedBy { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the intel message.
        /// </summary>
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
    }
}