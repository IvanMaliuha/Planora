using MediatR;
using Planora.BLL.DTOs;

namespace Planora.BLL.Handlers.Commands.Subjects
{
    public class AddSubjectCommand : IRequest<bool>
    {
        public AddSubjectDto SubjectData { get; }

        public AddSubjectCommand(AddSubjectDto subjectData)
        {
            SubjectData = subjectData;
        }
    }
}
