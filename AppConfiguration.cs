using System;
using System.Collections.Generic;

namespace WIMP_IntelLog
{
    public class AppConfiguration
    {
        public static IReadOnlyDictionary<string, string> DefaultConfigurationStrings { get; } =
            new Dictionary<string, string>()
            {
                [$"EveLogDirectory"] = $"C:\\Users\\{Environment.UserName}\\Documents\\EVE\\logs\\Chatlogs",
                [$"IntelChannelName"] = "WOMP intel",
            };

        public string EveLogDirectory { get; set; }
        public string IntelChannelName { get; set; }
    }
}