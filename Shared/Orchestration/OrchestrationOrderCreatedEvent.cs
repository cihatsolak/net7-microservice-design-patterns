namespace Shared.Orchestration
{
    //CorrelatedBy<Guid>: Eventler dönüp dolaşıp state machine'e geldiğinde, bu event hangi satırla (db) ilgili event, hangi instance'la ilgili event olduğunu tespit etmesi lazım.
    //Event'in hangi instance ile ilişkili olduğunu tespit edebilmesi için benim her bir event'imin, State machine'de fırlatılan her bir event'in correlation id'si olması gereklidir.
    //Masstransit bir eventi dinlediğinde o event'in correlationid property'si üzerinden veri tabanında hangi satırla ilişkili olduğunu bulup, state'i değiştirecek.
    public interface IOrchestrationOrderCreatedEvent : CorrelatedBy<Guid>
    {
        List<OrderItemMessage> OrderItems { get; set; }
    }

    public class OrchestrationOrderCreatedEvent : IOrchestrationOrderCreatedEvent
    {
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
