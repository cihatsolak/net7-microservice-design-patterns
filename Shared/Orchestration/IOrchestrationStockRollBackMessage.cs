namespace Shared.Orchestration
{
    public interface IOrchestrationStockRollBackMessage
    {
        List<OrderItemMessage> OrderItems { get; set; }
    }

    public class StockRollbackMessage : IOrchestrationStockRollBackMessage
    {
        public StockRollbackMessage(List<OrderItemMessage> orderItems)
        {
            OrderItems = orderItems;
        }

        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
