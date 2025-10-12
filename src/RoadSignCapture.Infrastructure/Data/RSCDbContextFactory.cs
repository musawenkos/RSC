using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace RoadSignCapture.Infrastructure.Data
{
    public class RSCDbContextFactory : IDesignTimeDbContextFactory<RSCDbContext>
    {
        public RSCDbContext CreateDbContext(string[] args)
        {
            // Build configuration from Web project's appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../src/RoadSignCapture.Web"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine($"Using connection string: {connectionString}");

            var optionsBuilder = new DbContextOptionsBuilder<RSCDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new RSCDbContext(optionsBuilder.Options);
        }
    }
}
