using System;
using System.Collections.Generic;
using System.Linq;
using CustomersApi.Models;
using Microsoft.EntityFrameworkCore;

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
        return db.Customers.FirstOrDefault(o => o.Id == id);
    }

    public Customer Add(Customer entity)
    {
        var newOrder = db.Customers.Add(entity).Entity;
        db.SaveChanges();
        return newOrder;
    }
    
    public void Edit(Customer entity)
    {
        db.Entry(entity).State = EntityState.Modified;
        db.SaveChanges();
    }

    public void Remove(int id)
    {
        throw new NotImplementedException();
    }
}