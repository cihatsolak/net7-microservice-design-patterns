namespace Choreography.OrderService.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrdersController(
            AppDbContext dbContext,
            IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateRequest request)
        {
            var order = await AddOrderAsync(request);

            OrderCreatedEvent orderCreatedEvent = new()
            {
                BuyerId = request.BuyerId,
                OrderId = order.Id,
                Payment = new PaymentMessage
                {
                    CardName = request.Payment.CardName,
                    CardNumber = request.Payment.CardNumber,
                    Expiration = request.Payment.Expiration,
                    CVV = request.Payment.CVV,
                    TotalPrice = request.OrderItems.Sum(x => x.Price * x.Count)
                },
            };

            request.OrderItems.ForEach(item =>
            {
                orderCreatedEvent.OrderItems.Add(new OrderItemMessage
                {
                    Count = item.Count,
                    ProductId = item.ProductId
                });
            });

            await _publishEndpoint.Publish(orderCreatedEvent);

            return Ok();
        }

        private async Task<Order> AddOrderAsync(OrderCreateRequest request)
        {
            Order order = new()
            {
                BuyerId = request.BuyerId,
                Status = OrderStatus.Suspend,
                Address = new Address
                {
                    Line = request.Address.Line,
                    Province = request.Address.Province,
                    District = request.Address.District
                },
                CreatedDate = DateTime.Now
            };

            request.OrderItems.ForEach(requestOrderItem =>
            {
                order.Items.Add(new OrderItem()
                {
                    Price = requestOrderItem.Price,
                    ProductId = requestOrderItem.ProductId,
                    Count = requestOrderItem.Count
                });
            });

            await _dbContext.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            return order;
        }
    }
}
