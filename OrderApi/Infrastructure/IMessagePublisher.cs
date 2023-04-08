using System.Collections.Generic;
using SharedModels;

namespace OrderApi.Infrastructure
{
    public interface IMessagePublisher
    {
        void PublishOrderCreateMessage(int customerId, int orderId, IList<OrderLine> orderLine);
        void OrderStatusChangedMessage(int id, IList<OrderLine> orderLine, string status);
    }
}