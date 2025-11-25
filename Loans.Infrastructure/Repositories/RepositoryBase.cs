using Loans.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Loans.Infrastructure.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected RepositoryContext Context { get; }
        protected RepositoryBase(RepositoryContext context)
        {
            Context = context;
        }

        public IQueryable<T> FindAll(bool trackChanges)
        {
            return trackChanges ? Context.Set<T>() : Context.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges)
        {
            return trackChanges ? Context.Set<T>().Where(expression) : Context.Set<T>().Where(expression).AsNoTracking();
        }

        public void Create(T entity)
        {
            Context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            Context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            Context.Set<T>().Remove(entity);
        }
    }
}
