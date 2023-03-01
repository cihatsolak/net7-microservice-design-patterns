namespace Shared.Infrastructure
{
    public static class RabbitQueueName
    {
        public const string StockReservedEventQueueName = "stock-reserved-queue";
        public const string StockOrderCreatedEventQueueName = "stock-order-created-queue";
        public const string StockPaymentFailedEventQueueName = "stock-payment-failed-queue";
        public const string OrderPaymentCompletedEventQueueName = "order-payment-completed-queue";
        public const string OrderPaymentFailedEventQueueName = "order-payment-failed-queue";
        public const string OrderStockNotReservedEventQueueName = "order-stock-not-reserved-queue";
    }
}
