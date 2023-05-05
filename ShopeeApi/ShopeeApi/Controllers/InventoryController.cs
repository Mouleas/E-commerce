using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopeeApi.Data;
using ShopeeApi.Model;
using ShopeeApi.Dao;

namespace ShopeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly ShopeeApiContext _context;

        public InventoryController(ShopeeApiContext context)
        {
            _context = context;
        }

        // GET: api/Inventory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryModel>>> GetInventoryModel()
        {
          if (_context.InventoryModel == null)
          {
              return NotFound();
          }
            return await _context.InventoryModel.ToListAsync();
        }

        // GET: api/Inventory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryModel>> GetInventoryModel(int id)
        {
          if (_context.InventoryModel == null)
          {
              return NotFound();
          }
            var inventoryModel = await _context.InventoryModel.FindAsync(id);

            if (inventoryModel == null)
            {
                return NotFound();
            }

            return inventoryModel;
        }

        // PUT: api/Inventory/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInventoryModel(int id, [FromBody] DaoInventory inventory)
        {
            InventoryModel inventoryModel = await _context.InventoryModel.FindAsync(id);
            inventoryModel.ItemQuantity = inventory.ItemQuantity;
            if (id != inventoryModel.ItemId)
            {
                return BadRequest();
            }

            _context.Entry(inventoryModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventoryModelExists(id))
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

        // POST: api/Inventory
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<InventoryModel>> PostInventoryModel([FromBody] DaoInventory inventory)
        {
            Console.WriteLine("In post");
          if (_context.InventoryModel == null)
          {
              return Problem("Entity set 'ShopeeApiContext.InventoryModel'  is null.");
          }
            InventoryModel inventoryModel = new InventoryModel()
            {
                ItemName = inventory.ItemName,
                ItemType = inventory.ItemType,
                ItemPrice = inventory.ItemPrice,
                ItemDescription = inventory.ItemDescription,
                ItemQuantity = inventory.ItemQuantity,
                ItemImageName = inventory.ItemImageName
            };

            _context.InventoryModel.Add(inventoryModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInventoryModel", new { id = inventoryModel.ItemId }, inventoryModel);
        }

        // DELETE: api/Inventory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryModel(int id)
        {
            if (_context.InventoryModel == null)
            {
                return NotFound();
            }
            var inventoryModel = await _context.InventoryModel.FindAsync(id);
            if (inventoryModel == null)
            {
                return NotFound();
            }

            _context.InventoryModel.Remove(inventoryModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InventoryModelExists(int id)
        {
            return (_context.InventoryModel?.Any(e => e.ItemId == id)).GetValueOrDefault();
        }
    }
}
