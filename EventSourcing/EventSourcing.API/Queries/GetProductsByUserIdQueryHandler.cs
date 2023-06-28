namespace EventSourcing.API.Queries
{
    public class GetProductsByUserIdQueryHandler : IRequestHandler<GetProductsByUserIdQuery, List<ProductDto>>
    {
        private readonly AppDbContext _dbContext;

        public GetProductsByUserIdQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProductDto>> Handle(GetProductsByUserIdQuery request, CancellationToken cancellationToken)
        {   
            var products = await _dbContext.Products
                .Where(product => product.UserId == request.UserId)
                .Select(product => new ProductDto
                {
                    Id = product.Id,
                    UserId = product.UserId,
                    Name = product.Name,
                    Price = product.Price,
                    Stock = product.Stock
                })
                .ToListAsync(cancellationToken);

            return products;
        }
    }
}
