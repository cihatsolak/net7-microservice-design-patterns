namespace Orchestration.PaymentService.API.Consumers
{
    public class StockReservedRequestPaymentConsumer : IConsumer<IOrchestrationStockReservedRequestPayment>
    {
        private readonly ILogger<StockReservedRequestPaymentConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public StockReservedRequestPaymentConsumer(
            ILogger<StockReservedRequestPaymentConsumer> logger, 
            IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<IOrchestrationStockReservedRequestPayment> context)
        {
            var balance = 3000m;

            if (balance > context.Message.Payment.TotalPrice)
            {
                _logger.LogInformation("{@totalPrice} TL was withdrawn from credit card for user id={@buyerId}", context.Message.Payment.TotalPrice, context.Message.BuyerId);

                await _publishEndpoint.Publish(new OrchestrationPaymentCompletedEvent(context.Message.CorrelationId));
            }
            else
            {
                _logger.LogInformation("{@totalPrice} TL was not withdrawn from credit card for user id = {@buyerId}", context.Message.Payment.TotalPrice, context.Message.BuyerId);

                await _publishEndpoint.Publish(new OrchestrationPaymentFailedEvent(context.Message.CorrelationId) { Reason = "not enough balance", OrderItems = context.Message.OrderItems });
            }
        }
    }
}
