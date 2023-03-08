namespace EventSourcing.API.Store
{
    public static class EventStoreContainerBuilderExtensions
    {
        public static async Task AddEventStoreAsync(this IServiceCollection services, IConfiguration configuration)
        {
            IEventStoreConnection eventStoreConnection = EventStoreConnection.Create(connectionString: configuration.GetConnectionString("EventStore"));

            await eventStoreConnection.ConnectAsync();

            services.AddSingleton(eventStoreConnection);

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddConsole();
            });

            var logger = loggerFactory.CreateLogger(nameof(EventStoreContainerBuilderExtensions));

            eventStoreConnection.Connected += (sender, args) =>
            {
                logger.LogInformation("EventStore connection established.");
            };

            eventStoreConnection.ErrorOccurred += (sender, args) =>
            {
                logger.LogError("{@exceptionMessage}", args.Exception.Message);
            };

            eventStoreConnection.Disconnected += (sender, args) =>
            {
                logger.LogError("Event store connection is broken.");
            };
        }
    }
}
