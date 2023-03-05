namespace Shared.Orchestration
{
    public interface IOrchestrationPaymentCompletedEvent : CorrelatedBy<Guid>
    {
    }

    public class OrchestrationPaymentCompletedEvent : IOrchestrationPaymentCompletedEvent
    {
        public OrchestrationPaymentCompletedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public Guid CorrelationId { get; }
    }
}
