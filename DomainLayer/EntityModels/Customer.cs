using DomainLayer.EntityAbstactions;

namespace DomainLayer.EntityModels
{
    public class Customer : BaseEntity
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public decimal Balance { get; set; }

        public int? AccountId { get; set; }
        public Account? Account { get; set; }
    }
}
