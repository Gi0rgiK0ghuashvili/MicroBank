using DomainLayer.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ApplicationLayer.Persistance
{
    public interface IApplicationDbContext : IDisposable
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        DatabaseFacade Database { get; }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);


    }
}
