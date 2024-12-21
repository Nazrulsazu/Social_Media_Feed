using Microsoft.AspNetCore.Mvc;
using Social_media_feed.Data;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class UserController : Controller
{
    public readonly AppDbContext _context;
    public UserController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
}
