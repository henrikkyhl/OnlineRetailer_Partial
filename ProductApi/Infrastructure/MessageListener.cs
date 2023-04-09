using EasyNetQ;
using ProductApi.Data;
using ProductApi.Models;
using SharedModels;
using SharedModels.Messages;

namespace ProductApi.Infrastructure;

public class MessageListener
{
    private readonly IServiceProvider _provider;
    private readonly string _connection;
    private IBus _bus;

    public MessageListener(IServiceProvider serviceProvider, string connection)
    {
        _provider = serviceProvider;
        _connection = connection;
    }

    public void Start()
    {
        using (_bus = RabbitHutch.CreateBus(_connection))
        {
            _bus.PubSub.Subscribe<OrderCreatedMessage>("product.orderCreated", HandleOrderCreated);
            
            _bus.PubSub.Subscribe<OrderStatusChangedMessage>("product.orderStatusChanged", HandleOrderCancelled,x=>x.WithTopic("cancelled"));
            
            _bus.PubSub.Subscribe<OrderStatusChangedMessage>("product.orderStatusChanged", HandleOrderShipped,x=>x.WithTopic("shipped"));
            
            lock (this)
            {
                Monitor.Wait(this);
            }
        }
    }

    private void HandleOrderShipped(OrderStatusChangedMessage obj)
    {
        Console.WriteLine("SHIP ORDER REQUEST "+obj.OrderId);
        using var scope = _provider.CreateScope();
        var services = scope.ServiceProvider;
        var repo = services.GetService<IRepository<Product>>();

        foreach (var orderLine in obj.OrderLine)
        {
            var product = repo.Get(orderLine.ProductId);
            var result = product;
            result.ItemsReserved -= orderLine.Quantity;
            result.ItemsInStock -= orderLine.Quantity;
            repo.Edit(result);
        }
    }

    private void HandleOrderCancelled(OrderStatusChangedMessage obj)
    {
        Console.WriteLine("CANCEL ORDER REQUEST "+obj.OrderId);
        using var scope = _provider.CreateScope();
        var services = scope.ServiceProvider;
        var repo = services.GetService<IRepository<Product>>();

        foreach (var orderLine in obj.OrderLine)
        {
            var product = repo.Get(orderLine.ProductId);
            var result = product;
            result.ItemsReserved -= orderLine.Quantity;
            repo.Edit(result);
        }
    }


    private void HandleOrderCreated(OrderCreatedMessage message)
    {
        Console.WriteLine("PRODUCT - LISTEN TO CREATE ORDER");
        using var scope = _provider.CreateScope();
        var services = scope.ServiceProvider;
        var productRepo = services.GetService<IRepository<Product>>();

        if (ProductItemsAvailable(message.OrderLines, productRepo))
        {
            foreach (var order in message.OrderLines)
            {
                var product = productRepo.Get(order.ProductId);
                product.ItemsReserved += order.Quantity;
                productRepo.Edit(product);
            }

            var replyMessage = new OrderAcceptedMessage
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

    private bool ProductItemsAvailable(IList<OrderLine> orderLines, IRepository<Product> productRepo)
    {
        foreach (var order in orderLines)
        {
            var product = productRepo.Get(order.ProductId);
            if (order.Quantity > product.ItemsInStock - product.ItemsReserved)
            {
                return false;
            }
        }

        return true;
    }
    
    
}