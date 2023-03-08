namespace EventSourcing.API.Command.ChangeProductName
{
    public class ChangeProductNameCommand : IRequest
    {
        public ChangeProductNameDto ChangeProductNameDto { get; set; }

        public ChangeProductNameCommand()
        {

        }
        public ChangeProductNameCommand(ChangeProductNameDto changeProductNameDto)
        {
            ChangeProductNameDto = changeProductNameDto;
        }
    }
}
