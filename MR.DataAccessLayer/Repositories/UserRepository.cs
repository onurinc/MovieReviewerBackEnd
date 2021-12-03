using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MR.DataAccessLayer.Context;
using MR.DataAccessLayer.Entities;
using MR.DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR.DataAccessLayer.Repositories
{
    class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(
            MovieReviewerContext movieReviewerContext, ILogger logger)
            : base (movieReviewerContext, logger)
        {
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            try
            {
                return await dbSet.Where(x => x.Status == 1).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Repo} GetAll method has generated an error", typeof(UserRepository));
                    return new List<User>();
            }
        }

    }
}
