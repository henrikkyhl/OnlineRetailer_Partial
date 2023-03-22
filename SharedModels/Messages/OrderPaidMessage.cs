namespace SharedModels.Messages;

public class OrderPaidMessage
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
}