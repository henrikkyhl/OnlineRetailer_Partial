using System;
using System.Collections.Generic;
using SharedModels;

namespace OrderApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public SharedModels.OrderDto.OrderStatus Status { get; set; }
        public IList<OrderLine> OrderLines { get; set; }
        public int CustomerId { get; set; }
    }
}
