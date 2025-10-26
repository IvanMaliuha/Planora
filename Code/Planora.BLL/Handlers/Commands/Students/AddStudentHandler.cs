using MediatR;
using Planora.DAL;
using Planora.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planora.BLL.Handlers.Commands.Students
{
    // Handler для обробки команди додавання студента
    public class AddStudentHandler : IRequestHandler<AddStudentCommand, bool>
    {
        private readonly PlanoraDbContext _context;

        public AddStudentHandler(PlanoraDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(AddStudentCommand request, CancellationToken cancellationToken)
        {
            var data = request.StudentData;
            
            // 1. ПЕРЕВІРКА: Чи існує користувач з таким Email
            var existingUser = await _context.Users
                                             .AsNoTracking()
                                             .AnyAsync(u => u.Email == data.Email, cancellationToken);
            
            if (existingUser)
            {
                // Повертаємо false, якщо користувач вже існує
                return false; 
            }

            // 2. ПЕРЕВІРКА І ОТРИМАННЯ: Знайти групу за назвою
            // Включаємо Students, щоб перевірити поточну кількість
            var group = await _context.Groups
                                      .Include(g => g.Students) 
                                      .FirstOrDefaultAsync(g => g.Name == data.GroupName, cancellationToken);

            if (group == null)
            {
                // Не існує групи з такою назвою
                return false; 
            }
            
            // 3. ПЕРЕВІРКА: Чи є місце в групі
            if (group.Students.Count >= group.StudentCount)
            {
                // Місткість групи вичерпана
                return false; 
            }

            // 4. Створення нової сутності Student (використовуючи TPH спадкування)
            var newStudent = new Student
            {
                // Поля успадковані від User
                FullName = data.FullName,
                Email = data.Email,
                PasswordHash = "DEFAULT_HASH", 
                Role = "student",
                
                // Специфічні поля Student
                Faculty = data.Faculty,
                GroupId = group.Id, // Зв'язуємо з існуючою групою
            };

            // Додаємо студента (він автоматично додається і до таблиці Users)
            _context.Users.Add(newStudent); 
            
            // Зберігаємо зміни
            await _context.SaveChangesAsync(cancellationToken);

            return true; 
        }
    }
}
