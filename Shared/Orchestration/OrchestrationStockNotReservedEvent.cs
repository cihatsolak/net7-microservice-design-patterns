namespace Shared.Orchestration
{
    public interface IOrchestrationStockNotReservedEvent : CorrelatedBy<Guid>
    {
        string Reason { get; set; }
    }

    public class OrchestrationStockNotReservedEvent : IOrchestrationStockNotReservedEvent
    {
        public OrchestrationStockNotReservedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public string Reason { get; set; }

        public Guid CorrelationId { get; }
    }
}
