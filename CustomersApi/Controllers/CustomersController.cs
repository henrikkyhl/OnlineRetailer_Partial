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
}