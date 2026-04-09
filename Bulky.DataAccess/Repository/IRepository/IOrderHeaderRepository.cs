using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulkyBook.Models;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        public void Update(OrderHeader OrderHeaderOBJ);
        public void UpdateStatus(int ID, string OrderStatus, string? PaymentStatus = null);
        public void UpdateStripePaymentID(int ID, string SessionID, string PaymentIntentID);
    }
}
