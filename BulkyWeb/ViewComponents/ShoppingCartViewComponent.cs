using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _UnitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork UnitOfWork)
        {
            _UnitOfWork = UnitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var userid = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (userid != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {
                    HttpContext.Session.SetInt32(SD.SessionCart,
                      _UnitOfWork.ShoppingCart.GetAll(C => C.ApplicationUserID == userid.Value).Count());

                }
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
