namespace EventSourcing.API.Store
{
    public abstract class BaseStream
    {
        protected readonly LinkedList<IEvent> Events = new();

        private readonly string _streamName;
        private readonly IEventStoreConnection _eventStoreConnection;

        protected BaseStream(string streamName, IEventStoreConnection eventStoreConnection)
        {
            _streamName = streamName;
            _eventStoreConnection = eventStoreConnection;
        }

        public async Task SaveAsync()
        {
            if (!Events.Any())
            {
                await Task.CompletedTask;
                return;
            }

            List<EventData> eventDataList = new();

            foreach (var item in Events)
            {
                Guid eventId = Guid.NewGuid();
                string eventType = item.GetType().Name;
                bool isJson = true;
                byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(item, inputType: item.GetType()));
                byte[] metaData = Encoding.UTF8.GetBytes(item.GetType().FullName);

                eventDataList.Add(new EventData(eventId, eventType, isJson, data, metaData));
            }

            await _eventStoreConnection.AppendToStreamAsync(_streamName, ExpectedVersion.Any, eventDataList);

            Events.Clear();
        }
    }
}
