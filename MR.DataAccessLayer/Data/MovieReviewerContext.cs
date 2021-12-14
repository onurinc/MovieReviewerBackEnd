using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MR.DataAccessLayer.Entities;

namespace MR.DataAccessLayer.Context
{
    public class MovieReviewerContext: IdentityDbContext
    {

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public MovieReviewerContext(DbContextOptions<MovieReviewerContext> options) : base(options)
        {

        }


    }
}
