using Dsw2025Tpi.Application.Dtos;
using Dsw2025Tpi.Application.Exceptions;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Dsw2025Tpi.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {

        private IProductsManagementService _productsManagmentService;

        public ProductsController(IProductsManagementService productsManagementService)
        {

            _productsManagmentService = productsManagementService;
        }

        //Agregar un producto #check
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] ProductModel.ProductRequest request)
        {
            try
            {
                var product = await _productsManagmentService.AddProduct(request);
                return Created("api/products", product

        );
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
            catch (DuplicatedEntityException de)
            {
                return Conflict(de.Message);
            }
            catch (PriceNullException pe)
            {
                return BadRequest(pe.Message);
            }
            catch (Exception e)
            {
                return Problem("Se produjo un error al guardar el producto, {0}", e.Message);
            }
        }

        // Obtener todos los productos 
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _productsManagmentService.GetProducts();
                return Ok(products); // 200
            }
            catch (EntityNotFoundException enft)
            {
                return NoContent(); // 204
            }
            catch (Exception e)
            {
                return Problem("Se produjo un error al obtener los productos, {0}", e.Message);
            }

        }

        // Obtener un producto por ID 
        [Authorize]
        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var product = await _productsManagmentService.GetProductById(id);
                return Ok(product); // 200
            }
            catch (EntityNotFoundException enft)
            {
                return NotFound(enft.Message); // 404
            }
            catch (Exception e)
            {
                return Problem("Se produjo un error al obtener el producto, {0}", e.Message);
            }


        }

        //deshabilitar un producto 
        [Authorize(Roles = "admin")]
        [HttpPatch("{id:Guid}")]
        public async Task<IActionResult> Disable(Guid id)
        {
            try
            {
                var success = await _productsManagmentService.DisableProductAsync(id);
            }
            catch (EntityNotFoundException enft)
            {
                return NotFound(enft.Message); // 404
            }
            catch (Exception e)
            {
                return Problem("Se produjo un error al deshabilitar el producto, {0}", e.Message);
            }
            return NoContent(); // 204
        }

        // Actualizar un producto por ID 
        [Authorize(Roles = "admin")]
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProductModel.ProductRequest request)
        {
            try
            {
                var response = await _productsManagmentService.UpdateAsync(request, id);
                return Ok(response);
            }
            catch (EntityNotFoundException enft)
            {
                return NotFound(enft.Message);
            }
            catch (ArgumentException ae)
            {
                return BadRequest(ae.Message);
            }
            catch (Exception e)
            {
                return Problem("Se produjo un error al actualizar el producto, {0}", e.Message);
            }
        }
    }
}