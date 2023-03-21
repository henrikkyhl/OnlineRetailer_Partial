using CustomersApi.Models;

namespace CustomersApi.Data;

public class CustomerRepository: IRepository<Customer>
{
    private readonly CustomerApiContext db;

    public CustomerRepository(CustomerApiContext context)
    {
        db = context;
    }
    public IEnumerable<Customer> GetAll()
    {
        return db.Customers.ToList();
    }

    public Customer Get(int id)
    {
        throw new NotImplementedException();
    }

    public Customer Add(Customer entity)
    {
        throw new NotImplementedException();
    }

    public void Edit(Customer entity)
    {
        throw new NotImplementedException();
    }

    public void Remove(int id)
    {
        throw new NotImplementedException();
    }
}