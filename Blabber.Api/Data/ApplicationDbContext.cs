using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Blabber.Api.Models;

namespace Blabber.Api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Blab> Blabs { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships for ApplicationUser
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(au => au.Author)
                .WithOne(a => a.ApplicationUser)
                .HasForeignKey<Author>(a => a.ApplicationUserId);

            // Configure relationships for Author
            modelBuilder.Entity<Author>()
                .HasMany(a => a.Likes)
                .WithMany(b => b.Liked)
                .UsingEntity(j => j.ToTable("Likes"));

            modelBuilder.Entity<Author>()
                .HasMany(a => a.Following)
                .WithMany(a => a.Followers)
                .UsingEntity(j => j.ToTable("Followers"));

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
    }
}
