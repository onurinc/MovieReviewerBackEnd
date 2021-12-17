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
            var connectionString = "server=DESKTOP-NCSPB7A;database=MovieReviewer;trusted_connection=true;";


            dbContextBuilder.UseSqlServer(connectionString);

            return new MovieReviewerContext(dbContextBuilder.Options);
        }
    }
}
