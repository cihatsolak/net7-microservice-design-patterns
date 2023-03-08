namespace EventSourcing.API.Command
{
    public class ChangeProducNameCommand : IRequest
    {
        public ChangeProductNameDto ChangeProductNameDto { get; set; }
    }
}
