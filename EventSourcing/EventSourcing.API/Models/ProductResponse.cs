namespace EventSourcing.API.Models
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
    }
}
