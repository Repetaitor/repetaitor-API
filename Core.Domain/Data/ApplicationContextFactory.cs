using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Core.Domain.Data;

public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
{
    public ApplicationContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
        optionsBuilder.UseSqlServer("Server=sql.bsite.net\\MSSQL2016;Database=repetaitor_DB;User Id=repetaitor_DB;Password=dachidachi1;Trusted_Connection=False;TrustServerCertificate=True;");
        return new ApplicationContext(optionsBuilder.Options);
    }
}