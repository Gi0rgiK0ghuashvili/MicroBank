namespace MicroBank.DTO_Models.Customers
{
    public class CustomerModel
    {
        public Guid? Id { get; set; }
        public bool? Active { get; set; }

        public string? CreatedBy { get; set; }
        public string? UpdateBy { get; set; }

        public DateTime? UpdateDate { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public decimal? Balance { get; set; }

        public Guid? AccountId { get; set; }

    }
}
