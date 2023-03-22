using System.Collections.Generic;

namespace SharedModels
{
    public class OrderStatusChangedMessage
    {
        public int? CustomerId { get; set; }
        public IEnumerable<OrderLine> OrderLines { get; set; }
    }
}