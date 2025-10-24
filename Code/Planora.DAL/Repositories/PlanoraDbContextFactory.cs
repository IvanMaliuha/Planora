using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Planora.DAL
{
    public class PlanoraDbContextFactory : IDesignTimeDbContextFactory<PlanoraDbContext>
    {
        public PlanoraDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<PlanoraDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new PlanoraDbContext(optionsBuilder.Options);
        }
    }
}
