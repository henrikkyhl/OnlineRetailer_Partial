using System.Collections.Generic;
using CustomersApi.Data;
using CustomersApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CustomersApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IRepository<Customer> _repository;

    public CustomersController(IRepository<Customer> repository)
    {
        _repository = repository;
    }
    // Just for Testing!
    // GET Customers
    [HttpGet]
    public IEnumerable<Customer> Get()
    {
        return _repository.GetAll();
    }
    
    [HttpGet("{id}", Name = "GetCustomer")]
    public IActionResult GetCustomerById(int id)
    {
        var customer = _repository.Get(id);
        if (customer == null)
        {
            return NotFound();
        }
        return new ObjectResult(customer);
    }
    
    [HttpPost]
    public IActionResult Register([FromBody] Customer customer)
    {
        if (customer == null)
        {
            return BadRequest();
        }

        var newCustomer = _repository.Add(customer);
        return CreatedAtRoute("GetCustomer",
            new { id = newCustomer.Id }, newCustomer);
    }
    
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody]Customer customer)
    {
        if (customer == null || customer.Id != id)
        {
            return BadRequest();
        }

        var modifiedCustomer = _repository.Get(id);

        if (modifiedCustomer == null)
        {
            return NotFound();
        }

        modifiedCustomer.Email = customer.Email;
        modifiedCustomer.Phone = customer.Phone;
        modifiedCustomer.BillingAddress = customer.BillingAddress;
        modifiedCustomer.ShippingAddress = customer.ShippingAddress;

        _repository.Edit(modifiedCustomer);
        return new NoContentResult();
    }
}