using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BulkyBook.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _DB;
        private readonly DbSet<T> _dbSet;
        public Repository(AppDbContext DB)
        {
            _DB = DB;
            this._dbSet = _DB.Set<T>();
            //_DB.Products.Include(P => P.category).Include(P => P.CategoryID);
        }
        public void Add(T Entity)
        {
            _dbSet.Add(Entity);
        }

        public T Get(Expression<Func<T, bool>> Filter, string? IncludeProperties = null, bool Tracked = false)
        {
            IQueryable<T> query;

            if(Tracked)
            {
                query = _dbSet;

            }
            else
            {
                query = _dbSet.AsNoTracking();

            }
            query = query.Where(Filter);
            if (!string.IsNullOrEmpty(IncludeProperties))
            {
                foreach (var includeproperty in IncludeProperties.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeproperty);
                }
            }


            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? Filter, string? IncludeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if(Filter != null)
            {
                query = query.Where(Filter);
            }

            if (!string.IsNullOrEmpty(IncludeProperties))
            {
                foreach(var includeproperty in IncludeProperties.Split(",",StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeproperty);
                }
            }

            return query.ToList();
        }

        public void Remove(T Entity)
        {
            _dbSet.Remove(Entity);
        }

        public void RemoveRange(IEnumerable<T> Entities)
        {
            _dbSet.RemoveRange(Entities);
        }
    }
}
