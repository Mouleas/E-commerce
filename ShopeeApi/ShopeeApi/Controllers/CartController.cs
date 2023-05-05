using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopeeApi.Data;
using ShopeeApi.Model;

namespace ShopeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ShopeeApiContext _context;

        public CartController(ShopeeApiContext context)
        {
            _context = context;
        }

        // GET: api/Cart
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartModel>>> GetCartModel()
        {
          if (_context.CartModel == null)
          {
              return NotFound();
          }
            return await _context.CartModel.ToListAsync();
        }

        // GET: api/Cart/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CartModel>> GetCartModel(int id)
        {
          if (_context.CartModel == null)
          {
              return NotFound();
          }
            var cartModel = await _context.CartModel.FindAsync(id);

            if (cartModel == null)
            {
                return NotFound();
            }

            return cartModel;
        }

        // PUT: api/Cart/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCartModel(int id, CartModel cartModel)
        {
            if (id != cartModel.CartId)
            {
                return BadRequest();
            }

            _context.Entry(cartModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartModelExists(id))
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

        // POST: api/Cart
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CartModel>> PostCartModel(CartModel cartModel)
        {
          if (_context.CartModel == null)
          {
              return Problem("Entity set 'ShopeeApiContext.CartModel'  is null.");
          }
            _context.CartModel.Add(cartModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCartModel", new { id = cartModel.CartId }, cartModel);
        }

        // DELETE: api/Cart/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartModel(int id)
        {
            if (_context.CartModel == null)
            {
                return NotFound();
            }
            var cartModel = await _context.CartModel.FindAsync(id);
            if (cartModel == null)
            {
                return NotFound();
            }

            _context.CartModel.Remove(cartModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CartModelExists(int id)
        {
            return (_context.CartModel?.Any(e => e.CartId == id)).GetValueOrDefault();
        }
    }
}
