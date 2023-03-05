namespace Shared.Orchestration
{
    public interface IOrchestrationOrderRequestFailedEvent
    {
        public int OrderId { get; set; }
        public string Reason { get; set; }
    }

    public class OrchestrationOrderRequestFailedEvent : IOrchestrationOrderRequestFailedEvent
    {
        public OrchestrationOrderRequestFailedEvent(int orderId, string reason)
        {
            OrderId = orderId;
            Reason = reason;
        }

        public int OrderId { get; set; }
        public string Reason { get; set; }
    }
}
