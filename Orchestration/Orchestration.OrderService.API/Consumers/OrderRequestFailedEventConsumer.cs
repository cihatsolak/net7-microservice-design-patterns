namespace Orchestration.OrderService.API.Consumers
{
    public class OrderRequestFailedEventConsumer : IConsumer<IOrchestrationOrderRequestFailedEvent>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<OrderRequestFailedEventConsumer> _logger;

        public OrderRequestFailedEventConsumer(AppDbContext dbContext, ILogger<OrderRequestFailedEventConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IOrchestrationOrderRequestFailedEvent> context)
        {
            var order = await _dbContext.Orders.FindAsync(context.Message.OrderId);

            if (order != null)
            {
                order.Status = OrderStatus.Fail;
                order.FailMessage = context.Message.Reason;
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
