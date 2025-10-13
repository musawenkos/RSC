using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace RoadSignCapture.Infrastructure.Data
{
    public class RSCDbContextFactory : IDesignTimeDbContextFactory<RSCDbContext>
    {
        public RSCDbContext CreateDbContext(string[] args)
        {
            // Build configuration from multiple sources
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../../src/RoadSignCapture.Web"))
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            // Try to get connection string from multiple sources in priority order
            var connectionString =
                configuration.GetConnectionString("DefaultConnection") ??
                Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") ??
                configuration["ConnectionStrings:DefaultConnection"];

            Console.WriteLine($"Using connection string: {connectionString}");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' not found. " +
                    "Provide it via appsettings.json, environment variable, or --connection parameter.");
            }

            Console.WriteLine($"Using connection string: {connectionString}");

            var optionsBuilder = new DbContextOptionsBuilder<RSCDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new RSCDbContext(optionsBuilder.Options);
        }
    }
}