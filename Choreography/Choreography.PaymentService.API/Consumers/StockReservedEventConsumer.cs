namespace PaymentService.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        private readonly ILogger<StockReservedEventConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumer(
            ILogger<StockReservedEventConsumer> logger,
            IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            var balance = 3000m; //simulasyon

            if (balance < context.Message.Payment.TotalPrice)
            {
                _logger.LogInformation("{@totalPrice} TL was not withdrawn from credit card for user id={@buyerId}", context.Message.Payment.TotalPrice, context.Message.BuyerId);

                PaymentFailedEvent paymentFailedEvent = new()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    Message = "Not enough balance",
                    OrderItems = context.Message.OrderItems
                };

                await _publishEndpoint.Publish(paymentFailedEvent);
                return;
            }

            _logger.LogInformation("{@totalPrice} TL was withdrawn from credit card for user id = {@buyerId}", context.Message.Payment.TotalPrice, context.Message.BuyerId);

            PaymentCompletedEvent paymentCompletedEvent = new()
            {
                BuyerId = context.Message.BuyerId,
                OrderId = context.Message.OrderId
            };

            await _publishEndpoint.Publish(paymentCompletedEvent);
        }
    }
}
