using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MR.DataAccessLayer.Interfaces
{
   public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();

        Task<T> GetById(Guid id);

        Task<bool> Add(T entity);

        Task<bool> Upsert(T entity);

        Task<bool> Delete(Guid id);

        Task<bool> Update(T entity);

    }
}
