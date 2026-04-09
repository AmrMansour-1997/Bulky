using System.Diagnostics;
using System.Security.Claims;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _UnitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork UnitOfWork)
        {
            _logger = logger;
            _UnitOfWork = UnitOfWork;
        }

        public IActionResult Index()
        {
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userid != null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart,
                     _UnitOfWork.ShoppingCart.GetAll(C => C.ApplicationUserID == userid).Count());
            }
            IEnumerable<Product> ProductsList = _UnitOfWork.Product.GetAll(IncludeProperties: "category,ProductImages");
            return View(ProductsList);
        }

        public IActionResult Details(int Productid)
        {
            ShoppingCart cart = new ShoppingCart()
            {
                Product = _UnitOfWork.Product.Get(P => P.ID == Productid, "category,ProductImages"),
                Count = 1,
                ProductID = Productid
            };

            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart ShoppingCartObj)
        {
            //var ClaimsIdentity = (ClaimsIdentity)User.Identity;
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ShoppingCartObj.ApplicationUserID = userid;



            ShoppingCart CartFromDB = _UnitOfWork.ShoppingCart.Get(C => C.ApplicationUserID == ShoppingCartObj.ApplicationUserID &&
            C.ProductID == ShoppingCartObj.ProductID);

            if (CartFromDB != null)
            {
                //Update ShoppingCart
                CartFromDB.Count += ShoppingCartObj.Count;
                _UnitOfWork.ShoppingCart.Update(CartFromDB);
                TempData["success"] = "Cart Updated Successfully";
                _UnitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                     _UnitOfWork.ShoppingCart.GetAll(C => C.ApplicationUserID == ShoppingCartObj.ApplicationUserID).Count());
            }
            else
            {
                //Add New ShoppingCart
                TempData["success"] = "Cart Updated Successfully";
                _UnitOfWork.ShoppingCart.Add(ShoppingCartObj);
                _UnitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _UnitOfWork.ShoppingCart.GetAll(C => C.ApplicationUserID == ShoppingCartObj.ApplicationUserID).Count());

                
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
