namespace MicroBank.DTO_Models.Customers
{
    public class UpdateCustomerModel
    {
        public Guid? Id { get; set; }

        public string? UpdateBy { get; set; }

        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public decimal? Balance { get; set; }

        public Guid? AccountId { get; set; }
    }
}
