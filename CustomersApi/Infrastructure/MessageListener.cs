using CustomersApi.Data;
using CustomersApi.Models;
using EasyNetQ;

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
            
        }
        
        lock (this)
        {
            Monitor.Wait(this);
        }
    }


    public void HandleOrderCreated()
    {
        
    }

    public void HandleOrderPaid()
    {
        
    }

    public bool GoodStanding(int customerId, IRepository<Customer> customerRepo)
    {
        var customer = customerRepo.Get(customerId);
        return customer is 
            {CreditStanding: true};
    }
}