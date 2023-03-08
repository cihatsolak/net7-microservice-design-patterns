namespace EventSourcing.API.Models
{
    public class CreateProductDto
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
    }
}
