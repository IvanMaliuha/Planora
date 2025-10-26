using MediatR;
using Planora.DAL;
using Planora.DAL.Models;
using Microsoft.EntityFrameworkCore;

// Planora.BLL/Handlers/Commands/Teachers/AddTeacherHandler.cs

// ... (using statements)

namespace Planora.BLL.Handlers.Commands.Teachers
{
    public class AddTeacherHandler : IRequestHandler<AddTeacherCommand, bool>
    {
        private readonly PlanoraDbContext _context;

        public AddTeacherHandler(PlanoraDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(AddTeacherCommand request, CancellationToken cancellationToken)
        {
            var data = request.TeacherData;
            
            // 1. Перевірка на унікальність Email (залишається)
            var existingUser = await _context.Users
                                             .AsNoTracking() // Додайте AsNoTracking, щоб уникнути відстеження під час перевірки
                                             .FirstOrDefaultAsync(u => u.Email == data.Email, cancellationToken);
            
            if (existingUser != null)
            {
                return false; 
            }

            // 2. Створення запису ТІЛЬКИ в спеціалізованій таблиці (Teacher, який успадковує User)
            // Припускаємо, що Teacher успадковує User і має поля User + свої поля (Faculty, Position)
            var newTeacher = new Teacher
            {
                // Поля успадковані від User
                FullName = data.FullName,
                Email = data.Email,
                PasswordHash = "DEFAULT_HASH", 
                Role = "teacher", // Дискримінатор
                
                // Специфічні поля Teacher
                Faculty = data.Faculty,
                Position = data.Position
                
                // Id не встановлюється, він згенерується автоматично
            };

            // EF Core додасть цю сутність, і завдяки спадкуванню, 
            // збереже її як у таблиці Users (включно з Discriminator), так і в таблиці Teachers.
            _context.Users.Add(newTeacher); 
            
            await _context.SaveChangesAsync(cancellationToken);

            return true; 
        }
    }
}