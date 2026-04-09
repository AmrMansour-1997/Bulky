using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }
        public IActionResult Index()
        {
            string UserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = _UnitOfWork.ShoppingCart.GetAll(C => C.ApplicationUserID == UserID, IncludeProperties: "Product"),
                OrderHeader = new()

            };
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                IEnumerable<ProductImage> ProductImages = _UnitOfWork.ProductImage.GetAll();
                cart.Product.ProductImages = ProductImages.Where(I => I.ProductID == cart.Product.ID).ToList();
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }

        public IActionResult Increment(int CartId)
        {
            ShoppingCart ShoppingCartOBJ = _UnitOfWork.ShoppingCart.Get(C => C.ID == CartId);

            ShoppingCartOBJ.Count += 1;
            _UnitOfWork.ShoppingCart.Update(ShoppingCartOBJ);
            _UnitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Decrement(int CartId)
        {
            ShoppingCart ShoppingCartOBJ = _UnitOfWork.ShoppingCart.Get(C => C.ID == CartId);
            string UserID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (ShoppingCartOBJ.Count <= 1)
            {
                //remove from cart
                _UnitOfWork.ShoppingCart.Remove(ShoppingCartOBJ);
                _UnitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
        _UnitOfWork.ShoppingCart.GetAll(C => C.ApplicationUserID == UserID).Count());
            }
            else
            {
                ShoppingCartOBJ.Count -= 1;
                _UnitOfWork.ShoppingCart.Update(ShoppingCartOBJ);
                _UnitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));

        }

        public IActionResult Remove(int CartId)
        {
            string UserID = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShoppingCart ShoppingCartOBJ = _UnitOfWork.ShoppingCart.Get(C => C.ID == CartId);

            //remove from cart
            _UnitOfWork.ShoppingCart.Remove(ShoppingCartOBJ);
            _UnitOfWork.Save();
            HttpContext.Session.SetInt32(SD.SessionCart,
        _UnitOfWork.ShoppingCart.GetAll(C => C.ApplicationUserID == UserID).Count());


            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            string UserID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = _UnitOfWork.ShoppingCart.GetAll(C => C.ApplicationUserID == UserID, IncludeProperties: "Product"),
                OrderHeader = new()

            };
            ShoppingCartVM.OrderHeader.ApplicationUser = _UnitOfWork.ApplicationUser.Get(A => A.Id == UserID);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }
            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            string UserID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ShoppingCartVM.ShoppingCartList = _UnitOfWork.ShoppingCart.GetAll(C => C.ApplicationUserID == UserID, IncludeProperties: "Product");
            ApplicationUser ApplicationUser = _UnitOfWork.ApplicationUser.Get(A => A.Id == UserID);
            ShoppingCartVM.OrderHeader.ApplicationUserID = UserID;
            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (ApplicationUser.CompanyID.GetValueOrDefault() == 0)
            {
                // then its a regular customer and not a company customer

                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            }
            else
            {
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
            }

            _UnitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _UnitOfWork.Save();

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetails OrderDetails = new()
                {
                    OrderHeaderID = ShoppingCartVM.OrderHeader.Id,
                    ProductID = cart.ProductID,
                    Count = cart.Count,
                    Price = cart.Price
                };
                _UnitOfWork.OrderDetails.Add(OrderDetails);
                _UnitOfWork.Save();
            }

            if (ApplicationUser.CompanyID.GetValueOrDefault() == 0)
            {
                // then its a regular customer and not a company customer
                //stripe logic

                var domain = "https://localhost:7041/";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain + $"Customer/Cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain + "Customer/Cart/Index",
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in ShoppingCartVM.ShoppingCartList)
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
                _UnitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _UnitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader OrderHeaderFromDB = _UnitOfWork.OrderHeader.Get(H => H.Id == id, IncludeProperties: "ApplicationUser");
            if (OrderHeaderFromDB.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                //then its a customer
                var service = new SessionService();
                Session session = service.Get(OrderHeaderFromDB.SessionID);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _UnitOfWork.OrderHeader.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    _UnitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _UnitOfWork.Save();
                }
                HttpContext.Session.Clear();
            }
            List<ShoppingCart> SCL = _UnitOfWork.ShoppingCart.GetAll(U =>
            U.ApplicationUserID == OrderHeaderFromDB.ApplicationUserID).ToList();
            _UnitOfWork.ShoppingCart.RemoveRange(SCL);
            _UnitOfWork.Save();

            return View(id);
        }
        private double GetPriceBasedOnQuantity(ShoppingCart cart)
        {
            if (cart.Count <= 50)
            {
                return cart.Product.Price;
            }
            else if (cart.Count <= 100)
            {
                return cart.Product.Price50;
            }
            else
            {
                return cart.Product.Price100;
            }
        }
    }
}
