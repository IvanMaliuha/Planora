// Planora.BLL/Handlers/Queries/Teachers/FindTeacherLocationHandler.cs
using MediatR;
using Planora.DAL;
using Planora.BLL.DTOs.Queries;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Planora.BLL.Handlers.Queries.Teachers
{
    public class FindTeacherLocationHandler : IRequestHandler<FindTeacherLocationQuery, TeacherLocationResultDto>
    {
        private readonly PlanoraDbContext _context;

        public FindTeacherLocationHandler(PlanoraDbContext context)
        {
            _context = context;
        }

        public async Task<TeacherLocationResultDto> Handle(FindTeacherLocationQuery request, CancellationToken cancellationToken)
        {
            var data = request.SearchData;

            // Крок 1: Знайти викладача за ПІБ
            var teacher = await _context.Teachers
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.FullName.ToLower() == data.FullName.ToLower(), cancellationToken);

            if (teacher == null)
            {
                return new TeacherLocationResultDto { Message = "❌ Викладача з таким ПІБ не знайдено." };
            }

            // Крок 2: Отримати факультет викладача
            var baseResult = new TeacherLocationResultDto { TeacherFaculty = teacher.Faculty };

            // Крок 3: Пошук поточної пари
            // Шукаємо пару, яка відбувається ПРЯМО ЗАРАЗ
            var currentSchedule = await _context.Schedules
                .AsNoTracking()
                .Include(s => s.Classroom) // Включаємо аудиторію для отримання корпусу та номера
                .Where(s => s.UserId == teacher.Id)
                .Where(s => s.DayOfWeek == data.CurrentDayOfWeek)
                // Перевірка, чи поточний час знаходиться між StartTime та EndTime пари
                .Where(s => s.StartTime <= data.CurrentTime && s.EndTime >= data.CurrentTime)
                .FirstOrDefaultAsync(cancellationToken);

            if (currentSchedule != null)
            {
                // Викладач зараз викладає
                return new TeacherLocationResultDto
                {
                    IsCurrentlyTeaching = true,
                    StartTime = currentSchedule.StartTime,
                    EndTime = currentSchedule.EndTime,
                    ClassroomNumber = currentSchedule.Classroom?.Number ?? "N/A",
                    Building = currentSchedule.Classroom?.Building ?? "N/A",
                    TeacherFaculty = teacher.Faculty,
                    Message = "✅ Викладач зараз на парі."
                };
            }
            
            // Крок 4: Пошук найближчої майбутньої пари на сьогодні
            var nextSchedule = await _context.Schedules
                .AsNoTracking()
                .Include(s => s.Classroom)
                .Where(s => s.UserId == teacher.Id)
                .Where(s => s.DayOfWeek == data.CurrentDayOfWeek)
                // Шукаємо пари, які почнуться пізніше, ніж поточний час
                .Where(s => s.StartTime > data.CurrentTime)
                .OrderBy(s => s.StartTime)
                .FirstOrDefaultAsync(cancellationToken);
            
            if (nextSchedule != null)
            {
                // У викладача буде пара пізніше сьогодні
                return new TeacherLocationResultDto
                {
                    IsCurrentlyTeaching = false,
                    StartTime = nextSchedule.StartTime,
                    EndTime = nextSchedule.EndTime,
                    ClassroomNumber = nextSchedule.Classroom?.Number ?? "N/A",
                    Building = nextSchedule.Classroom?.Building ?? "N/A",
                    TeacherFaculty = teacher.Faculty,
                    Message = "✅ Наступна пара викладача сьогодні."
                };
            }

            // Якщо пар немає (ні зараз, ні пізніше)
            baseResult.Message = "ℹ️ Сьогодні пар немає. Факультет викладача.";
            return baseResult;
        }
    }
}