using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Core.Domain.Data;

public class ApplicationContextFactory(IConfiguration configuration) : IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
        optionsBuilder.UseSqlServer(configuration["Mode"]?.ToLower() == "test"
            ? configuration.GetConnectionString("TestConnection")
            : configuration.GetConnectionString("ProductionConnection"));
        return new ApplicationContext(optionsBuilder.Options, configuration);
    }
}