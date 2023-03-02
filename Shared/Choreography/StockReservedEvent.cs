namespace Shared.Choreography
{
    /// <summary>
    /// Stok işlemi başarılı olduğunda
    /// </summary>
    public class StockReservedEvent
    {
        public StockReservedEvent()
        {
            OrderItems = new List<OrderItemMessage>();
        }
        public int OrderId { get; set; }
        public string BuyerId { get; set; }
        public PaymentMessage Payment { get; set; }

        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
