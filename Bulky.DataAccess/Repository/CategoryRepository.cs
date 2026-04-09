using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _DB;
        public CategoryRepository(AppDbContext DB) : base(DB)
        {
            _DB = DB;
        }

        public void Update(Category category)
        {
            _DB.Categories.Update(category);
        }
    }
}
