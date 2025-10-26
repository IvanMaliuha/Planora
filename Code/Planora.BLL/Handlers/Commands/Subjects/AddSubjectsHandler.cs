using MediatR;
using Planora.DAL;
using Planora.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Planora.BLL.Handlers.Commands.Subjects
{
    public class AddSubjectHandler : IRequestHandler<AddSubjectCommand, bool>
    {
        private readonly PlanoraDbContext _context;

        public AddSubjectHandler(PlanoraDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(AddSubjectCommand request, CancellationToken cancellationToken)
        {
            var data = request.SubjectData;

            // БІЗНЕС-ПРАВИЛО: Перевірка на унікальність назви предмету
            var existingSubject = await _context.Subjects
                .AsNoTracking()
                .AnyAsync(s => s.Name == data.Name, cancellationToken);

            if (existingSubject)
            {
                return false; // Предмет з такою назвою вже існує
            }

            // Створення нової сутності Subject
            var newSubject = new Subject
            {
                Name = data.Name,
                Type = data.Type,
                Requirements = data.Requirements,
                Duration = data.Duration
            };

            _context.Subjects.Add(newSubject);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
