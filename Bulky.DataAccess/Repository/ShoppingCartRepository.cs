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
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly AppDbContext _DB;
        public ShoppingCartRepository(AppDbContext DB) : base(DB)
        {
            _DB = DB;
        }

        public void Update(ShoppingCart ShoppingCartObj)
        {
            _DB.ShoppingCarts.Update(ShoppingCartObj);
        }
    }
}
