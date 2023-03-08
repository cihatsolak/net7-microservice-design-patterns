namespace EventSourcing.API.Command
{
    public class ChangeProductPriceCommand : IRequest
    {
        public ChangeProductPriceDto ChangeProductPriceDto { get; set; }
    }
}
