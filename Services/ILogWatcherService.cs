using System.Threading.Tasks;

namespace WIMP_IntelLog.Services
{
    public interface ILogWatcherService
    {
        Task RunAsync();
    }
}