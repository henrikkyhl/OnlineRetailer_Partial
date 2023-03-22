namespace CustomersApi.Models;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int Phone { get; set; }
    public string BillingAddress { get; set; }
    public string ShippingAddress { get; set; }
    public bool CreditStanding { get; set; }
    
    
}