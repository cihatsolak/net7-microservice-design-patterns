namespace EventSourcing.API.Store;

public abstract class BaseStream
{
    protected readonly LinkedList<IStoredEvent> Events = new();

    private readonly string _streamName;
    private readonly EventStoreClient _eventStoreClient;

    protected BaseStream(string streamName, EventStoreClient eventStoreClient)
    {
        _streamName = streamName;
        _eventStoreClient = eventStoreClient;
    }

    public async Task SaveAsync()
    {
        if (!Events.Any())
            return;

        List<EventData> eventDataList = new();

        foreach (var item in Events)
        {
            Uuid eventId = Uuid.NewUuid();
            string eventType = item.GetType().Name;
            byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(item, inputType: item.GetType()));
            byte[] metaData = Encoding.UTF8.GetBytes(item.GetType().FullName);

            eventDataList.Add(new EventData(eventId, eventType, data, metaData));
        }

        await _eventStoreClient.AppendToStreamAsync(_streamName, StreamState.Any, eventDataList);

        Events.Clear();
    }
}
