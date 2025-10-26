using MediatR;
using Planora.DAL;
using Planora.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Planora.BLL.Handlers.Commands.TeachingAssignments
{
    public class AddTeachingAssignmentHandler : IRequestHandler<AddTeachingAssignmentCommand, bool>
    {
        private readonly PlanoraDbContext _context;

        public AddTeachingAssignmentHandler(PlanoraDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(AddTeachingAssignmentCommand request, CancellationToken cancellationToken)
        {
            var data = request.AssignmentData;

            // БІЗНЕС-ПРАВИЛО: уникаємо дублювання — викладач не може бути призначений
            // на той самий предмет двічі
            var existingAssignment = await _context.TeachingAssignments
                .AsNoTracking()
                .AnyAsync(a => a.UserId == data.UserId && a.SubjectId == data.SubjectId, cancellationToken);

            if (existingAssignment)
            {
                return false; // Таке призначення вже існує
            }

            // Створення нової сутності TeachingAssignment
            var newAssignment = new TeachingAssignment
            {
                UserId = data.UserId,
                SubjectId = data.SubjectId,
                Hours = data.Hours
            };

            _context.TeachingAssignments.Add(newAssignment);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
