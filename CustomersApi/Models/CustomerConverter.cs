using SharedModels;

namespace CustomersApi.Models;

public class CustomerConverter : IConverter<Customer, CustomerDto>
{
    public Customer ConvertDtoToModel(CustomerDto dto)
    {
        return new Customer
        {
            Id = dto.Id,
            Name = dto.Name,
            Email = dto.Email,
            Phone = dto.Phone,
            BillingAddress = dto.BillingAddress,
            ShippingAddress = dto.ShippingAddress,
            CreditStanding = dto.CreditStanding
        };
    }
    
    public CustomerDto ConvertModelToDto(Customer model)
    {
        return new CustomerDto
        {
            Id = model.Id,
            Name = model.Name,
            Email = model.Email,
            Phone = model.Phone,
            BillingAddress = model.BillingAddress,
            ShippingAddress = model.ShippingAddress,
            CreditStanding = model.CreditStanding
        };
    }
}