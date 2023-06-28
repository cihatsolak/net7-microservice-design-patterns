namespace Choreography.OrderService.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<PaymentCompletedEventConsumer> _logger;

        public PaymentFailedEventConsumer(
            AppDbContext dbContext,
            ILogger<PaymentCompletedEventConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var order = await _dbContext.Orders.FindAsync(context.Message.OrderId);
            if (order is not null)
            {
                order.Status = OrderStatus.Fail;
                order.FailMessage = context.Message.Message;
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Order (Id={@orderId}) status changed : {@orderStatus}", context.Message.OrderId, order.Status);
            }
            else
            {
                _logger.LogError("Order (Id={@orderId}) not found.", context.Message.OrderId);
            }
        }
    }
}
