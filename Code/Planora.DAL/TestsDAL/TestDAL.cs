using Planora.DAL;
using Planora.DAL.Models;
using Planora.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Planora.DAL.Test
{
    public class TestDAL
    {
        public static async Task TestCRUD()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PlanoraDbContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

            using var context = new PlanoraDbContext(optionsBuilder.Options);
            
            var userRepo = new UserRepository(context);
            
            // CREATE
            var testUser = new User 
            { 
                FullName = "Тестовий Користувач", 
                Email = "test@example.com", 
                PasswordHash = "hash", 
                Role = "teacher" 
            };
            await userRepo.AddAsync(testUser);
            Console.WriteLine("Користувача додано");

            // READ
            var users = await userRepo.GetAllAsync();
            Console.WriteLine($"Отримано {users.Count()} користувачів");

            var userByEmail = await userRepo.GetByEmailAsync("test@example.com");
            Console.WriteLine($"Користувача знайдено по email: {userByEmail?.FullName}");

            // UPDATE
            testUser.FullName = "Оновлений Користувач";
            await userRepo.UpdateAsync(testUser);
            Console.WriteLine("Користувача оновлено");

            // DELETE
            await userRepo.DeleteAsync(testUser.Id);
            Console.WriteLine("Користувача видалено");
        }
    }
}