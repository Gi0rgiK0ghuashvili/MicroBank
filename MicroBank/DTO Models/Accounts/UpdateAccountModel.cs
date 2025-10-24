namespace MicroBank.DTO_Models.Accounts
{
    public class UpdateAccountModel
    {
        public Guid? Id { get; set; }
        public bool? Active { get; set; }

        public string? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }

        // Account
        public string? UserName { get; set; }
        public string? Password { get; set; }

        public Guid? CustomerId { get; set; }

    }
}
