namespace EventSourcing.Shared.Events
{
    public class ProductNameChangedEvent : IStoredEvent
    {
        public Guid Id { get; set; }
        public string ChangedName { get; set; }
    }
}
