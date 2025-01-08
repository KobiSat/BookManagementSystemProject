// Ensures the migration tool can construct the AppDbContext with the correct options at design time.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookManagementSystem
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlite("Data Source=BookManagement.db"); // Match your connection string

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
