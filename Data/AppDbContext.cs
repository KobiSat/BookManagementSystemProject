using Microsoft.EntityFrameworkCore;
using BookManagementSystem.Models;

namespace BookManagementSystem
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
    }
}
