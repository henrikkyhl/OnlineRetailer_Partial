using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using SharedModels;
using Order = OrderApi.Models.Order;

namespace OrderApi.Data
{
    public class OrderRepository : IRepository<Order>
    {
        private readonly OrderApiContext db;

        public OrderRepository(OrderApiContext context)
        {
            db = context;
        }

        Order IRepository<Order>.Add(Order entity)
        {
            if (entity.Date == null)
                entity.Date = DateTime.Now;
            
            var newOrder = db.Orders.Add(entity).Entity;
            db.SaveChanges();
            return newOrder;
        }

        void IRepository<Order>.Edit(Order entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }

        Order IRepository<Order>.Get(int id)
        {
            return db.Orders.FirstOrDefault(o => o.Id == id);
        }

        IEnumerable<Order> IRepository<Order>.GetAll()
        {
            var select = db.Orders.Select(order => new Order()
            {
                Id = order.Id,
                Date = order.Date,
                CustomerId = order.CustomerId,
                OrderLines = order.OrderLines.Select(ol => new OrderLine()
                {
                    Id = ol.Id,
                    ProductId = ol.ProductId,
                    Quantity = ol.Quantity,
                    OrderId = ol.OrderId,
                }).ToList(),
            });
            return select.ToList();
        }

        void IRepository<Order>.Remove(int id)
        {
            var order = db.Orders.FirstOrDefault(p => p.Id == id);
            db.Orders.Remove(order);
            db.SaveChanges();
        }
    }
}
