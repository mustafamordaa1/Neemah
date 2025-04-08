using Microsoft.EntityFrameworkCore;
using Neemah.Models;

namespace Neemah.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<FactoryOrder> FactoryOrders { get; set; }
    }
}