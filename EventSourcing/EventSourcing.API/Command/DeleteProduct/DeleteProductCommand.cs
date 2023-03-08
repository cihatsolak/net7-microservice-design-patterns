namespace EventSourcing.API.Command.DeleteProduct
{
    public class DeleteProductCommand : IRequest
    {
        public Guid Id { get; set; }

        public DeleteProductCommand()
        {
        }

        public DeleteProductCommand(Guid ıd)
        {
            Id = ıd;
        }
    }
}
