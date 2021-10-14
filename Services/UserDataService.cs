// <copyright file="UserDataService.cs" company="WIMP">
// Copyright (c) WIMP. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace WIMP_IntelLog.Services
{
    using System;
    using System.IO;
    using System.Text.Json;
    using Microsoft.Extensions.Logging;
    using WIMP_IntelLog.Models;

    internal class UserDataService : IUserDataService
    {
        private readonly ILogger logger;
        private readonly string userDataFilePath;

        public UserDataService(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<UserDataService>();
            this.userDataFilePath = Path.Combine(AppContext.BaseDirectory, "userdata.json");

            this.Load();
        }

        public UserData UserData { get; set; } = new UserData();

        public void Save()
        {
            try
            {
                var fileContent = JsonSerializer.Serialize(this.UserData);
                File.WriteAllText(this.userDataFilePath, fileContent);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Couldn't save the user data: {ex.Message}");
            }
        }

        private void Load()
        {
            if (!File.Exists(this.userDataFilePath))
            {
                return;
            }

            var fileContent = File.ReadAllText(this.userDataFilePath);

            try
            {
                this.UserData = JsonSerializer.Deserialize<UserData>(fileContent);
                this.logger.LogDebug($"Successfully saved user data at: {this.userDataFilePath}");
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Couldn't load the user data: {ex.Message}");
            }
        }
    }
}