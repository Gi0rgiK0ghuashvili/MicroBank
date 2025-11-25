using DomainLayer.EntityAbstactions;
using System.ComponentModel.DataAnnotations;

namespace DomainLayer.EntityModels
{
    public class Customer : BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public decimal Balance { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public Guid? AccountId { get; set; }
        public Account? Account { get; set; }

        public ICollection<Transaction>? SentTransactions { get; set; }

        public ICollection<Transaction>? ReceivedTransactions { get; set; }
    }
}
