using Microsoft.EntityFrameworkCore;

namespace EntityFramework
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Purchase> Purchases => Set<Purchase>();

        public ApplicationContext() => Database.EnsureCreated();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=ShopAppDB.db");            
        }
    }
}
