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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly AppDbContext _DB;
        public OrderHeaderRepository(AppDbContext DB) : base(DB)
        {
            _DB = DB;
        }

        public void Update(OrderHeader OrderHeaderOBJ)
        {
            _DB.OrdersHeaders.Update(OrderHeaderOBJ);
        }

        public void UpdateStatus(int ID, string OrderStatus, string? PaymentStatus = null)
        {
            var orderHeaderFromDB = _DB.OrdersHeaders.FirstOrDefault(H => H.Id == ID);
            if (orderHeaderFromDB != null)
            {
                orderHeaderFromDB.OrderStatus = OrderStatus;
                if (!string.IsNullOrEmpty(PaymentStatus))
                {
                    orderHeaderFromDB.PaymentStatus = PaymentStatus;
                }
            }
        }

        public void UpdateStripePaymentID(int ID, string SessionID, string PaymentIntentID)
        {
            var orderHeaderFromDB = _DB.OrdersHeaders.FirstOrDefault(H => H.Id == ID);

            if (!string.IsNullOrEmpty(SessionID))
            {
                orderHeaderFromDB.SessionID = SessionID;
            }
            if (!string.IsNullOrEmpty(PaymentIntentID))
            {
                orderHeaderFromDB.PaymentIntentID = PaymentIntentID;
                orderHeaderFromDB.PaymentDate = DateTime.Now;
            }
        }
    }
}
