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
        private readonly IOrderRepository<Order> _orderRepository;
        private IServiceGateway<ProductDto> _productServiceGateway;
        private IMessagePublisher _messagePublisher;
        private IOrderConverter _converter;

        public OrdersController(IOrderRepository<Order> repos,
            IMessagePublisher publisher,
            IServiceGateway<ProductDto> gateway,
            IOrderConverter orderConverter)
        {
            _orderRepository = repos;
            _messagePublisher = publisher;
            _productServiceGateway = gateway;
            _converter = orderConverter;
        }

        // GET: orders
        [HttpGet]
        public IEnumerable<OrderDto> Get()
        {
            var orders = _orderRepository.GetAll();
            var dtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderLines = o.OrderLines,
                CustomerId = o.CustomerId,
                Status = o.Status,
                Date = o.Date
            });
            return dtos;
        }

        [HttpGet("getAllByCustomer/{customerId}")]
        public IEnumerable<OrderDto> GetByCustomerId(int customerId)
        {
            var orders = _orderRepository.GetAllByCustomer(customerId);
            var dtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                OrderLines = o.OrderLines,
                CustomerId = o.CustomerId,
                Status = o.Status,
                Date = o.Date
            });
            return dtos;
        }

        // GET orders/5
        [HttpGet("{id}", Name = "GetOrder")]
        public IActionResult Get(int id)
        {
            var item = _orderRepository.Get(id);
            if (item == null)
            {
                return NotFound();
            }

            return new ObjectResult(item);
        }

        // POST orders
        [HttpPost]
        public IActionResult Post([FromBody] OrderDto order)
        {
            if (order == null)
            {
                return BadRequest();
            }

            try
            {
                order.Status = OrderDto.OrderStatus.pending;
                
                var newOrder = _orderRepository.Add(_converter.Convert(order));
                _messagePublisher.PublishOrderCreateMessage(
                    newOrder.CustomerId, newOrder.Id,newOrder.OrderLines);
                
                return CreatedAtRoute("GetOrder", new {id = newOrder.Id}, newOrder);
            }
            catch
            {
                return StatusCode(500, "An error occured.");
            }
        }
        

        [HttpPut("cancel/{id}")]
        public IActionResult Cancel(int id)
        {
            var order = _orderRepository.Get(id);
            if (order.Status ==  OrderDto.OrderStatus.shipped)
            {
                return StatusCode(403);
            }
            _orderRepository.Edit(id,new Order
            {
                Id = id,
                Status = OrderDto.OrderStatus.cancelled
            });
            _messagePublisher.OrderStatusChangedMessage(id,order.OrderLines,"cancelled");
            _messagePublisher.CreditStandingChangedMessage(order.CustomerId);
            return Ok();
        }
        
        [HttpPut("ship/{id}")]
        public IActionResult Ship(int id)
        {
            var order = _orderRepository.Get(id);
            _orderRepository.Edit(id,new Order
            {
                Id = id,
                Status = OrderDto.OrderStatus.shipped
            });
            _messagePublisher.OrderStatusChangedMessage(id,order.OrderLines,"shipped");
            return Ok();
        }
        
        [HttpPut("pay/{id}")]
        public IActionResult Pay(int id)
        {
            var order = _orderRepository.Get(id);
            _orderRepository.Edit(id,new Order
            {
                Id = id,
                Status = OrderDto.OrderStatus.paid
            });
            _messagePublisher.CreditStandingChangedMessage(order.CustomerId);
            return Ok();
        }
    }
}