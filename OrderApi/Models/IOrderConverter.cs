using SharedModels;

namespace OrderApi.Models;

public interface IOrderConverter
{
    public Order Convert(OrderDto order);
    public OrderDto Convert(Order order);
}