using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Authorize]

    [Route("api/orders")]
    public class OrdersController : ControllerBase 
    {
        private IOrderManagement _orderManagmentService;
        public OrdersController(IOrderManagement orderManagement)
        {
            _orderManagmentService = orderManagement;
        }

        [Authorize(Roles = "customer")]
        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderModel.OrderRequest request)
        {
            try
            {
                var order = await _orderManagmentService.AddOrder(request);
                return Created("api/orders", order); 
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message); 
            }
            catch (EntityNotFoundException enfe)
            {
                return NotFound(enfe.Message);
            }
            catch (DuplicatedEntityException de)
            {
                return Conflict(de.Message);
            }
            catch (Exception e)
            {
                return Problem(e.Message); 
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetOrders(
            [FromQuery] string? status,
            [FromQuery] Guid? customerId,
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10) 
        {
            try
            {
                var orders = await _orderManagmentService.GetOrders(status, customerId, pageNumber, pageSize);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener las órdenes: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            try
            { 
                var order = await _orderManagmentService.GetOrderById(id); 
                if (order == null) return NotFound();
                return Ok(order);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, $"Error al buscar la orden: {ex.Message}");
            }
        }


        [Authorize(Roles = "admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] OrderModel.UpdateOrderStatusRequest request)
        {
            try
            {
                var result = await _orderManagmentService.UpdateOrderStatus(id, request.NewStatus);
                return Ok(result);
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);  
            }
            catch (Exception ex)
            {
                return Problem(ex.Message); 
            }
        }
    }
}