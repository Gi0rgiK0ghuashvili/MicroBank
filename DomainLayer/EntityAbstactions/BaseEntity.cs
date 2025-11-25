namespace DomainLayer.EntityAbstactions
{
    public abstract class BaseEntity
    {
        
        public Guid Id { get; set; }
        public bool Active { get; set; }

        public string? CreatedBy { get; set; }
        public string? UpdateBy { get; set; }

        public DateTime? UpdateDate { get; set; }
        public DateTime? CreatedDate { get; set; }

    }
}
