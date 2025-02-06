namespace EventTicketingManagementSystem.Models.BaseModels
{
    public abstract class EntityAuditBase<T> : EntityBase<T>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
