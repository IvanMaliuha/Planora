using MediatR;
using Planora.DAL;
using Planora.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Planora.BLL.Handlers.Commands.Classrooms
{
    public class AddClassroomHandler : IRequestHandler<AddClassroomCommand, bool>
    {
        private readonly PlanoraDbContext _context;

        public AddClassroomHandler(PlanoraDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(AddClassroomCommand request, CancellationToken cancellationToken)
        {
            var data = request.ClassroomData;

            // БІЗНЕС-ПРАВИЛО: перевірка на унікальність номера аудиторії в межах одного корпусу
            var existingClassroom = await _context.Classrooms
                .AsNoTracking()
                .AnyAsync(c => c.Number == data.Number && c.Building == data.Building, cancellationToken);

            if (existingClassroom)
            {
                return false; // Аудиторія з таким номером у цьому корпусі вже існує
            }

            // Створення нової сутності Classroom
            var newClassroom = new Classroom
            {
                Number = data.Number,
                Building = data.Building,
                Capacity = data.Capacity,
                Faculty = data.Faculty,
                HasComputers = data.HasComputers,
                HasProjector = data.HasProjector
            };

            _context.Classrooms.Add(newClassroom);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
