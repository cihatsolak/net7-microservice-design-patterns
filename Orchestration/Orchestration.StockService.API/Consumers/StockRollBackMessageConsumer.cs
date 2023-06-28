namespace Orchestration.StockService.API.Consumers
{
    public class StockRollBackMessageConsumer : IConsumer<IOrchestrationStockRollBackMessage>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<StockRollBackMessageConsumer> _logger;

        public StockRollBackMessageConsumer(AppDbContext dbContext, ILogger<StockRollBackMessageConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IOrchestrationStockRollBackMessage> context)
        {
            foreach (var item in context.Message.OrderItems)
            {
                var stock = await _dbContext.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                if (stock != null)
                {
                    stock.Count += item.Count;
                    await _dbContext.SaveChangesAsync();
                }
            }

            _logger.LogInformation("Stock was released");
        }
    }
}
