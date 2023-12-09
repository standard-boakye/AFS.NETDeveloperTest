using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AFS.Data;
using AFS.Interface;
using Microsoft.EntityFrameworkCore;

namespace AFS.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _appDbContext;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _dbSet = _appDbContext.Set<T>();
        }

        public async Task<T> Get(Expression<Func<T, bool>> expression, List<string> includes = null)
        {
            IQueryable<T> query = _dbSet;
            if (includes != null)
            {
                foreach (var includeProperty in includes)
                    query = query.Include(includeProperty);
            }
            return await query.AsNoTracking().FirstOrDefaultAsync(expression);
        }

        public async Task<List<T>> GetAll(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<string> includes = null)
        {
            IQueryable<T> query = _dbSet;

            if (expression != null)
                query = query.Where(expression);

            if (includes != null)
            {
                foreach (var includeProperty in includes)
                    query = query.Include(includeProperty);
            }

            if (orderBy != null)
                query = orderBy(query);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task Insert(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _appDbContext.SaveChangesAsync();

        }
    }
}
