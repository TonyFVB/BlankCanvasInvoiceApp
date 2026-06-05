using BlankCanvasApp.Application.Interfaces;
using BlankCanvasApp.Domain.Common;
using BlankCanvasApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace BlankCanvasApp.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly BcDContext _context;
        private readonly DbSet<T> _entity;

        public BaseRepository(BcDContext context) 
        {
            _context = context;
            _entity = _context.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _entity.AsNoTracking().ToListAsync();
        }
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _entity.AsNoTracking().Where(predicate).ToListAsync();
        }
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _entity.FindAsync(id);
        }
        public async Task<bool> AddAsync(T entity)
        {
            await _entity.AddAsync(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateAsync(T entity)
        {
            _entity.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _entity.FindAsync(id);
            if (entity == null) return false;
            _entity.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await _entity.FindAsync(id);
            if (entity == null) return false;
            entity.GetType().GetProperty("IsDeleted")?.SetValue(entity, true);
            _entity.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        public IQueryable<T> Query() => _entity.AsQueryable();

        public async Task<bool> BulkInsertAsync(IEnumerable<T> entities)
        {
            await _entity.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
