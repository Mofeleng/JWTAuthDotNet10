using JWTAuthDotNet10.Entities;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthDotNet10.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options): DbContext(options)
    {
        public DbSet<User> Users { get; set; }

    }
}
