using Microsoft.EntityFrameworkCore;
using Planora.DAL;
using Planora.BLL.DTOs;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Planora.BLL.Services
{
    public class WorkloadService : IWorkloadService
    {
        private readonly PlanoraDbContext _context;

        public WorkloadService(PlanoraDbContext context)
        {
            _context = context;
        }

        public async Task<List<WorkloadDto>> GenerateWorkloadAsync(CancellationToken cancellationToken = default)
        {
            // 1. Завантажуємо всі GroupDisciplineLists з пов'язаними даними
            // Це основний перелік, який визначає, що має викладатися.
            var gdlRecords = await _context.GroupDisciplineLists
                .Include(gdl => gdl.Group)
                .Include(gdl => gdl.Subject)
                .Where(gdl => gdl.Group != null && gdl.Subject != null && gdl.Subject.Duration.HasValue)
                .ToListAsync(cancellationToken);

            // 2. Завантажуємо всі TeachingAssignments (призначення викладачів)
            var assignments = await _context.TeachingAssignments
                .Include(ta => ta.Teacher)
                .Where(ta => ta.Teacher != null)
                .ToListAsync(cancellationToken);
            
            // 3. Завантажуємо аудиторії для автоматичного підбору
            var classrooms = await _context.Classrooms.ToListAsync(cancellationToken);

            var workloads = new List<WorkloadDto>();

            foreach (var gdl in gdlRecords)
            {
                // Знаходимо викладача для цього предмета
                var ta = assignments.FirstOrDefault(t => t.SubjectId == gdl.SubjectId);

                // Знаходимо першу доступну аудиторію за факультетом групи
                var classroom = classrooms.FirstOrDefault(c => c.Faculty == gdl.Group.Faculty);

                // Створення WorkloadDto
                workloads.Add(new WorkloadDto
                {
                    // КЛЮЧІ
                    UserId = ta?.UserId ?? 0, // Id викладача. Якщо 0, предмет не призначений
                    SubjectId = gdl.SubjectId,
                    GroupId = gdl.GroupId,
                    Duration = gdl.Subject.Duration.Value, // Години

                    // ДЕТАЛІ
                    TeacherName = ta?.Teacher?.FullName ?? "Не призначено",
                    GroupName = gdl.Group.Name,
                    SubjectName = gdl.Subject.Name,
                    SubjectType = gdl.Subject.Type ?? "Основна",
                    
                    // АУДИТОРІЯ
                    ClassroomNumber = classroom?.Number ?? "N/A",
                    ClassroomBuilding = classroom?.Building ?? "N/A",
                    ClassroomCapacity = classroom?.Capacity ?? 0,
                    HasProjector = classroom?.HasProjector ?? false,
                    HasComputers = classroom?.HasComputers ?? false
                });
            }

            // Повертаємо тільки записи, де призначено викладача і є години
            return workloads.Where(w => w.UserId > 0 && w.Duration > 0).ToList();
        }
    }
}