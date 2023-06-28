namespace Orchestration.StockService.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<IOrchestrationOrderCreatedEvent>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<OrderCreatedEventConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer
            (AppDbContext dbContext,
            ILogger<OrderCreatedEventConsumer> logger,
            IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<IOrchestrationOrderCreatedEvent> context)
        {
            bool stockStatus = await CheckStockOfAllProductsAsync(context);
            if (!stockStatus) // ürünlerden herhangi birinin stoğu yoksa
            {
                await _publishEndpoint.Publish<IOrchestrationStockNotReservedEvent>(new OrchestrationStockNotReservedEvent(context.Message.CorrelationId)
                {
                    Reason = "Not enough stock"
                });

                _logger.LogInformation("Not enough stock for CorrelationId Id :{@correlationId}", context.Message.CorrelationId);

                return;
            }

            foreach (var orderItem in context.Message.OrderItems)
            {
                var stock = await _dbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == orderItem.ProductId);

                stock.Count -= orderItem.Count;

                await _dbContext.SaveChangesAsync();
            }

            _logger.LogInformation("Stock was reserved for CorrelationId Id :{@correlationId}", context.Message.CorrelationId);

            OrchestrationStockReservedEvent orchestrationStockReservedEvent = new(context.Message.CorrelationId)
            {
                OrderItems = context.Message.OrderItems
            };

            await _publishEndpoint.Publish<IOrchestrationStockReservedEvent>(orchestrationStockReservedEvent);
        }

        /// <summary>
        /// Tüm ürünlerin stoğunu kontrol eder.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CheckStockOfAllProductsAsync(ConsumeContext<IOrchestrationOrderCreatedEvent> context)
        {
            foreach (var orderItem in context.Message.OrderItems)
            {
                bool isHasStock = await _dbContext.Stocks.AnyAsync(x => x.ProductId == orderItem.ProductId && x.Count > orderItem.Count);
                if (!isHasStock)
                {
                    return isHasStock; //herhangi bir üründe stok yoksa işlemi bitir.
                }
            }

            return true;
        }
    }
}
