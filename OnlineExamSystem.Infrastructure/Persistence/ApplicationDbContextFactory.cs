using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace OnlineExamSystem.Infrastructure.Persistence
{
    public class ApplicationDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.GetFullPath(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "..",
                    "OnlineExamSystem.Web"));

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var optionsBuilder =
                new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));

            return new ApplicationDbContext(
                optionsBuilder.Options);
        }
    }
}