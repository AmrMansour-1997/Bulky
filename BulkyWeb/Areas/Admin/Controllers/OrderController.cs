using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;

        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IUnitOfWork UnitOfWork) 
        {
            _UnitOfWork = UnitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int OrderId)
        {
            OrderVM = new()
            {
                OrderHeader = _UnitOfWork.OrderHeader.Get(U => U.Id == OrderId, IncludeProperties: "ApplicationUser"),
                OrderDetails = _UnitOfWork.OrderDetails.GetAll(U => U.OrderHeaderID == OrderId, IncludeProperties: "Product")
            };

            return View(OrderVM);
        }

        [HttpPost]
        [Authorize(Roles =SD.Role_Admin+","+SD.Role_Employee)]
        public IActionResult UpdateOrderDetails()
        {
            var OrderHeaderFromDB = _UnitOfWork.OrderHeader.Get(O => O.Id == OrderVM.OrderHeader.Id);
            OrderHeaderFromDB.Name = OrderVM.OrderHeader.Name;
            OrderHeaderFromDB.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            OrderHeaderFromDB.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            OrderHeaderFromDB.PostalCode = OrderVM.OrderHeader.PostalCode;
            OrderHeaderFromDB.State = OrderVM.OrderHeader.State;
            if(!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier))
            {
                OrderHeaderFromDB.Carrier = OrderVM.OrderHeader.Carrier;

            }
            if (!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber))
            {
                OrderHeaderFromDB.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;

            }
            _UnitOfWork.OrderHeader.Update(OrderHeaderFromDB);
            _UnitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully";
            return RedirectToAction("Details", new { OrderId = OrderHeaderFromDB.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            _UnitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusProcessing);
            _UnitOfWork.Save();
            TempData["Success"] = "Order Details Updated Successfully";
            return RedirectToAction("Details", new { OrderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            OrderHeader OrderHeader = _UnitOfWork.OrderHeader.Get(O=>O.Id == OrderVM.OrderHeader.Id);
            OrderHeader.ShippingDate = DateTime.Now;
            OrderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            OrderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            if(OrderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                OrderHeader.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }

            _UnitOfWork.OrderHeader.Update(OrderHeader);
            _UnitOfWork.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusShipped);
            _UnitOfWork.Save();
            TempData["Success"] = "Order Shipped Successfully";
            return RedirectToAction("Details", new { OrderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            OrderHeader OrderHeader = _UnitOfWork.OrderHeader.Get(O=>O.Id==OrderVM.OrderHeader.Id);
            if(OrderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = OrderHeader.PaymentIntentID
                };

                var service = new RefundService();
                Refund Refund = service.Create(options);
                _UnitOfWork.OrderHeader.UpdateStatus(OrderHeader.Id, SD.StatusRefunded, SD.StatusRefunded);
            }
            else
            {
                _UnitOfWork.OrderHeader.UpdateStatus(OrderHeader.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
            _UnitOfWork.Save();
            TempData["Success"] = "Order Cancelled Successfully";
            return RedirectToAction("Details", new { OrderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [ActionName("Details")]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult PayNowForDelayedPayment()
        {
            OrderVM.OrderHeader = _UnitOfWork.OrderHeader.Get(U => U.Id == OrderVM.OrderHeader.Id, IncludeProperties: "ApplicationUser");
            OrderVM.OrderDetails = _UnitOfWork.OrderDetails.GetAll(U => U.OrderHeaderID == OrderVM.OrderHeader.Id, IncludeProperties: "Product");

            var domain = "https://localhost:7041/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                SuccessUrl = domain + $"Admin/Order/PaymentConfirmation?OrderHeaderid={OrderVM.OrderHeader.Id}",
                CancelUrl = domain + $"Admin/Order/details?orderid{OrderVM.OrderHeader.Id}",
                LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                Mode = "payment",
            };

            foreach (var item in OrderVM.OrderDetails)
            {
                var sessionlineitem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)item.Price * 100,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title
                        }
                    },
                    Quantity = item.Count
                };
                options.LineItems.Add(sessionlineitem);
            }

            var service = new Stripe.Checkout.SessionService();
            Stripe.Checkout.Session session = service.Create(options);
            _UnitOfWork.OrderHeader.UpdateStripePaymentID(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _UnitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult PaymentConfirmation(int OrderHeaderid)
        {
            OrderHeader OrderHeaderFromDB = _UnitOfWork.OrderHeader.Get(H => H.Id == OrderHeaderid, IncludeProperties: "ApplicationUser");
            if (OrderHeaderFromDB.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                //then its a company user
                var service = new SessionService();
                Session session = service.Get(OrderHeaderFromDB.SessionID);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _UnitOfWork.OrderHeader.UpdateStripePaymentID(OrderHeaderid, session.Id, session.PaymentIntentId);
                    _UnitOfWork.OrderHeader.UpdateStatus(OrderHeaderid, OrderHeaderFromDB.OrderStatus, SD.PaymentStatusApproved);
                    _UnitOfWork.Save();
                }

            }
            return View(OrderHeaderid);
        }
        #region Api Calls

        [HttpGet]
        public IActionResult Getall(string status)
        {
            IEnumerable<OrderHeader> OrderHeadersObj; 

            if(User.IsInRole(SD.Role_Employee) || User.IsInRole(SD.Role_Admin))
            {
                OrderHeadersObj = _UnitOfWork.OrderHeader.GetAll(IncludeProperties: "ApplicationUser").ToList();
            }
            else
            {
                string UserID = User.FindFirstValue(ClaimTypes.NameIdentifier);

                OrderHeadersObj = _UnitOfWork.OrderHeader.GetAll(O => O.ApplicationUserID == UserID, IncludeProperties: "ApplicationUser");
            }

            switch(status)
            {
                case "inprocess":
                    OrderHeadersObj = OrderHeadersObj.Where(U => U.OrderStatus == SD.StatusProcessing);
                    break;
                case "pending":
                    OrderHeadersObj = OrderHeadersObj.Where(U => U.OrderStatus == SD.StatusPending); 
                    break;
                case "completed":
                    OrderHeadersObj = OrderHeadersObj.Where(U => U.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    OrderHeadersObj = OrderHeadersObj.Where(U => U.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    break;

            }
            return Json(new { data = OrderHeadersObj });
        }

        #endregion
    }
}
