using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Social_media_feed.Data;
using Social_media_feed.Models;
using System.Collections.Generic;
using System.IO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using Azure.Core;

[Route("api/[controller]")]
[ApiController]
[Authorize] // Ensures that only authenticated users can access the endpoints
public class PostController : ControllerBase
{
    private readonly AppDbContext _appDbContext;
    private readonly IWebHostEnvironment _environment;

    public PostController(AppDbContext appDbContext, IWebHostEnvironment environment)
    {
        _appDbContext = appDbContext;
        _environment = environment;
    }
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
    {
        try
        {
            var posts = await _appDbContext.Posts.ToListAsync();
            return Ok(posts);
        }
        catch (Exception ex)
        {
            // Log the error
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, "Internal server error");
        }
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<Post>> GetPost(int id)
    {
        var post = await _appDbContext.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Pictures)
            .FirstOrDefaultAsync(p => p.PostId == id);

        if (post == null)
        {
            return NotFound();
        }

        return Ok(post);
    }

  

[Authorize]
[HttpPost]
public async Task<ActionResult<Post>> CreatePost([FromForm] string content, [FromForm] IFormFile? image)
{
    // Get the token from the request headers
    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
    Console.WriteLine($"Token: {token}");

    // Create a JWT handler
    var handler = new JwtSecurityTokenHandler();

    // Parse the token
    var jwtToken = handler.ReadJwtToken(token);

    // Extract the 'sub' claim
    var subClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
    Console.WriteLine($"User ID from 'sub' claim: {subClaim}");

    if (string.IsNullOrEmpty(subClaim))
        {
            return Unauthorized("User not authenticated");
        
        }

        if (!int.TryParse(subClaim, out var userId))
        {
            return Unauthorized("Invalid user ID in token");
           
        }
        userId=int.Parse(subClaim);
        var post = new Post
    {
        Content = content,
        UserId = userId,
        CreatedDate = DateTime.UtcNow,
    };

    if (image != null)
    {
        var imagePath = await SaveImageAsync(image);
        if (imagePath != null)
        {
            var picture = new Picture { Url = imagePath, Post = post };
            post.Pictures = new List<Picture> { picture };
        }
    }

    _appDbContext.Posts.Add(post);
    await _appDbContext.SaveChangesAsync();
    return CreatedAtAction(nameof(GetPost), new { id = post.PostId }, post);
}





[HttpPut("{id}")]
    public async Task<IActionResult> UpdatePost(int id, Post post)
    {
        if (id != post.PostId)
        {
            return BadRequest();
        }

        _appDbContext.Entry(post).State = EntityState.Modified;

        try
        {
            await _appDbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_appDbContext.Posts.Any(p => p.PostId == id))
            {
                return NotFound();
            }
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        var post = await _appDbContext.Posts
            .Include(p => p.Pictures)
            .FirstOrDefaultAsync(p => p.PostId == id);

        if (post == null)
        {
            return NotFound();
        }

        // Optionally delete images associated with the post
        if (post.Pictures != null)
        {
            foreach (var picture in post.Pictures)
            {
                var filePath = Path.Combine(_environment.WebRootPath, picture.Url.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }

        _appDbContext.Posts.Remove(post);
        await _appDbContext.SaveChangesAsync();

        return NoContent();
    }

    private async Task<string?> SaveImageAsync(IFormFile imageFile)
    {
        if (imageFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
            Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists

            var uniqueFileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Return the relative path to store in the database
            return $"/images/{uniqueFileName}";
        }

        return null;
    }
}
