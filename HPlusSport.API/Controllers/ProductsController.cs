using HPlusSport.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace HPlusSport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly ShopContext _shopContext;
        public ProductsController(ShopContext shopContext)
        {
            _shopContext = shopContext;
            _shopContext.Database.EnsureCreated();
        }

        [HttpGet]
        public async Task<ActionResult> GetProducts()
        {
            return Ok(await _shopContext.Products.ToArrayAsync());
        }

        [HttpGet("available/{id}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetAvailableProducts(int id)
        {
            bool availability;
            if (id == 1)
            {
                availability = true;
            }
            else if (id == 2)
            {
                availability = false;
            }
            else
            {
                return NotFound(new { message = "invalid Category type" });
            }

            var availableProducts = await _shopContext.Products.Where(p => p.IsAvailable == availability).ToArrayAsync();
            return Ok(availableProducts);
        }
    


        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var product = await _shopContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }
            return Ok(product);
        }

        [HttpGet("CategoryFilter/{id}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(int id)
        {
            var availableProductsInCategory = await _shopContext.Products.Where(p => p.CategoryId == id).ToArrayAsync();
            return Ok(availableProductsInCategory);
        }


        [HttpPost]
        public async Task<ActionResult> AddProduct(Product product)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            _shopContext.Products.Add(product);
            await _shopContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new {id = product.Id}, product);
        }


    }
}
