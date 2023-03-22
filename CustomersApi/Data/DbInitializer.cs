using System.Collections.Generic;
using System.Linq;
using CustomersApi.Models;

namespace CustomersApi.Data;

public class DbInitializer : IDbInitializer
{
    // This method will create and seed the database.
    
    public void Initialize(CustomerApiContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        
        if (context.Customers.Any())
        {
            return;   // DB has been seeded
        }

        List<Customer> customers = new List<Customer>()
        {
            new Customer
            {
                Name = "Peter",
                BillingAddress = "Yolovej 1",
                Email = "Yoloemail1@gmail.com",
                CreditStanding = true,
                Phone = 71972301,
                ShippingAddress = "Yolovej 1"
            },
            new Customer
            {
                Name = "Bo",
                BillingAddress = "Yolovej 2",
                Email = "Yoloemail2@gmail.com",
                CreditStanding = true,
                Phone = 71972302,
                ShippingAddress = "Yolovej 2"
            },
            new Customer
            {
                Name = "Lars",
                BillingAddress = "Yolovej 3",
                Email = "Yoloemail3@gmail.com",
                CreditStanding = false,
                Phone = 71972303,
                ShippingAddress = "Yolovej 3"
            },
        };
        context.Customers.AddRange(customers);
        context.SaveChanges();
    }
}