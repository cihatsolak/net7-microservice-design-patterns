namespace Orchestration.OrderService.API.Models
{
    public class OrderItemRequest
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
