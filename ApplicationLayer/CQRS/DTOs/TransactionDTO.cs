namespace ApplicationLayer.CQRS.DTOs
{
    public class TransactionDTO
    {
        // BaseEntity
        public Guid? Id { get; set; }
        public bool? Active { get; set; }

        public string? CreatedBy { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? CreatedDate { get; set; }

        // Transaction
        public Guid? SenderId { get; set; }
        public Guid? RecipientId { get; set; }
        public decimal? TransferredAmount { get; set; }

    }
}
