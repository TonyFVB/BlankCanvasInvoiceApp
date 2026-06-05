using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BlankCanvasApp.Application.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetByIdAsync(int id);
        Task<bool> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);

        Task<bool> SoftDeleteAsync(int id);
        IQueryable<T> Query();
        Task<bool> BulkInsertAsync(IEnumerable<T> entities);

    }
}
