namespace Shared.Orchestration
{
    public interface IOrchestrationPaymentFailedEvent : CorrelatedBy<Guid>
    {
        public string Reason { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
    }

    public class OrchestrationPaymentFailedEvent : IOrchestrationPaymentFailedEvent
    {
        public OrchestrationPaymentFailedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public string Reason { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }

        public Guid CorrelationId { get; }
    }
}
