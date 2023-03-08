namespace EventSourcing.API.Command
{
    public class DeleteProductCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
