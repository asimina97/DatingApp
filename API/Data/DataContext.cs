using API.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<AppUser>  Users  { get; set; }
        public DbSet<UserLike> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserLike>()
                //represents the primary key for this table
                .HasKey(k=>new{k.SourceUserId,k.TargetUserId});

            builder.Entity<UserLike>()
                .HasOne(s=>s.SourceUser)
                .WithMany(l=>l.LikedUsers)
                .HasForeignKey(s=>s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);

                
            builder.Entity<UserLike>()
                .HasOne(s=>s.TargetUser)
                .WithMany(l=>l.LikedByUsers)
                .HasForeignKey(s=>s.TargetUserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}