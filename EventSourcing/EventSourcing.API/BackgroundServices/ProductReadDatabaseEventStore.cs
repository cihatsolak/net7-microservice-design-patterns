namespace EventSourcing.API.BackgroundServices
{
    public class ProductReadDatabaseEventStore : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly ILogger<ProductReadDatabaseEventStore> _logger;

        public ProductReadDatabaseEventStore(
            IServiceProvider serviceProvider,
            IEventStoreConnection eventStoreConnection,
            ILogger<ProductReadDatabaseEventStore> logger)
        {
            _eventStoreConnection = eventStoreConnection;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //autoAck: true  --> event store message gönderdiğinde, direkt olarak mesajı gönderilmiş sayar. doğru/yanlış işlendiğiyle ilgilenmez.
            //                   EventAppeared metodu doğru çalışırsa, herhangi bir hata fırlatılmazsa mesaj gönderilmiş der. eğer metotda hata fırlatılırsa mesajı bir sonraki aşamada tekrar gönderir.
            //autoAck: false --> Event'i bana gönder, ben sana ne zaman bilgi gönderirsem o eventi gönderilmiş say.
            await _eventStoreConnection.ConnectToPersistentSubscriptionAsync(ProductStream.StreamName, ProductStream.GroupName, EventAppeared, autoAck: false);
        }

        private async Task EventAppeared(EventStorePersistentSubscriptionBase eventStore, ResolvedEvent resolvedEvent)
        {
            //Metadata bilgisi ayrı bir class library'de olduğu için virgül ile konumunu belirtiyorum.
            //Tipi belirleyerek ProductCreatedEvent mı? ProductNameChangedEvent mi? olduğunu anlıyorum.
            Type type = Type.GetType($"{Encoding.UTF8.GetString(resolvedEvent.Event.Metadata)}, EventSourcing.Shared");

            _logger.LogInformation("The Message processing... : {@type}", type);

            string eventData = Encoding.UTF8.GetString(resolvedEvent.Event.Data);

            object @event = JsonSerializer.Deserialize(eventData, type);

            using var serviceScope = _serviceProvider.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

            Product product;

            switch (@event)
            {
                case ProductCreatedEvent productCreatedEvent:

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

                    product = await context.Products.FindAsync(productNameChangedEvent.Id);
                    if (product is not null)
                    {
                        product.Name = productNameChangedEvent.ChangedName;
                    }
                    break;

                case ProductPriceChangedEvent productPriceChangedEvent:

                    product = await context.Products.FindAsync(productPriceChangedEvent.Id);
                    if (product is not null)
                    {
                        product.Price = productPriceChangedEvent.ChangedPrice;
                    }
                    break;

                case ProductDeletedEvent productDeletedEvent:

                    product = await context.Products.FindAsync(productDeletedEvent.Id);
                    if (product is not null)
                    {
                        context.Products.Remove(product);
                    }
                    break;
            }

            await context.SaveChangesAsync();

            eventStore.Acknowledge(resolvedEvent.Event.EventId);
        }
    }
}
