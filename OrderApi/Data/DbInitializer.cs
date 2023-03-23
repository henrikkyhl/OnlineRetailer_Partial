using System.Collections.Generic;
using System.Linq;
using System;
using SharedModels;
using Order = OrderApi.Models.Order;

namespace OrderApi.Data
{
    public class DbInitializer : IDbInitializer
    {
        // This method will create and seed the database.
        public void Initialize(OrderApiContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Look for any Products
            if (context.Orders.Any())
            {
                return;   // DB has been seeded
            }
            
            
            
            var orderLines = new List<OrderLine>{
                new OrderLine{Id = 0,Quantity = 15,ProductId = 1,OrderId = 0},
                new OrderLine{Id = 0,Quantity = 40,ProductId = 2,OrderId = 0},
                new OrderLine{Id = 0,Quantity = 4,ProductId = 3,OrderId = 0}
                
            };
            
            List<Order> orders = new List<Order>
            {
                new Order{Id =0,Date = DateTime.Today, Status = SharedModels.Order.OrderStatus.completed,CustomerId = 0,OrderLines = orderLines},
            };

            context.Orders.AddRange(orders);
            context.OrderLines.AddRange(orderLines);
            context.SaveChanges();
        }
    }
}
