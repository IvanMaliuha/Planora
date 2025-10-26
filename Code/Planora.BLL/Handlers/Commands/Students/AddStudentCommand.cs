using MediatR;
using Planora.BLL.DTOs;

namespace Planora.BLL.Handlers.Commands.Students
{
    public class AddStudentCommand : IRequest<bool>
    {
        public AddStudentDto StudentData { get; }

        public AddStudentCommand(AddStudentDto studentData)
        {
            StudentData = studentData;
        }
    }
}
