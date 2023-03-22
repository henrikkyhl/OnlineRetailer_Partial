using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Data;
using OrderApi.Infrastructure;
using OrderApi.Models;
using RestSharp;
using SharedModels;
using Order = OrderApi.Models.Order;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IRepository<Order> repository;
        private IServiceGateway<ProductDto> productServiceGateway;
        private IMessagePublisher messagePublisher;

        public OrdersController(IRepository<Order> repos,
            IMessagePublisher publisher,
            IServiceGateway<ProductDto> gateway)
        {
            repository = repos;
            messagePublisher = publisher;
            productServiceGateway = gateway;
        }

        // GET: orders
        [HttpGet]
        public IEnumerable<Order> Get()
        {
            return repository.GetAll();
        }

        // GET orders/5
        [HttpGet("{id}", Name = "GetOrder")]
        public IActionResult Get(int id)
        {
            var item = repository.Get(id);
            if (item == null)
            {
                return NotFound();
            }

            return new ObjectResult(item);
        }

        // POST orders
        [HttpPost]
        public IActionResult Post([FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest();
            }
            

            if (!ProductItemsAvailable(order))
            {
                return StatusCode(500, "Items are not available...");
            }
            
            try
            {
                messagePublisher.PublishOrderStatusChangedMessage(
                    order.CustomerId, order.OrderLines, "completed");
                order.Status = SharedModels.Order.OrderStatus.completed;
                var newOrder = repository.Add(order);
                return CreatedAtRoute("GetOrder", new {id = newOrder.Id}, newOrder);
            }
            catch
            {
                return StatusCode(500, "An error occured.");
            }
        }

        private bool ProductItemsAvailable(Order order)
        {
            foreach (var orderLine in order.OrderLines)
            {
                var orderedProduct = productServiceGateway.Get(orderLine.ProductId);
                if (orderedProduct == null)
                {
                    Console.WriteLine("ITS NULL");
                }
                if (orderLine.Quantity > orderedProduct.ItemsInStock - orderedProduct.ItemsReserved)
                {
                    return false;
                }
            }
            return true;
        }
    }
}