using MediatR;
using Planora.BLL.DTOs;

namespace Planora.BLL.Handlers.Commands.Classrooms
{
    public class AddClassroomCommand : IRequest<bool>
    {
        public AddClassroomDto ClassroomData { get; }

        public AddClassroomCommand(AddClassroomDto classroomData)
        {
            ClassroomData = classroomData;
        }
    }
}
