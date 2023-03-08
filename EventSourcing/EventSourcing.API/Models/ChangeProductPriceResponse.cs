namespace EventSourcing.API.Models
{
    public class ChangeProductPriceResponse
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }
    }
}
