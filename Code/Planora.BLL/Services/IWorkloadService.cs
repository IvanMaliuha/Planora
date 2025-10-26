using Planora.BLL.DTOs;

namespace Planora.BLL.Services
{
    public interface IWorkloadService
    {
        Task<List<WorkloadDto>> GenerateWorkloadAsync(CancellationToken cancellationToken = default);
    }
}


