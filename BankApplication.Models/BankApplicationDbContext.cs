using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApplication.Models
{
    public class BankApplicationDbContext:DbContext
    {
        public DbSet<Bank> Banks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString: @"Data Source=VENKAT-ELON\SQLEXPRESS;Initial Catalog=BankApplicationDB;Integrated Security=True");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bank>().HasKey(v => v.Id);
            modelBuilder.Entity<Bank>().Property(p => p.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<Employee>().HasKey(e => e.Id);
            modelBuilder.Entity<Employee>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Employee>()
                .HasOne(p => p.Bank)
                .WithMany(b => b.Employees)
                .HasForeignKey(p => p.BankId);

            modelBuilder.Entity<AccountHolder>()
                .HasOne(p => p.Bank)
                .WithMany(b => b.AccountHolders)
                .HasForeignKey(p => p.BankId);

            modelBuilder.Entity<Branch>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<Branch>()
                .HasOne(p => p.Bank)
                .WithMany(b => b.Branches)
                .HasForeignKey(p => p.BankId);

            modelBuilder.Entity<CurrencyCode>().Property(p => p.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<CurrencyCode>()
                .HasOne(p => p.Bank)
                .WithMany(b => b.CurrencyCodes)
                .HasForeignKey(p => p.BankId);

        }

    }
}
