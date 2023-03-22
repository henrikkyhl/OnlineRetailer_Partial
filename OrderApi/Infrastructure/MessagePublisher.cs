using System;
using System.Collections.Generic;
using SharedModels;
using EasyNetQ;

namespace OrderApi.Infrastructure
{
    public class MessagePublisher : IMessagePublisher,IDisposable
    {
        IBus bus;
        
        public MessagePublisher(string connectionString)
        {
            bus = RabbitHutch.CreateBus(connectionString);
        }
        
        public void PublishOrderStatusChangeMEssage(int? customerId, IEnumerable<OrderLine> orderLines, string topic)
        {
            var message = new OrderStatusChangedMessage
            {
                CustomerId = customerId,
                OrderLines = orderLines
            };
            
            bus.PubSub.Publish(message,topic);
        }

        public void Dispose()
        {
            bus.Dispose();
        }
    }
}