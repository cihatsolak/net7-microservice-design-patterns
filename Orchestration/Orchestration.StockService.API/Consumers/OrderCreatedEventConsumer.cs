namespace Orchestration.StockService.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<IOrchestrationOrderCreatedEvent>
    {
        public Task Consume(ConsumeContext<IOrchestrationOrderCreatedEvent> context)
        {
            throw new NotImplementedException();
        }
    }
}
