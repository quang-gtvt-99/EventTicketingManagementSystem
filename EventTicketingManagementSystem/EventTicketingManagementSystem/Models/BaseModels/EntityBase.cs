namespace EventTicketingManagementSystem.Models.BaseModels
{
    public abstract class EntityBase<T>
    {
        public T Id { get; set; }
    }
}
