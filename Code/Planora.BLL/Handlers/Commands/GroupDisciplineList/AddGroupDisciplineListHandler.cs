using MediatR;
using Planora.DAL;
using Planora.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Planora.BLL.Handlers.Commands.GroupDisciplineLists
{
    public class AddGroupDisciplineListHandler : IRequestHandler<AddGroupDisciplineListCommand, bool>
    {
        private readonly PlanoraDbContext _context;

        public AddGroupDisciplineListHandler(PlanoraDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(AddGroupDisciplineListCommand request, CancellationToken cancellationToken)
        {
            var data = request.ListData;

            // БІЗНЕС-ПРАВИЛО: уникаємо дублювання — група не може мати однаковий предмет двічі
            var existingList = await _context.GroupDisciplineLists
                .AsNoTracking()
                .AnyAsync(l => l.GroupId == data.GroupId && l.SubjectId == data.SubjectId, cancellationToken);

            if (existingList)
            {
                return false; // Такий запис уже існує
            }

            // Створення нової сутності GroupDisciplineList
            var newList = new GroupDisciplineList
            {
                GroupId = data.GroupId,
                SubjectId = data.SubjectId,
                Hours = data.Hours
            };

            _context.GroupDisciplineLists.Add(newList);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
