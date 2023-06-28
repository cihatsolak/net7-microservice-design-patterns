namespace Orchestration.OrderService.API.Consumers
{
    public class OrderRequestCompletedEventConsumer : IConsumer<IOrchestrationOrderRequestCompletedEvent>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<OrderRequestCompletedEventConsumer> _logger;

        public OrderRequestCompletedEventConsumer(AppDbContext dbContext, ILogger<OrderRequestCompletedEventConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IOrchestrationOrderRequestCompletedEvent> context)
        {
            var order = await _dbContext.Orders.FindAsync(context.Message.OrderId);
            if (order is not null)
            {
                order.Status = OrderStatus.Completed;
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Order (Id={context.Message.OrderId}) status changed : {order.Status}");
            }
            else
            {
                _logger.LogError($"Order (Id={context.Message.OrderId}) not found");
            }
        }
    }
}
