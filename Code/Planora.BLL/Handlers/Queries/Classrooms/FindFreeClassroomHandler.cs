using MediatR;
using Planora.DAL;
using Planora.BLL.DTOs.Queries;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Planora.BLL.Handlers.Queries.Classrooms
{
    public class FindFreeClassroomHandler : IRequestHandler<FindFreeClassroomQuery, List<FreeClassroomResultDto>>
    {
        private readonly PlanoraDbContext _context;

        public FindFreeClassroomHandler(PlanoraDbContext context)
        {
            _context = context;
        }

        public async Task<List<FreeClassroomResultDto>> Handle(FindFreeClassroomQuery request, CancellationToken cancellationToken)
        {
            var data = request.SearchData;
            
            // 1. Знайти ID аудиторій, які ЗАЙНЯТІ у вказаний час
            var occupiedClassroomIds = await _context.Schedules
                .AsNoTracking()
                .Where(s => 
                    s.DayOfWeek == data.DayOfWeek &&
                    s.StartTime == data.StartTime
                )
                .Select(s => s.ClassroomId)
                .Distinct()
                .ToListAsync(cancellationToken);

            // 2. Знайти аудиторії, які ВІЛЬНІ та відповідають вимогам
            var freeClassrooms = await _context.Classrooms
                .AsNoTracking()
                .Where(c => !occupiedClassroomIds.Contains(c.Id))
                // Фільтрація за вимогами
                .Where(c => c.Building == data.Building)
                .Where(c => c.Capacity >= data.RequiredCapacity)
                // Якщо комп'ютери потрібні (NeedsComputers=true), HasComputers має бути true. Якщо не потрібні (NeedsComputers=false), приймаємо будь-яку.
                .Where(c => data.NeedsComputers == false || c.HasComputers == true) 
                .Where(c => data.NeedsProjector == false || c.HasProjector == true) 
                // Сортуємо за місткістю (починаючи з меншої)
                .OrderBy(c => c.Capacity) 
                .Select(c => new FreeClassroomResultDto
                {
                    ClassroomId = c.Id,
                    Number = c.Number,
                    Building = c.Building,
                    Capacity = c.Capacity
                })
                .ToListAsync(cancellationToken);

            return freeClassrooms;
        }
    }
}