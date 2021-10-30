// <copyright file="AppConfiguration.cs" company="WIMP">
// Copyright (c) WIMP. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace WIMP_IntelLog
{
    using System;
    using System.Collections.Generic;

    internal class AppConfiguration
    {
        public static IReadOnlyDictionary<string, string> DefaultConfigurationStrings { get; } =
            new Dictionary<string, string>()
            {
                ["EveLogDirectory"] = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\EVE\\logs\\Chatlogs",
                ["IntelChannelNames:0"] = "MyIntelChannel",
                ["IntelEndpoint"] = "http://localhost:5000/intel/",
                ["IntelApiKey"] = "INTEL_API_KEY_HERE",
            };

        public string EveLogDirectory { get; set; }

        public string IntelChannelName { get; set; }

        public string[] IntelChannelNames { get; set; }

        public string IntelEndpoint { get; set; }

        public string IntelApiKey { get; set; }
    }
}