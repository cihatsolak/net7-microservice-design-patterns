namespace Choreography.StockService.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<PaymentFailedEventConsumer> _logger;

        public PaymentFailedEventConsumer(
            AppDbContext dbContext,
            ILogger<PaymentFailedEventConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            foreach (var item in context.Message.OrderItems)
            {
                var stock = await _dbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);
                if (stock is not null)
                {
                    stock.Count += item.Count;
                    await _dbContext.SaveChangesAsync();
                }
            }

            _logger.LogInformation("Stock was released for Order Id ({@orderId})", context.Message.OrderId);
        }
    }
}
