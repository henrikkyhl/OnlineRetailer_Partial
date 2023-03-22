﻿using EasyNetQ;
using ProductApi.Data;
using ProductApi.Models;
using SharedModels;
using SharedModels.Messages;

namespace ProductApi.Infrastructure;

public class MessageListener
{
    private IServiceProvider _provider;
    private string _connection;
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
            _bus.PubSub.Subscribe<OrderCreatedMessage>("productApiCreate", HandleOrderCreated);
            
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

    public bool ProductItemsAvailable(IList<OrderLine> orderLines, IRepository<Product> productRepo)
    {
        foreach (var order in orderLines)
        {
            var product = productRepo.Get(order.ProductId);
            if (order.Quantity > product.ItemsReserved - product.ItemsReserved)
            {
                return false;
            }
        }

        return false;
    }
    
    
}