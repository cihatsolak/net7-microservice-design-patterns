namespace EventSourcing.Shared.Events
{
    public class ProductPriceChangedEvent : IStoredEvent
    {
        public Guid Id { get; set; }
        public decimal ChangedPrice { get; set; }
    }
}
