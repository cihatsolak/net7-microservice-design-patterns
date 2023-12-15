namespace EventSourcing.API.Queries;

public class GetProductsByUserIdQuery : IRequest<List<ProductDto>>
{
    public GetProductsByUserIdQuery(int userId)
    {
        UserId = userId;
    }

    public int UserId { get; set; }
}

public class GetProductsByUserIdQueryHandler : IRequestHandler<GetProductsByUserIdQuery, List<ProductDto>>
{
    private readonly AppDbContext _context;

    public GetProductsByUserIdQueryHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> Handle(GetProductsByUserIdQuery request, CancellationToken cancellationToken)
    {   
        var products = await _context.Products
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
