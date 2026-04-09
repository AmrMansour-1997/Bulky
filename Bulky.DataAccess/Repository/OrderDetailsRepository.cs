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
    public class OrderDetailsRepository : Repository<OrderDetails>, IOrderDetailsRepository
    {
        private readonly AppDbContext _DB;
        public OrderDetailsRepository(AppDbContext DB) : base(DB)
        {
            _DB = DB;
        }

        public void Update(OrderDetails OrderDetailsOBJ)
        {
            _DB.OrdersDetails.Update(OrderDetailsOBJ);
        }
    }
}
