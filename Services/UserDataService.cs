using System;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using WIMP_IntelLog.Models;

namespace WIMP_IntelLog.Services
{
    public class UserDataService : IUserDataService
    {
        private readonly ILogger _logger;
        private readonly string _userDataFilePath;

        public UserDataService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<UserDataService>();
            _userDataFilePath = Path.Combine(AppContext.BaseDirectory, "userdata.json");

            Load();
        }

        public UserData UserData { get; set; } = new UserData();

        public void Save()
        {
            try
            {
                var fileContent = JsonSerializer.Serialize(UserData);
                File.WriteAllText(_userDataFilePath, fileContent);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Couldn't save the user data: {ex.Message}");
            }

        }

        private void Load()
        {
            if (!File.Exists(_userDataFilePath)) return;

            var fileContent = File.ReadAllText(_userDataFilePath);

            try
            {
                UserData = JsonSerializer.Deserialize<UserData>(fileContent);
                _logger.LogDebug($"Successfully saved user data at: {_userDataFilePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Couldn't load the user data: {ex.Message}");
            }
        }
    }
}