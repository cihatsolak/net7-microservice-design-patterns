namespace Shared.Orchestration
{
    public interface IOrchestrationStockReservedRequestPayment : CorrelatedBy<Guid>
    {
        public PaymentMessage Payment { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }

        public string BuyerId { get; set; }
    }

    public class OrchestrationStockReservedRequestPayment : IOrchestrationStockReservedRequestPayment
    {
        public OrchestrationStockReservedRequestPayment(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public PaymentMessage Payment { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }

        public Guid CorrelationId { get; }
        public string BuyerId { get; set; }
    }
}
