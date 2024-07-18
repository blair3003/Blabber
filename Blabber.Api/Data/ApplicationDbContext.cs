using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Blabber.Api.Models;

namespace Blabber.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Blab> Blabs { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigureRelationships(modelBuilder);
            SeedData(modelBuilder);
        }

        private void ConfigureRelationships(ModelBuilder modelBuilder)
        {
            // Configure unique constraint for Author
            modelBuilder.Entity<Author>()
                .HasIndex(a => a.ApplicationUserId)
                .IsUnique();

            // Configure relationships for ApplicationUser
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(au => au.Author)
                .WithOne(a => a.ApplicationUser)
                .HasForeignKey<Author>(a => a.ApplicationUserId);

            // Configure relationships for Author
            modelBuilder.Entity<Author>()
                .HasMany(a => a.Likes)
                .WithMany(b => b.Liked)
                .UsingEntity(
                    "Likes",
                    l => l.HasOne(typeof(Blab))
                          .WithMany()
                          .HasForeignKey("LikesId")
                          .OnDelete(DeleteBehavior.Cascade),
                    r => r.HasOne(typeof(Author))
                          .WithMany()
                          .HasForeignKey("LikedId")
                          .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasKey("LikesId", "LikedId"));

            modelBuilder.Entity<Author>()
                .HasMany(a => a.Following)
                .WithMany(a => a.Followers)
                .UsingEntity(
                    "Followers",
                    j => j.HasOne(typeof(Author))
                          .WithMany()
                          .HasForeignKey("FollowingId")
                          .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne(typeof(Author))
                          .WithMany()
                          .HasForeignKey("FollowerId")
                          .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasKey("FollowingId", "FollowerId"));

            // Configure relationships for Blab
            modelBuilder.Entity<Blab>()
                .HasOne(b => b.Author)
                .WithMany(u => u.Blabs)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure relationships for Comment
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Blab)
                .WithMany(b => b.Comments)
                .HasForeignKey(c => c.BlabId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.AuthorId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Application Users
            modelBuilder.Entity<ApplicationUser>().HasData(
                new ApplicationUser { Id = "user1", UserName = "user1", Email = "user1@example.com", NormalizedUserName = "USER1", NormalizedEmail = "USER1@EXAMPLE.COM", EmailConfirmed = true },
                new ApplicationUser { Id = "user2", UserName = "user2", Email = "user2@example.com", NormalizedUserName = "USER2", NormalizedEmail = "USER2@EXAMPLE.COM", EmailConfirmed = true }
            );

            // Authors
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, ApplicationUserId = "user1", Handle = "Author1", DisplayName = "First Author" },
                new Author { Id = 2, ApplicationUserId = "user2", Handle = "Author2", DisplayName = "Second Author" }
            );

            // Blabs
            modelBuilder.Entity<Blab>().HasData(
                new Blab { Id = 1, AuthorId = 1, Body = "First blab by Author1" },
                new Blab { Id = 2, AuthorId = 2, Body = "First blab by Author2" },
                new Blab { Id = 3, AuthorId = 1, Body = "Second blab by Author1" },
                new Blab { Id = 4, AuthorId = 2, Body = "Second blab by Author2" },
                new Blab { Id = 5, AuthorId = 1, Body = "Third blab by Author1" },
                new Blab { Id = 6, AuthorId = 2, Body = "Third blab by Author2" }
            );

            // Comments
            modelBuilder.Entity<Comment>().HasData(
                new Comment { Id = 1, BlabId = 1, AuthorId = 2, Body = "Comment by Author2 on Blab1" },
                new Comment { Id = 2, BlabId = 2, AuthorId = 1, Body = "Comment by Author1 on Blab2" },
                new Comment { Id = 3, BlabId = 1, AuthorId = 1, Body = "Reply by Author1 on Blab1", ParentId = 1 },
                new Comment { Id = 4, BlabId = 5, AuthorId = 1, Body = "Comment by Author1 on Blab5" },
                new Comment { Id = 5, BlabId = 6, AuthorId = 2, Body = "Comment by Author2 on Blab6" },
                new Comment { Id = 6, BlabId = 5, AuthorId = 2, Body = "Reply by Author2 on Blab5", ParentId = 4 }
            );

            // Likes (Using shadow table configuration)
            modelBuilder.Entity("Likes").HasData(
                new { LikedId = 1, LikesId = 2 }, // Author1 likes Blab2
                new { LikedId = 2, LikesId = 1 }  // Author2 likes Blab1
            );

            // Followers (Using shadow table configuration)
            modelBuilder.Entity("Followers").HasData(
                new { FollowerId = 1, FollowingId = 2 }, // Author1 follows Author2
                new { FollowerId = 2, FollowingId = 1 }  // Author2 follows Author1
            );

        }
    }
}