﻿namespace Orchestration.StateMachineWorkerService.CustomState
{
    public class OrderStateInstance : SagaStateMachineInstance
    {
        //her bir satır için random correlation id üretilir.
        //state machine'in stateleri birbirinden ayırması için kullanılır.
        public Guid CorrelationId { get; set; } //SagaStateMachineInstance interface araacılığıyla geliyor

        public string CurrentState { get; set; }
        public string BuyerId { get; set; }
        public int OrderId { get; set; }

        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string Expiration { get; set; }
        public string CVV { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        public DateTime CreatedDate { get; set; }

        public override string ToString()
        {
            var properties = GetType().GetProperties().ToList();

            StringBuilder stringBuilder = new();

            properties.ForEach(p =>
            {
                var value = p.GetValue(this, null);
                stringBuilder.AppendLine($"{p.Name}:{value}");
            });

            stringBuilder.Append("------------------------");
            return stringBuilder.ToString();
        }
    }
}
