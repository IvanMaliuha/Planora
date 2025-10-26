using MediatR;
using Planora.DAL;
using Planora.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace Planora.BLL.Handlers.Commands.Groups
{
    public class AddGroupHandler : IRequestHandler<AddGroupCommand, bool>
    {
        private readonly PlanoraDbContext _context;

        public AddGroupHandler(PlanoraDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(AddGroupCommand request, CancellationToken cancellationToken)
        {
            var data = request.GroupData;

            // БІЗНЕС-ПРАВИЛО: Перевірка на унікальність імені групи (наприклад, "ПМ-21" може бути лише один раз)
            var existingGroup = await _context.Groups
                                              .AsNoTracking()
                                              .AnyAsync(g => g.Name == data.Name, cancellationToken);
            
            if (existingGroup)
            {
                return false; // Група з таким ім'ям вже існує
            }

            // Створення нової сутності Group
            var newGroup = new Group
            {
                Name = data.Name,
                Faculty = data.Faculty,
                StudentCount = data.StudentCount 
            };

            _context.Groups.Add(newGroup);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}