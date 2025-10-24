using Microsoft.EntityFrameworkCore;
using Planora.DAL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planora.DAL.Repositories
{
    public class TeacherRepository : GenericRepository<Teacher>
    {
        public TeacherRepository(PlanoraDbContext context) : base(context) { }

        public async Task<IEnumerable<Teacher>> GetByFacultyAsync(string faculty)
        {
            return await _context.Teachers
                .Where(t => t.Faculty == faculty)
                .ToListAsync();
        }
    }
}