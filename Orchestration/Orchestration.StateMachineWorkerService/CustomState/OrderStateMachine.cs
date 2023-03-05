namespace Orchestration.StateMachineWorkerService.CustomState
{
    /// <summary>
    /// Bu sınıf tüm distributed transaction'ı yönettiğim yer
    /// Then: Business kodları yazacağınız yer
    /// </summary>
    public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
    {
        public Event<IOrderCreatedRequestEvent> OrderCreatedRequestEvent { get; set; }
        public Event<IOrchestrationStockReservedEvent> StockReservedEvent { get; set; }
        public Event<IOrchestrationStockNotReservedEvent> StockNotReservedEvent { get; set; }
        public Event<IOrchestrationPaymentCompletedEvent> PaymentCompletedEvent { get; set; }
        public Event<IOrchestrationPaymentFailedEvent> PaymentFailedEvent { get; set; }

        public State OrderCreated { get; set; }
        public State StockReserved { get; set; }
        public State StockNotReserved { get; set; }
        public State PaymentCompleted { get; set; }
        public State PaymentFailed { get; set; }

        public OrderStateMachine()
        {
            InstanceState(p => p.CurrentState); // Initial State, İlk State

            Event(() => OrderCreatedRequestEvent, eventCorrelationConfigurator =>
            {
                //veri tabanı ile event'deki order id'leri karşılaştırıyorum. eğer var ise yeni bir satır oluşturma. eğer yok ise SelectId ile beraber yeni bir guid oluştur.
                //veri tabanındaki order ile event'den gelen order id'yi karşılaştır. Eğer var ise herhangi birşey yapma. Eğer yok ise yeni bir satır oluştur. Oluşturmuş olduğun bu order state instance satırını da correlation id'sine random bir değer ata.
                //aynı order id'ye sahip event fırlatılırsa, veri tabanında yeni bir satır oluşmaz. (aşağıdaki kontrolle beraber)
                eventCorrelationConfigurator.CorrelateBy<int>(database => database.OrderId, @event => @event.Message.OrderId).SelectId(selector => Guid.NewGuid());
            });

            Event(() => StockReservedEvent, eventCorrelationConfigurator =>
            {
                //StockReservedEvent eventi fırlatıldığında hangi correlationId'ye sahip satırın state'ini değiştirecek? burada belirtiyoruz.
                eventCorrelationConfigurator.CorrelateById(context => context.Message.CorrelationId);
            });

            Event(() => StockNotReservedEvent, eventCorrelationConfigurator =>
            {
                //StockNotReservedEvent eventi fırlatıldığında hangi correlationId'ye sahip satırın state'ini değiştirecek? burada belirtiyoruz.
                eventCorrelationConfigurator.CorrelateById(context => context.Message.CorrelationId);
            });

            Event(() => PaymentCompletedEvent, eventCorrelationConfigurator =>
            {
                //PaymentCompletedEvent eventi fırlatıldığında hangi correlationId'ye sahip satırın state'ini değiştirecek? burada belirtiyoruz.
                eventCorrelationConfigurator.CorrelateById(context => context.Message.CorrelationId);
            });

            //Initial evresinden order created evresine geçerken bunu açıkca belirtmek gereklidir.
            //Initial aşamasındayken, eğerki OrderCreatedRequestEvent event'i geldiyse, Then metoduyla birlikte şunu yap demek.
            Initially(
                 When(OrderCreatedRequestEvent)
                .Then(context =>
                {
                    // context.Saga: veri tabanına kaydedilecek olan satırı temsil eder.
                    // context.Message: Event'den gelen datayı temsil eder.

                    context.Saga.BuyerId = context.Message.BuyerId;

                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.CreatedDate = DateTime.Now;

                    context.Saga.CardName = context.Message.Payment.CardName;
                    context.Saga.CardNumber = context.Message.Payment.CardNumber;
                    context.Saga.CVV = context.Message.Payment.CVV;
                    context.Saga.Expiration = context.Message.Payment.Expiration;
                    context.Saga.TotalPrice = context.Message.Payment.TotalPrice;
                })
                .Then(context => //state'i değiştirmeden Then blogu ile ikinci kez business çalıştırabilirim
                {
                    Console.WriteLine($"OrderCreatedRequestEvent before : {context.Saga}");
                })
                .Publish(context => new OrchestrationOrderCreatedEvent(context.CorrelationId.Value)
                {
                    OrderItems = context.Message.OrderItems
                }) // Publish ya da Send kullanılabilir.
                .TransitionTo(OrderCreated) //yukarıdaki Then blokları (business kodlar) işlendikten sonra inital state'inde order created evresine geçmesi için
                .Then(context =>
                {
                    Console.WriteLine($"OrderCreatedRequestEvent After : {context.Saga}");
                })
            );

            //OrderCreated state'indeyken, OrderCreated evresindeyken
            During(OrderCreated,
                //1.Seçenek
                When(StockReservedEvent)
                    .Send(new Uri($"queue:{RabbitQueueName.PaymentStockReservedRequestQueueName}"), context => new OrchestrationStockReservedRequestPayment(context.Message.CorrelationId)
                    {
                        //context.Message: StockReservedEvent'i tesmil eder
                        //context.Saga : Veri tabanını temsil eder.

                        OrderItems = context.Message.OrderItems, //context.Message: StockReservedEvent'i tesmil eder
                        Payment = new PaymentMessage
                        {
                            CardName = context.Saga.CardName, //context.Saga : Veri tabanını temsil eder.
                            CardNumber = context.Saga.CardNumber,
                            CVV = context.Saga.CVV,
                            Expiration = context.Saga.Expiration,
                            TotalPrice = context.Saga.TotalPrice
                        },
                        BuyerId = context.Saga.BuyerId
                    })
                    .TransitionTo(StockReserved)
                    .Then(context =>
                    {
                        Console.WriteLine($"StockReservedEvent After : {context.Saga}");
                    }),
                //2.Seçenek
                When(StockNotReservedEvent)
                 .Publish(context => new OrchestrationOrderRequestFailedEvent(context.Saga.OrderId, context.Message.Reason))
                 .TransitionTo(StockNotReserved)
                 .Then(context =>
                 {
                     Console.WriteLine($"StockReservedEvent After : {context.Saga}");
                 })
           );

          During(StockReserved,
               //1.Seçenek
               When(PaymentCompletedEvent)
                  .Publish(context => new OrchestrationOrderRequestCompletedEvent(context.Saga.OrderId))
                  .TransitionTo(PaymentCompleted)
                  .Then(context =>
                  {
                      Console.WriteLine($"PaymentCompletedEvent After : {context.Message}");
                  })
                  .Finalize(),
               //2.Seçenek
               When(PaymentFailedEvent)
                  .Publish(context => new OrchestrationOrderRequestFailedEvent(context.Saga.OrderId, context.Message.Reason))
                  .Send(new Uri($"queue:{RabbitQueueName.StockRollBackMessageQueueName}"), context => new StockRollbackMessage(context.Message.OrderItems))
                  .TransitionTo(PaymentFailed)
                  .Then(context =>
                  {
                      Console.WriteLine($"PaymentFailedEvent After : {context.Message}");
                  })
           );

          SetCompletedWhenFinalized(); //State'i final olarak biten işleri veri tabanından siler.

        } 
    }
}
