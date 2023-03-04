namespace Orchestration.StateMachineWorkerService.CustomState
{
    /// <summary>
    /// Bu sınıf tüm distributed transaction'ı yönettiğim yer
    /// Then: Business kodları yazacağınız yer
    /// </summary>
    public class OrderStateMachine : MassTransitStateMachine<OrderStateInstance>
    {
        public Event<IOrderCreatedRequestEvent> OrderCreatedRequestEvent { get; set; }
        public State OrderCreated { get; set; }

        public OrderStateMachine()
        {
            InstanceState(p => p.CurrentState); // Initial State, İlk State

            Event(() => OrderCreatedRequestEvent, eventCorrelationConfigurator =>
            {
                //veri tabanı ile event'deki order id'leri karşılaştırıyorum. eğer var ise yeni bir satır oluşturma. eğer yok ise SelectId ile beraber yeni bir guid oluştur.
                //veri tabanındaki order ile event'den gelen order id'yi karşılaştır. Eğer var ise herhangi birşey yapma. Eğer yok ise yeni bir satır oluştur. Oluşturmuş olduğun bu order state instance satırını da correlation id'sine random bir değer ata.
                //aynı order id'ye sahip event fırlatılırsa, veri tabanında yeni bir satır oluşmaz. (aşağıdaki kontrolle beraber)
                eventCorrelationConfigurator.CorrelateBy<int>(database => database.OrderId, @event => @event.Message.OrderId).SelectId(selector => Guid.NewGuid());

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
                .Publish(context => new OrchestrationOrderCreatedEvent
                {
                    OrderItems = context.Message.OrderItems
                }) // Publish ya da Send kullanılabilir.
                .TransitionTo(OrderCreated) //yukarıdaki Then blokları (business kodlar) işlendikten sonra inital state'inde order created evresine geçmesi için
                
                .Then(context =>
                {
                    Console.WriteLine($"OrderCreatedRequestEvent After : {context.Saga}");
                }));

            });
        }
    }
}
