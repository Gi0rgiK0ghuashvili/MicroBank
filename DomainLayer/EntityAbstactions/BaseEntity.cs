namespace DomainLayer.EntityAbstactions
{
    public abstract class BaseEntity
    {
        
        public int Id { get; set; }
        public bool Active { get; set; }

        public DateTime UpdateDate { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
