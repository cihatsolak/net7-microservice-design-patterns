namespace Choreography.OrderService.API.Consumers
{
    public class StockNotReservedEventConsumer : IConsumer<StockNotReservedEvent>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PaymentCompletedEventConsumer> _logger;

        public StockNotReservedEventConsumer(
            AppDbContext context,
            ILogger<PaymentCompletedEventConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            var order = await _context.Orders.FindAsync(context.Message.OrderId);
            if (order is not null)
            {
                order.Status = OrderStatus.Fail;
                order.FailMessage = context.Message.Message;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Order (Id={@orderId}) status changed : {@orderStatus}", context.Message.OrderId, order.Status);
            }
            else
            {
                _logger.LogError("Order (Id={@orderId}) not found.", context.Message.OrderId);
            }
        }
    }
}
