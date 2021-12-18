using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MR.DataAccessLayer.Context;

namespace MR.DataAccessLayer.ContextFactory
{
    public class MovieReviewerContextFactory : IDesignTimeDbContextFactory<MovieReviewerContext>
    {
        public MovieReviewerContext CreateDbContext(string[] args)
        {
            var dbContextBuilder = new DbContextOptionsBuilder<MovieReviewerContext>();
            var connectionString = "Server=localhost\\SQLEXPRESS;Database=MovieReviewer;Trusted_Connection=True;";


            dbContextBuilder.UseSqlServer(connectionString);

            return new MovieReviewerContext(dbContextBuilder.Options);
        }
    }
}
