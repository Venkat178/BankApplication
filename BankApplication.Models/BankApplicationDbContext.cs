using Microsoft.EntityFrameworkCore;

namespace BankApplication.Models
{
    public class BankApplicationDbContext:DbContext
    {
        public DbSet<Bank> Banks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString: @"Data Source=VENKAT-ELON\SQLEXPRESS;Initial Catalog=BankApplicationDB;Integrated Security=True");
        }

    }
}
