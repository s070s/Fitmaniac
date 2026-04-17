using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Fitmaniac.Infrastructure.Persistence;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<FitmaniacDbContext>
{
    public FitmaniacDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FitmaniacDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\MSSQLLocalDB;Database=FitmaniacDb.Design;Trusted_Connection=True;TrustServerCertificate=True;");

        return new FitmaniacDbContext(
            optionsBuilder.Options,
            new Interceptors.AuditSaveChangesInterceptor(new Identity.CurrentUserService()),
            new Interceptors.SoftDeleteInterceptor());
    }
}
