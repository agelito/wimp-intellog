using WIMP_IntelLog.Models;

namespace WIMP_IntelLog.Services
{
    public interface IUserDataService
    {
        UserData UserData { get; set; }

        void Save();
    }
}