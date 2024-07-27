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
            var products = await _shopContext.Products.ToArrayAsync();
            return Ok(products);
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
            // Check if there are any products in the given category
            bool categoryPresent = await _shopContext.Products.AnyAsync(p => p.CategoryId == id);

            if (!categoryPresent)
            {
                return NotFound($"No products found in category with ID {id}");
            }

            // Get all products in the given category
            var availableProductsInCategory = await _shopContext.Products
                .Where(p => p.CategoryId == id)
                .ToListAsync();

            return Ok(availableProductsInCategory);
        }

        [HttpPost]
        public async Task<ActionResult> AddProduct(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _shopContext.Products.Add(product);
            await _shopContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _shopContext.Entry(product).State = EntityState.Modified;
            try
            {
                await _shopContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_shopContext.Products.Any(p => p.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await _shopContext.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            _shopContext.Products.Remove(product);
            await _shopContext.SaveChangesAsync();

            return Ok(product);
        }

        [HttpGet("multipleProductsAtATime")]
        public async Task<ActionResult<IEnumerable<Product>>> GetMultipleProducts([FromQuery]int[] ids)
        {
            var products = await _shopContext.Products
                                             .Where(p => ids.Contains(p.Id))
                                             .ToListAsync();

            if (products.Count == 0)
            {
                return NotFound();
            }

            return Ok(products);
        }

        [HttpPost("Delete")]
        public async Task<ActionResult> DeleteMultipalProducts([FromQuery] int[] ids)
        {
            var products = new List<Product>();
            foreach (var id in ids)
            {
                var product = await _shopContext.Products.FindAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                products.Add(product);
            }
            _shopContext.Products.RemoveRange(products);
            await _shopContext.SaveChangesAsync();

            return Ok(products);
        }
    }
}
