using ApplicationLayer.Persistance;
using DomainLayer.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.Persistance
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
            
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Primary_Keys
            modelBuilder
                .Entity<Customer>()
                .HasKey(x => x.Id);

            modelBuilder
                .Entity<Account>()
                .HasKey(x => x.Id);

            modelBuilder
                .Entity<Transaction>()
                .HasKey(x => x.Id);
            #endregion

            #region RelationShips
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Customer)
                .WithOne(c => c.Account)
                .HasForeignKey<Customer>(foreignKeyExpression: c => c.AccountId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Transaction>()
                .HasOne(navigationExpression: t => t.Sender)
                .WithOne(navigationExpression: t => t.Transaction)
                .HasForeignKey<Account>(a => a.TransactionId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Transaction>()
                .HasOne(navigationExpression: t => t.Recipient)
                .WithOne(navigationExpression: t => t.Transaction)
                .HasForeignKey<Account>(a => a.TransactionId)
                .OnDelete(DeleteBehavior.NoAction);
            #endregion

        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

    }
}
