namespace Shared.Orchestration
{
    public interface IOrchestrationStockReservedEvent : CorrelatedBy<Guid>
    {
        List<OrderItemMessage> OrderItems { get; set; }
    }

    public class OrchestrationStockReservedEvent : IOrchestrationStockReservedEvent
    {
        public OrchestrationStockReservedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public List<OrderItemMessage> OrderItems { get; set; }
        public Guid CorrelationId { get; }
    }
}
