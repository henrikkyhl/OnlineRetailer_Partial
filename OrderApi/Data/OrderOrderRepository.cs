using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using Microsoft.AspNetCore.Mvc;
using SharedModels;
using Order = OrderApi.Models.Order;

namespace OrderApi.Data
{
    public class OrderOrderRepository : IOrderRepository<Order>
    {
        private readonly OrderApiContext db;

        public OrderOrderRepository(OrderApiContext context)
        {
            db = context;
        }

        Order IOrderRepository<Order>.Add(Order entity)
        {
            if (entity.Date == null)
                entity.Date = DateTime.Now;

            var newOrder = db.Orders.Add(entity).Entity;
            db.SaveChanges();
            return newOrder;
        }

        void IOrderRepository<Order>.Edit(int id ,Order entity)
        {
            var order = db.Orders.Find(id);
            if (order != null)
            {
                order.Status = entity.Status;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
            }

        }

        public IEnumerable<Order> GetAllByCustomer(int customerId)
        {
            var select = db.Orders.Where(order => order.CustomerId == customerId).Select(order => new Order()
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

        Order IOrderRepository<Order>.Get(int id)
        {
            var select = db.Orders.Where(o => o.Id == id).Select(order => new Order()
            {
                Id = order.Id,
                Date = order.Date,
                CustomerId = order.CustomerId,
                Status = order.Status,
                OrderLines = order.OrderLines.Select(ol => new OrderLine()
                {
                    Id = ol.Id,
                    ProductId = ol.ProductId,
                    Quantity = ol.Quantity,
                    OrderId = ol.OrderId,
                }).ToList()
            }).FirstOrDefault();
            
            return select;
        }

        IEnumerable<Order> IOrderRepository<Order>.GetAll()
        {
            var select = db.Orders.Select(order => new Order()
            {
                Id = order.Id,
                Date = order.Date,
                Status = order.Status,
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


        void IOrderRepository<Order>.Remove(int id)
        {
            var order = db.Orders.FirstOrDefault(p => p.Id == id);
            db.Orders.Remove(order);
            db.SaveChanges();
        }
    }
}