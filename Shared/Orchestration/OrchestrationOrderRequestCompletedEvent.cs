namespace Shared.Orchestration
{
    public interface IOrchestrationOrderRequestCompletedEvent
    {
        public int OrderId { get; set; }
    }

    public class OrchestrationOrderRequestCompletedEvent : IOrchestrationOrderRequestCompletedEvent
    {
        public OrchestrationOrderRequestCompletedEvent(int orderId)
        {
            OrderId = orderId;
        }

        public int OrderId { get; set; }
    }
}
