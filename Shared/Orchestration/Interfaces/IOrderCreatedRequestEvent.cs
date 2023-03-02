namespace Shared.Orchestration.Interfaces
{
    public interface IOrderCreatedRequestEvent
    {
        public int OrderId { get; set; }
        public string BuyerId { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }

        public PaymentMessage Payment { get; set; }
    }
}
