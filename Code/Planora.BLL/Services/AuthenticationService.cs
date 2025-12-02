using System.Linq;

namespace Planora.BLL.Services
{
    public interface IAuthenticationService
    {
        bool Authenticate(string login, string password);
    }

    public class AuthenticationService : IAuthenticationService
    {
        public bool Authenticate(string login, string password)
        {
    
            var testUsers = new[]
            {
                new { Login = "admin", Password = "Admin111" },
                new { Login = "teacher", Password = "Teacher111" },
                new { Login = "student", Password = "Student111" }
            };

            return testUsers.Any(u => u.Login == login && u.Password == password);
        }
    }
}