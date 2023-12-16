namespace EventSourcing.API.BackgroundServices;

public class ProductReadDatabaseEventStore : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ProductReadDatabaseEventStore> _logger;

    private EventStorePersistentSubscriptionsClient _eventStorePersistentSubscriptionsClient;

    private const int PSTREAM_BUFFER_SIZE = 10;
    private static readonly UserCredentials PSTREAM_USER_CREDENTIALS = new(username: "admin", password: "changeit");

    public ProductReadDatabaseEventStore(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<ProductReadDatabaseEventStore> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var eventStoreClientSettings = EventStoreClientSettings.Create(_configuration.GetConnectionString("EventStore"));
        _eventStorePersistentSubscriptionsClient = new EventStorePersistentSubscriptionsClient(eventStoreClientSettings);

        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConnectToSubscriptionAsync();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _eventStorePersistentSubscriptionsClient.DeleteToStreamAsync(
            ProductStream.StreamName,
            ProductStream.GroupName,
            deadline: null,
            PSTREAM_USER_CREDENTIALS,
            cancellationToken);
    }

    private async Task<PersistentSubscription> ConnectToSubscriptionAsync()
    {
        var persistentSubscription = await _eventStorePersistentSubscriptionsClient.SubscribeToStreamAsync(
            ProductStream.StreamName,
            ProductStream.GroupName,
            EventAppearedAsync,
            EventSubscriptionDropped,
            PSTREAM_USER_CREDENTIALS,
            PSTREAM_BUFFER_SIZE //bir istemcinin aynı anda ele alabileceği, işlemekte olduğu ancak henüz tamamlanmamış olan olay sayısını belirtir.
            );

        //await _eventStoreConnection.ConnectToPersistentSubscriptionAsync(ProductStream.StreamName, ProductStream.GroupName, EventAppeared, autoAck: false);

        return persistentSubscription;
    }

    private async Task EventAppearedAsync(PersistentSubscription persistentSubscription, ResolvedEvent resolvedEvent, int? retryCount, CancellationToken cancellationToken)
    {
        //Metadata bilgisi ayrı bir class library'de olduğu için virgül ile konumunu belirtiyorum.
        //Tipi belirleyerek ProductCreatedEvent mı? ProductNameChangedEvent mi? olduğunu anlıyorum.
        Type type = Type.GetType($"{Encoding.UTF8.GetString(resolvedEvent.Event.Metadata.Span)}, EventSourcing.Shared");

        _logger.LogInformation("The Message processing... : {@type}", type);

        string eventData = Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span);

        object @event = JsonSerializer.Deserialize(eventData, type);

        using var serviceScope = _serviceProvider.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

        Product product;

        switch (@event)
        {
            case ProductCreatedEvent productCreatedEvent:

                if (await context.Products.AnyAsync(product => product.Id == productCreatedEvent.Id, cancellationToken))
                {
                    break;
                }

                product = new Product()
                {
                    Name = productCreatedEvent.Name,
                    Id = productCreatedEvent.Id,
                    Price = productCreatedEvent.Price,
                    Stock = productCreatedEvent.Stock,
                    UserId = productCreatedEvent.UserId
                };
                context.Products.Add(product);
                break;

            case ProductNameChangedEvent productNameChangedEvent:

                product = await context.Products.FindAsync(new object[] { productNameChangedEvent.Id }, cancellationToken);
                if (product is not null)
                {
                    product.Name = productNameChangedEvent.ChangedName;
                }
                break;

            case ProductPriceChangedEvent productPriceChangedEvent:

                product = await context.Products.FindAsync(new object[] { productPriceChangedEvent.Id }, cancellationToken);
                if (product is not null)
                {
                    product.Price = productPriceChangedEvent.ChangedPrice;
                }
                break;

            case ProductDeletedEvent productDeletedEvent:

                product = await context.Products.FindAsync(new object[] { productDeletedEvent.Id }, cancellationToken);
                if (product is not null)
                {
                    context.Products.Remove(product);
                }
                break;
        }

        await context.SaveChangesAsync(cancellationToken);

        await persistentSubscription.Ack(resolvedEvent);
        //autoAck: true  --> event store message gönderdiğinde, direkt olarak mesajı gönderilmiş sayar. doğru/yanlış işlendiğiyle ilgilenmez.
        //                   EventAppeared metodu doğru çalışırsa, herhangi bir hata fırlatılmazsa mesaj gönderilmiş der. eğer metotda hata fırlatılırsa mesajı bir sonraki aşamada tekrar gönderir.
        //autoAck: false --> Event'i bana gönder, ben sana ne zaman bilgi gönderirsem o eventi gönderilmiş say.
    }

    private void EventSubscriptionDropped(PersistentSubscription persistentSubscription, SubscriptionDroppedReason subscriptionDroppedReason, Exception? exception)
    {
        _logger.LogError(exception, "SubscriptionId: {SubscriptionId}, Reason: {subscriptionDroppedReason}", persistentSubscription.SubscriptionId, subscriptionDroppedReason);
    }
}
