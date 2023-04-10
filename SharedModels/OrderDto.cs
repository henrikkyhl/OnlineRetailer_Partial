using System;
using System.Collections.Generic;

namespace SharedModels
{
    public class OrderDto
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int CustomerId { get; set; }
        public OrderStatus Status { get; set; }
        public IList<OrderLine> OrderLines { get; set; }
        public enum OrderStatus
        {
            pending,
            cancelled,
            completed,
            shipped,
            paid,
        }
    }

    public class OrderLine
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}