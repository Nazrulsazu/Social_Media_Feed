using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Social_media_feed.Data;
using Social_media_feed.Models;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class LikeController : ControllerBase
{
    private readonly AppDbContext _context;

    public LikeController(AppDbContext context)
    {
        _context = context;
    }

    // POST: api/Like
    [HttpPost]
    public async Task<IActionResult> LikePost(int userId, int postId)
    {
        var like = new Like { UserId = userId, PostId = postId };
        _context.Likes.Add(like);
        await _context.SaveChangesAsync();

        return Ok();
    }

    // DELETE: api/Like
    [HttpDelete]
    public async Task<IActionResult> UnlikePost(int userId, int postId)
    {
        var like = await _context.Likes.FirstOrDefaultAsync(l=> l.UserId==userId && l.PostId==postId);
          
        if (like == null)
        {
            return NotFound();
        }

        _context.Likes.Remove(like);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
