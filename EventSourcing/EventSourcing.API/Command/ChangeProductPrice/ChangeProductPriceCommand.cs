namespace EventSourcing.API.Command.ChangeProductPrice
{
    public class ChangeProductPriceCommand : IRequest
    {
        public ChangeProductPriceDto ChangeProductPriceDto { get; set; }

        public ChangeProductPriceCommand()
        {

        }
        public ChangeProductPriceCommand(ChangeProductPriceDto changeProductPriceDto)
        {
            ChangeProductPriceDto = changeProductPriceDto;
        }
    }
}
