using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR.DataAccessLayer.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }

        IReviewRepository Reviews { get; }
        Task CompleteAsync();
    }
}
