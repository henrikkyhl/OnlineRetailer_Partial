using System;
using System.Collections.Generic;
using SharedModels;

namespace OrderApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public SharedModels.Order.OrderStatus Status { get; set; }
        public List<OrderLine> OrderLines { get; set; }
        public int CustomerId { get; set; }
    }
}
