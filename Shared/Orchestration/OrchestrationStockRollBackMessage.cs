namespace Shared.Orchestration
{
    public interface IOrchestrationStockRollBackMessage
    {
        List<OrderItemMessage> OrderItems { get; set; }
    }

    public class OrchestrationStockRollBackMessage : IOrchestrationStockRollBackMessage
    {
        public OrchestrationStockRollBackMessage(List<OrderItemMessage> orderItems)
        {
            OrderItems = orderItems;
        }

        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
