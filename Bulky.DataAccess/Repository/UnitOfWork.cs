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
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _DB;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailsRepository OrderDetails { get; private set; }
        public IProductImageRepository ProductImage { get; private set; }

        public UnitOfWork(AppDbContext DB)
        {
            _DB = DB;
            ProductImage = new ProductImageRepository(_DB);
            Category = new CategoryRepository(_DB);
            Product = new ProductRepository(_DB);
            Company = new CompanyRepository(_DB);
            ShoppingCart = new ShoppingCartRepository(_DB);
            ApplicationUser = new ApplicationUserRepository(_DB);
            OrderHeader = new OrderHeaderRepository(_DB);
            OrderDetails = new OrderDetailsRepository(_DB);
        }

        public void Save()
        {
            _DB.SaveChanges();
        }
    }
}
