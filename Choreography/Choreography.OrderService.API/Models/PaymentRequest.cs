namespace Choreography.OrderService.API.Models
{
    public class PaymentRequest
    {
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string Expiration { get; set; }
        public string CVV { get; set; }
    }
}
