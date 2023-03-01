namespace Shared
{
    /// <summary>
    /// Stok işlemi başarısız olduğunda
    /// </summary>
    public class StockNotReservedEvent
    {
        public int OrderId { get; set; }
        public string Message { get; set; }
    }
}
