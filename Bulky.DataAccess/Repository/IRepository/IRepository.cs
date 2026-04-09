using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        // T-Category or Product
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? Filter=null, string? IncludeProperties = null);
        T Get(Expression<Func<T, bool>> Filter, string? IncludeProperties = null, bool Tracked = false);
        void Add(T Entity);
        void Remove(T Entity);
        void RemoveRange(IEnumerable<T> Entity);
    }
}
