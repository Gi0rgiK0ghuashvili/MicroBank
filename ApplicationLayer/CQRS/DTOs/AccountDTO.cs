namespace ApplicationLayer.CQRS.DTOs
{
    public class AccountDTO
    {
        // BaseEntity
        public Guid? Id { get; set; }
        public bool? Active { get; set; }

        public string? CreatedBy { get; set; }
        public string? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? CreatedDate { get; set; }

        // Account
        public string? UserName { get; set; }
        public string? Password { get; set; }

        public Guid? CustomerId { get; set; }

    }
}
