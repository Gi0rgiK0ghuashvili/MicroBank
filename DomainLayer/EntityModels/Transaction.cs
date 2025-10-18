using DomainLayer.EntityAbstactions;

namespace DomainLayer.EntityModels
{
    public class Transaction : BaseEntity
    {
        public int SenderId { get; set; }
        public Account Sender { get; set; }

        public int RecipientId { get; set; }
        public Account Recipient { get; set; }

        public decimal TransferredAmount { get; set; }
    }
}
