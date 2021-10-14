using System.Threading.Tasks;

namespace WIMP_IntelLog.Services
{
    public interface ILogMessageProcessService
    {
        Task ProcessLogMessage(string messageLine);
    }
}