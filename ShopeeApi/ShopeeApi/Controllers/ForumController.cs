using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopeeApi.Dao;
using ShopeeApi.Data;
using ShopeeApi.Model;

namespace ShopeeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForumController : ControllerBase
    {
        private readonly ShopeeApiContext _context;

        public ForumController(ShopeeApiContext context)
        {
            _context = context;
        }

        // GET: api/Forum
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ForumModel>>> GetForumModel()
        {
          if (_context.ForumModel == null)
          {
              return NotFound();
          }
            return await _context.ForumModel.ToListAsync();
        }

        // GET: api/Forum/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ForumModel>> GetForumModel(int id)
        {
          if (_context.ForumModel == null)
          {
              return NotFound();
          }
            var forumModel = await _context.ForumModel.FindAsync(id);

            if (forumModel == null)
            {
                return NotFound();
            }

            return forumModel;
        }

        // PUT: api/Forum/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutForumModel(int id, [FromBody] DaoForum forum)
        {
            ForumModel forumModel = await _context.ForumModel.FindAsync(id);
            forumModel.ForumBody = forum.ForumBody;
            forumModel.ForumSubject = forum.ForumSubject;
            forumModel.ForumStatus = forum.ForumStatus;

            if (id != forumModel.ForumId)
            {
                return BadRequest();
            }

            _context.Entry(forumModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ForumModelExists(id))
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

        // POST: api/Forum
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ForumModel>> PostForumModel([FromBody] DaoForum forum)
        {
          if (_context.ForumModel == null)
          {
              return Problem("Entity set 'ShopeeApiContext.ForumModel'  is null.");
          }
            ForumModel forumModel = new ForumModel()
            {
                ItemId = forum.ItemId,
                UserId = forum.UserId,
                ForumSubject = forum.ForumSubject,
                ForumBody = forum.ForumBody,
                ForumStatus = forum.ForumStatus
            };
            _context.ForumModel.Add(forumModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetForumModel", new { id = forumModel.ForumId }, forumModel);
        }

        // DELETE: api/Forum/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteForumModel(int id)
        {
            if (_context.ForumModel == null)
            {
                return NotFound();
            }
            var forumModel = await _context.ForumModel.FindAsync(id);
            if (forumModel == null)
            {
                return NotFound();
            }

            _context.ForumModel.Remove(forumModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ForumModelExists(int id)
        {
            return (_context.ForumModel?.Any(e => e.ForumId == id)).GetValueOrDefault();
        }
    }
}
