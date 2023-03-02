using Shared.Infrastructure;

namespace Choreography.StockService.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OrderCreatedEventConsumer> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer
            (AppDbContext context,
            ILogger<OrderCreatedEventConsumer> logger,
            ISendEndpointProvider sendEndpointProvider,
            IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _logger = logger;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            bool stockStatus = await CheckStockOfAllProductsAsync(context);
            if (!stockStatus) // ürünlerden herhangi birinin stoğu yoksa
            {
                StockNotReservedEvent stockNotReservedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    Message = "Not enough stock"
                };

                await _publishEndpoint.Publish(stockNotReservedEvent);

                _logger.LogInformation("Not enough stock for Buyer Id : {@buyerId}", context.Message.BuyerId);

                return;
            }

            foreach (var orderItem in context.Message.OrderItems)
            {
                var stock = await _context.Stocks.FirstOrDefaultAsync(x => x.ProductId == orderItem.ProductId);

                stock.Count -= orderItem.Count;

                await _context.SaveChangesAsync();
            }

            _logger.LogInformation("Stock was reserved for buyer Id :{@buyerId}", context.Message.BuyerId);

            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitQueueName.StockReservedEventQueueName}"));

            StockReservedEvent stockReservedEvent = new()
            {
                Payment = context.Message.Payment,
                BuyerId = context.Message.BuyerId,
                OrderId = context.Message.OrderId,
                OrderItems = context.Message.OrderItems
            };

            await sendEndpoint.Send(stockReservedEvent);
        }

        /// <summary>
        /// Tüm ürünlerin stoğunu kontrol eder.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> CheckStockOfAllProductsAsync(ConsumeContext<OrderCreatedEvent> context)
        {
            foreach (var orderItem in context.Message.OrderItems)
            {
                bool isHasStock = await _context.Stocks.AnyAsync(x => x.ProductId == orderItem.ProductId && x.Count > orderItem.Count);
                if (!isHasStock)
                {
                    return isHasStock; //herhangi bir üründe stok yoksa işlemi bitir.
                }
            }

            return true;
        }
    }
}
