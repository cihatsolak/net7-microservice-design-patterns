namespace Shared.Choreography
{
    public class OrderCreatedEvent
    {
        public OrderCreatedEvent()
        {
            OrderItems = new List<OrderItemMessage>();
        }

        public int OrderId { get; set; }
        public string BuyerId { get; set; }

        public PaymentMessage Payment { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
