using Microsoft.EntityFrameworkCore;
using Planora.DAL.Models;
using System.Threading.Tasks;

namespace Planora.DAL.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(PlanoraDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
