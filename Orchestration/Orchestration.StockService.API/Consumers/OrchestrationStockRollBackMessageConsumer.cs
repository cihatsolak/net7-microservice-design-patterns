namespace Orchestration.StockService.API.Consumers
{
    public class OrchestrationStockRollBackMessageConsumer : IConsumer<OrchestrationStockRollBackMessage>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrchestrationStockRollBackMessageConsumer> _logger;

        public OrchestrationStockRollBackMessageConsumer(AppDbContext context, ILogger<OrchestrationStockRollBackMessageConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrchestrationStockRollBackMessage> context)
        {
            foreach (var item in context.Message.OrderItems)
            {
                var stock = await _context.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                if (stock != null)
                {
                    stock.Count += item.Count;
                    await _context.SaveChangesAsync();
                }
            }

            _logger.LogInformation("Stock was released");
        }
    }
}
