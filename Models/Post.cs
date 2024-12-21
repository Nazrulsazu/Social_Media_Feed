using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.ComponentModel.DataAnnotations;

namespace Social_media_feed.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<Like> Likes { get; set; }
        public ICollection<Picture> Pictures { get; set; }
    }
}
