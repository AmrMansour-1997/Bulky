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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly AppDbContext _DB;
        public ProductRepository(AppDbContext DB) : base(DB) 
        {
            _DB = DB;
        }

        public void Update(Product product)
        {
            var ProductFromDB = _DB.Products.FirstOrDefault(p => p.ID == product.ID);
            if (ProductFromDB != null)
            {
                ProductFromDB.Title = product.Title;
                ProductFromDB.Description = product.Description;
                ProductFromDB.ISBN = product.ISBN;
                ProductFromDB.Author = product.Author;
                ProductFromDB.Price100 = product.Price100;
                ProductFromDB.Price50 = product.Price50;
                ProductFromDB.Price = product.Price;
                ProductFromDB.ListPrice = product.ListPrice;
                ProductFromDB.CategoryID = product.CategoryID;

                //if(ProductFromDB.ImageUrl != null)
                //{
                //    ProductFromDB.ImageUrl = product.ImageUrl;
                //}

            }
        }
    }
}
