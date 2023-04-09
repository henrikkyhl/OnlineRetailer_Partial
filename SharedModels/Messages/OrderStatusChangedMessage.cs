using System.Collections.Generic;

namespace SharedModels
{
    public class OrderStatusChangedMessage
    {
        public int OrderId { get; set; }
        public IList<OrderLine> OrderLine { get; set; }
    }
}