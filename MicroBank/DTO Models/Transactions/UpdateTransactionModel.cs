namespace MicroBank.DTO_Models.Transactions
{
    public class UpdateTransactionModel
    {
        // BaseEntity
        public Guid? Id { get; set; }
        public string? UpdateBy { get; set; }

        // Transaction
        public Guid? SenderId { get; set; }
        public Guid? RecipientId { get; set; }
        public decimal? TransferredAmount { get; set; }
    }
}
