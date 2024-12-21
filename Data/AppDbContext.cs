using Microsoft.EntityFrameworkCore;
using Social_media_feed.Models;

namespace Social_media_feed.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }
    
       public DbSet<User> Users { get; set; }
       public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }
      public DbSet<Picture> Pictures { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Like>()
                    .HasOne(l => l.User)
                    .WithMany(u => u.Likes)
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Like>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Picture>()
                .HasOne(p => p.Post)
                .WithMany(p => p.Pictures)
                .HasForeignKey(p => p.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.ClaimType)
                .IsUnique();
        }

    }
}
