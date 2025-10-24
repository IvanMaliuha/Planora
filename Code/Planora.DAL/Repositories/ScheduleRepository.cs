using Microsoft.EntityFrameworkCore;
using Planora.DAL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planora.DAL.Repositories
{
    public class ScheduleRepository : GenericRepository<Schedule>
    {
        public ScheduleRepository(PlanoraDbContext context) : base(context) { }

        public async Task<IEnumerable<Schedule>> GetByTeacherAsync(int teacherId)
        {
            return await _context.Schedules
                .Include(s => s.Subject)
                .Include(s => s.Group)
                .Include(s => s.Classroom)
                .Where(s => s.UserId == teacherId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetByGroupAsync(int groupId)
        {
            return await _context.Schedules
                .Include(s => s.Subject)
                .Include(s => s.User)
                .Include(s => s.Classroom)
                .Where(s => s.GroupId == groupId)
                .ToListAsync();
        }
    }
}