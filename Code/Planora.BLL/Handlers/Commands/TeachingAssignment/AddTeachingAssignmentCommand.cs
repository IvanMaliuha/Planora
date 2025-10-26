using MediatR;
using Planora.BLL.DTOs;

namespace Planora.BLL.Handlers.Commands.TeachingAssignments
{
    public class AddTeachingAssignmentCommand : IRequest<bool>
    {
        public AddTeachingAssignmentDto AssignmentData { get; }

        public AddTeachingAssignmentCommand(AddTeachingAssignmentDto assignmentData)
        {
            AssignmentData = assignmentData;
        }
    }
}
