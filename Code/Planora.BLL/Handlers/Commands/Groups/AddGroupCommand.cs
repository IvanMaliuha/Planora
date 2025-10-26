using MediatR;
using Planora.BLL.DTOs;

namespace Planora.BLL.Handlers.Commands.Groups
{
    public class AddGroupCommand : IRequest<bool>
    {
        public AddGroupDto GroupData { get; }

        public AddGroupCommand(AddGroupDto groupData)
        {
            GroupData = groupData;
        }
    }
}