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

            bus.PubSub.Subscribe<OrderAcceptedMessage>("orderAcceptedMessage", HandleOrderAccepted);

            bus.PubSub.Subscribe<OrderRejectedMessage>("orderRejectedMessage", HandleOrderRejected);
            lock (this)
            {
                Monitor.Wait(this);
            }
        }
    }
    
    private void HandleOrderRejected(OrderRejectedMessage obj)
    {
        using var scope = _provider.CreateScope();
        var services = scope.ServiceProvider;
        var orderRepo = services.GetService<IOrderRepository<Order>>();
        
        orderRepo.Remove(obj.OrderId);
    }

    private void HandleOrderAccepted(OrderAcceptedMessage obj)
    {
        using var scope = _provider.CreateScope();
        var services = scope.ServiceProvider;
        var orderRepo = services.GetService<IOrderRepository<Order>>();
        
        var order = orderRepo.Get(obj.OrderId);
        order.Status = OrderDto.OrderStatus.completed;
        orderRepo.Edit(order.Id, order);
    }
}