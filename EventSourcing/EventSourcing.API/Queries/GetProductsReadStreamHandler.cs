namespace EventSourcing.API.Queries;

public record GetProductsReadStream : IRequest<List<IStoredEvent>>
{
    public string StreamName { get; set; }

    public GetProductsReadStream(string streamName)
    {
        StreamName = streamName;
    }
}

public class GetProductsReadStreamHandler : IRequestHandler<GetProductsReadStream, List<IStoredEvent>>
{
    private readonly EventStoreClient _eventStoreClient;

    public GetProductsReadStreamHandler(EventStoreClient eventStoreClient)
    {
        _eventStoreClient = eventStoreClient;
    }

    public async Task<List<IStoredEvent>> Handle(GetProductsReadStream request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.StreamName))
            return default;

        List<IStoredEvent> storedEvents = new();

        var streamResult = _eventStoreClient.ReadStreamAsync(Direction.Forwards, request.StreamName, StreamPosition.Start, cancellationToken: cancellationToken);

        Type[] currentTypes = typeof(IStoredEvent).Assembly.GetTypes();

        await foreach(var item in streamResult)
        {
            Type type = currentTypes.FirstOrDefault(type => type.Name.Equals(item.Event.EventType, StringComparison.OrdinalIgnoreCase));
            if (type is null)
            {
                continue;
            }

            IStoredEvent storedEvent = JsonSerializer.Deserialize(item.Event.Data.Span, type) as IStoredEvent;

            storedEvents.Add(storedEvent);
        }

        return storedEvents;
    }
}
