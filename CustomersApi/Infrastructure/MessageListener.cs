using CustomersApi.Data;
using CustomersApi.Models;
using EasyNetQ;
using SharedModels;
using SharedModels.Messages;

namespace CustomersApi.Infrastructure;

public class MessageListener
{
    private IServiceProvider _provider;
    private string _connection;
    private IBus _bus;

    public MessageListener(IServiceProvider provider, string connection)
    {
        _provider = provider;
        _connection = connection;
    }

    public void Start()
    {
        using (_bus = RabbitHutch.CreateBus(_connection))
        {
            _bus.PubSub.Subscribe<OrderPaidMessage>("customerApiPaid", HandleOrderPaid);
            _bus.PubSub.Subscribe<OrderCreatedMessage>("customerApiCreated", HandleOrderCreated);
            
            lock (this)
            {
                Monitor.Wait(this);
            }
        }
    }


    private void HandleOrderCreated(OrderCreatedMessage message)
    {
        using var scope = _provider.CreateScope();
        var services = scope.ServiceProvider;
        var customerRepository= services.GetService<IRepository<Customer>>();

        if (GoodStanding(message.CustomerId, customerRepository))
        {
            var customer = customerRepository.Get(message.CustomerId);
            customer.CreditStanding = false;
            customerRepository.Edit(customer);

            var replyMessage = new OrderAcceptedMessage()
            {
                OrderId = message.OrderId
            };

            _bus.PubSub.Publish(replyMessage);
        }
        else
        {
            var replyMessage = new OrderRejectedMessage
            {
                OrderId = message.OrderId
            };

            _bus.PubSub.Publish(replyMessage);
        }
    }

    private void HandleOrderPaid(OrderPaidMessage message)
    {
        using var scope = _provider.CreateScope();
        var services = scope.ServiceProvider;
        var customerRepository = services.GetService<IRepository<Customer>>();

        if (!GoodStanding(message.CustomerId, customerRepository))
        {
            var customer = customerRepository.Get(message.CustomerId);
            customer.CreditStanding = true;
            customerRepository.Edit(customer);

            var replyMessage = new OrderPayAcceptedMessage
            {
                OrderId = message.OrderId
            };

            _bus.PubSub.Publish(replyMessage);
        }
        else
        {
            var replyMessage = new OrderRejectedMessage
            {
                OrderId = message.OrderId
            };

            _bus.PubSub.Publish(replyMessage);
        }
    }

    private bool GoodStanding(int customerId, IRepository<Customer> customerRepo)
    {
        var customer = customerRepo.Get(customerId);
        return customer is 
            {CreditStanding: true};
    }
}