using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace weatherapi.Entities.Declarations.Generic
{
    public interface IGenericRepository<T> where T : class
    {
        //Core databae operations
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task CreateAsync(T entity);
        void Update(T entity);


        void DeleteBatch(IEnumerable<T> entities);
        void Delete(T entity);


        Task<int> CountAsync();

        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);

        //returns a queryable list of entities
        IQueryable<T> GetAll();
        IQueryable<T> GetByBatchAsync(int skip, int take);
    }
}
