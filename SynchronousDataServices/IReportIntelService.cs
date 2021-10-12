using System.Threading.Tasks;
using WIMP_IntelLog.Dtos;

namespace WIMP_IntelLog.SynchronousDataServices
{
    public interface IReportIntelService
    {
        Task<bool> SendIntelReport(CreateIntelDto createIntelDto);
    }
}