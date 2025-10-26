using MediatR;
using Planora.BLL.DTOs;

namespace Planora.BLL.Handlers.Commands.GroupDisciplineLists
{
    public class AddGroupDisciplineListCommand : IRequest<bool>
    {
        public AddGroupDisciplineListDto ListData { get; }

        public AddGroupDisciplineListCommand(AddGroupDisciplineListDto listData)
        {
            ListData = listData;
        }
    }
}
