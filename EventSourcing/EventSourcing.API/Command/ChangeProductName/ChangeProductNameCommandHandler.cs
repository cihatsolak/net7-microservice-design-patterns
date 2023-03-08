namespace EventSourcing.API.Command.ChangeProductName
{
    public class ChangeProductNameCommandHandler : IRequestHandler<ChangeProductNameCommand>
    {
        public Task Handle(ChangeProductNameCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
