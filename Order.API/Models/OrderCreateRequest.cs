namespace OrderService.API.Models
{
    public class OrderCreateRequest
    {
        public string BuyerId { get; set; }
        public List<OrderItemRequest> OrderItems { get; set; }
        public PaymentRequest Payment { get; set; }
        public AddressRequest Address { get; set; }
    }
}
