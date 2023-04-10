using EasyNetQ;
using OrderApi.Data;
using OrderApi.Models;
using SharedModels;
using SharedModels.Messages;

namespace OrderApi.Infrastructure;

public class MessageListener
{
    private IServiceProvider _provider;
    private string connectionString;
    IBus bus;
    
    public MessageListener(IServiceProvider provider, string connectionString)
    {
        _provider = provider;
        this.connectionString = connectionString;
    }
    
    public void Start()
    {
        using (bus = RabbitHutch.CreateBus(connectionString))
        {

            bus.PubSub.Subscribe<OrderAcceptedMessage>("order.Accepted", HandleOrderAccepted);

            bus.PubSub.Subscribe<OrderRejectedMessage>("order.Rejected", HandleOrderRejected);
            lock (this)
            {
                Monitor.Wait(this);
            }
        }
    }
    
    private void HandleOrderRejected(OrderRejectedMessage obj)
    {
        Console.WriteLine($"ORDER REJECTED {obj.OrderId}");
        using var scope = _provider.CreateScope();
        var services = scope.ServiceProvider;
        var orderRepo = services.GetService<IOrderRepository<Order>>();
        
        orderRepo.Remove(obj.OrderId);
        Console.WriteLine($"ORDER REMOVED");
    }

    private void HandleOrderAccepted(OrderAcceptedMessage obj)
    {
        Console.WriteLine($"ORDER ACCEPTED {obj.OrderId}");
        using var scope = _provider.CreateScope();
        var services = scope.ServiceProvider;
        var orderRepo = services.GetService<IOrderRepository<Order>>();
        
        var order = orderRepo.Get(obj.OrderId);
        order.Status = OrderDto.OrderStatus.completed;
        orderRepo.Edit(order.Id, order);
        Console.WriteLine($"ORDER {obj.OrderId} CHANGED STATUS TO COMPLETED");
    }
}