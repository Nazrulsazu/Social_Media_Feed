using System.ComponentModel.DataAnnotations;

namespace Social_media_feed.Models
{
    public class Picture
    {
        [Key]
        public int PictureId { get; set; }

        [Required]
        public string Url { get; set; } 

        public string Caption { get; set; } 

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;     
        public int PostId { get; set; }
        public Post Post { get; set; } 
    }
}
