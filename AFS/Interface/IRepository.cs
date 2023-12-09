using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AFS.Interface
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAll(
            Expression<Func<T, bool>>? expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            List<string>? includes = null
            );

        Task<T> Get(Expression<Func<T, bool>> expression, List<string>? includes = null);

        Task Insert(T entity);
    }
}
