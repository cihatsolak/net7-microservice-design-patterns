namespace EventSourcing.Shared.Events
{
    public class ProductDeletedEvent : IStoredEvent
    {
        public Guid Id { get; set; }
    }
}
