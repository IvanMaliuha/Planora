using MediatR;
using Planora.BLL.DTOs;
using System.Collections.Generic;

namespace Planora.BLL.Handlers.Commands.Workload
{
    public class GenerateWorkloadCommand : IRequest<List<WorkloadDto>>
    {
    }
}
