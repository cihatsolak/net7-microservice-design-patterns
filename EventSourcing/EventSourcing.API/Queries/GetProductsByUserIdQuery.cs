namespace EventSourcing.API.Queries
{
    public class GetProductsByUserIdQuery : IRequest<List<ProductDto>>
    {
        public GetProductsByUserIdQuery(int userId)
        {
            UserId = userId;
        }

        public int UserId { get; set; }
    }
}
