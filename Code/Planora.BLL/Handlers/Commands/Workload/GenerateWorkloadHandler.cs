using MediatR;
using Planora.BLL.DTOs;
using Planora.BLL.Services;

namespace Planora.BLL.Handlers.Commands.Workload
{
    public class GenerateWorkloadHandler : IRequestHandler<GenerateWorkloadCommand, List<WorkloadDto>>
    {
        private readonly IWorkloadService _workloadService;

        public GenerateWorkloadHandler(IWorkloadService workloadService)
        {
            _workloadService = workloadService;
        }

        public async Task<List<WorkloadDto>> Handle(GenerateWorkloadCommand request, CancellationToken cancellationToken)
        {
            return await _workloadService.GenerateWorkloadAsync(cancellationToken);
        }
    }
}
