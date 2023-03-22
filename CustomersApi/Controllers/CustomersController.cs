using System.Collections.Generic;
using CustomersApi.Data;
using CustomersApi.Models;
using Microsoft.AspNetCore.Mvc;
using SharedModels;

namespace CustomersApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IRepository<Customer> _repository;
    private readonly IConverter<Customer, CustomerDto> _customerConverter;

    public CustomersController(IRepository<Customer> repository, IConverter<Customer, CustomerDto> customerConverter)
    {
        _repository = repository;
        _customerConverter = customerConverter;
    }
    
    // GET Customers
    [HttpGet]
    public IEnumerable<CustomerDto> GetAll()
    {
        var customerDtoList = new List<CustomerDto>();
        foreach (var customer in _repository.GetAll())
        {
            var customerDto = _customerConverter.ConvertModelToDto(customer);
            customerDtoList.Add(customerDto);
        }
        return customerDtoList;
    }
    
    [HttpGet("{id}", Name = "GetCustomer")]
    public IActionResult GetCustomerById(int id)
    {
        var customer = _repository.Get(id);
        if (customer == null)
        {
            return NotFound();
        }
        var customerDto = _customerConverter.ConvertModelToDto(customer);
        return new ObjectResult(customerDto);
    }
    
    [HttpPost]
    public IActionResult Register([FromBody] CustomerDto customerDto)
    {
        if (customerDto == null)
        {
            return BadRequest();
        }

        var customer = _customerConverter.ConvertDtoToModel(customerDto);
        var newCustomer = _repository.Add(customer);
            
        return new ObjectResult(newCustomer);
    }
    
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody]CustomerDto customerDto)
    {
        if (customerDto == null || customerDto.Id != id)
        {
            return BadRequest();
        }

        var modifiedCustomer = _repository.Get(id);

        if (modifiedCustomer == null)
        {
            return NotFound();
        }

        modifiedCustomer.Email = customerDto.Email;
        modifiedCustomer.Phone = customerDto.Phone;
        modifiedCustomer.BillingAddress = customerDto.BillingAddress;
        modifiedCustomer.ShippingAddress = customerDto.ShippingAddress;

        _repository.Edit(modifiedCustomer);
        return new NoContentResult();
    }
    
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (_repository.Get(id) == null)
        {
            return NotFound();
        }
        _repository.Remove(id);
        return new NoContentResult();
    }
}