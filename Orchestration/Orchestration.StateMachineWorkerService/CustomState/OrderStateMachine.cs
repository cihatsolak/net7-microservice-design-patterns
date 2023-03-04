namespace Orchestration.StateMachineWorkerService.CustomState
{
    /// <summary>
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
                //veri tabanındaki order ile event'den gelen order id'yi karşılaştır. Eğer var ise herhangi birşey yapma. Eğer yok ise yeni bir satır oluştur. Oluşturmuş olduğun
                //bu order state instance satırını da correlation id'sine random bir değer ata.
                eventCorrelationConfigurator.CorrelateBy<int>(database => database.OrderId, @event => @event.Message.OrderId).SelectId(selector => Guid.NewGuid());

                //Initial evresinden order created evresine geçerken bunu açıkca belirtmek gereklidir.
                //Initial aşamasındayken, eğerki OrderCreatedRequestEvent event'i geldiyse, Then metoduyla birlikte şunu yap demek.
                Initially(
                 When(OrderCreatedRequestEvent)
                .Then(context =>
                {
                    // context.Instance: veri tabanına kaydedilecek olan satırı temsil eder.
                    // context.Data: Event'den gelen datayı temsil eder.

                    context.Instance.BuyerId = context.Data.BuyerId;
                    
                    context.Instance.OrderId = context.Data.OrderId;
                    context.Instance.CreatedDate = DateTime.Now;

                    context.Instance.CardName = context.Data.Payment.CardName;
                    context.Instance.CardNumber = context.Data.Payment.CardNumber;
                    context.Instance.CVV = context.Data.Payment.CVV;
                    context.Instance.Expiration = context.Data.Payment.Expiration;
                    context.Instance.TotalPrice = context.Data.Payment.TotalPrice;
                })
                .Then(context => //state'i değiştirmeden Then blogu ile ikinci kez business çalıştırabilirim
                {
                    Console.WriteLine($"OrderCreatedRequestEvent before : {context.Saga}");
                })
                .TransitionTo(OrderCreated) //yukarıdaki Then blokları işlendikten sonra inital state'inde order created evresine geçmesi için
                .Then(context =>
                {
                    Console.WriteLine($"OrderCreatedRequestEvent After : {context.Saga}");
                }));

            });
        }
    }
}
