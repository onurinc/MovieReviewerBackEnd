using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR.DataAccessLayer.Context
{
    public class MovieReviewerContext:DbContext
    {
        public MovieReviewerContext(DbContextOptions<MovieReviewerContext> options) 
            : base(options)
        {

        }

        public DbSet<Entities.Comment> Comments { get; set; }

    }
}
