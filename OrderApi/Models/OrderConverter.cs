using SharedModels;

namespace OrderApi.Models;

public class OrderConverter : IOrderConverter
{
    public Order Convert(OrderDto order)
    {
        return new Order()
        {
            Id = order.Id,
            Date = order.Date,
            Status = order.Status,
            CustomerId = order.CustomerId,
            OrderLines = order.OrderLines
        };
    }

    public OrderDto Convert(Order order)
    {
        return new OrderDto()
        {
            Id = order.Id,
            Date = order.Date,
            Status = order.Status,
            CustomerId = order.CustomerId,
            OrderLines = order.OrderLines
        };
    }
}