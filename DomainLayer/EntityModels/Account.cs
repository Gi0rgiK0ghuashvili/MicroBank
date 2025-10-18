using DomainLayer.EntityAbstactions;

namespace DomainLayer.EntityModels
{
    public class Account : BaseEntity
    {
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public int TransactionId { get; set; }
        public Transaction? Transaction { get; set; }
    }
}
