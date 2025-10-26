using MediatR;
using Planora.BLL.DTOs;

namespace Planora.BLL.Handlers.Commands.Teachers
{
    // Команда MediatR, що містить DTO і очікує повернути bool (успіх/помилка)
    public class AddTeacherCommand : IRequest<bool>
    {
        public AddTeacherDto TeacherData { get; }

        public AddTeacherCommand(AddTeacherDto teacherData)
        {
            TeacherData = teacherData;
        }
    }
}