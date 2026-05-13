using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore.Design;

namespace LibraryInfrastructure.Persistence
{
    public class LibraryDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
    {
        public LibraryDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>();
            // Cấu hình tạm thời để EF Core có thể tạo Migration
            optionsBuilder.UseSqlServer("Server=.;Database=LibPro;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;");
            return new LibraryDbContext(optionsBuilder.Options);
        }
    }
}