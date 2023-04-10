using System;
using System.Collections.Generic;
using EasyNetQ;
using SharedModels;
using SharedModels.Messages;


namespace OrderApi.Infrastructure
{
    public class MessagePublisher : IMessagePublisher,IDisposable
    {
        IBus bus;
        
        public MessagePublisher(string connectionString)
        {
            bus = RabbitHutch.CreateBus(connectionString);
        }

        public void PublishOrderCreateMessage(int customerId, int orderId,IList<OrderLine> orderLine )
        {
            var message = new OrderCreatedMessage
            { 
                CustomerId = customerId,
                OrderId = orderId,
                OrderLines = orderLine 
            };
            bus.PubSub.Publish(message);
        }
        
        public void CreditStandingChangedMessage(int customerId)
        {
            var message = new CreditStandingChangedMessage
            {
                CustomerId = customerId
            };
            bus.PubSub.Publish(message);
        }

        public void OrderStatusChangedMessage(int id, IList<OrderLine> orderLine, string topic)
        {
            var message = new OrderStatusChangedMessage
            {
                OrderId = id,
                OrderLine = orderLine,
            };
            bus.PubSub.Publish(message, x =>
            {
                x.WithTopic(topic);
            });
        }

        public void Dispose()
        {
            bus.Dispose();
        }
    }
}