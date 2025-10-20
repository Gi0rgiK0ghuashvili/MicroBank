using DomainLayer.EntityAbstactions;

namespace DomainLayer.EntityModels
{
    public class Transaction : BaseEntity
    {
        public Guid SenderId { get; set; }
        public Customer Sender { get; set; }

        public Guid RecipientId { get; set; }
        public Customer Recipient { get; set; }

        public decimal TransferredAmount { get; set; }
    }
}
