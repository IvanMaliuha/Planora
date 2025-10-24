using Microsoft.EntityFrameworkCore;
using Planora.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planora.DAL.Repositories
{
    public class ClassroomRepository : GenericRepository<Classroom>
    {
        public ClassroomRepository(PlanoraDbContext context) : base(context) { }

        public async Task<IEnumerable<Classroom>> GetAvailableClassroomsAsync(DateTime date, TimeOnly startTime, TimeOnly endTime)
        {
            var dayOfWeek = (int)date.DayOfWeek;
            
            var busyClassrooms = await _context.Schedules
                .Where(s => s.DayOfWeek == dayOfWeek &&
                           s.StartTime < endTime &&
                           s.EndTime > startTime)
                .Select(s => s.ClassroomId)
                .ToListAsync();

            return await _context.Classrooms
                .Where(c => !busyClassrooms.Contains(c.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<Classroom>> GetByFacultyAsync(string faculty)
        {
            return await _context.Classrooms
                .Where(c => c.Faculty == faculty)
                .ToListAsync();
        }
    }
}