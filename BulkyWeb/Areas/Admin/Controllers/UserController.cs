using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<IdentityUser> _UserManager;
        public UserController(AppDbContext db, UserManager<IdentityUser> UserManager) //Here Asp.net get the Db options and object from DI Container
        {
            _db = db;
            _UserManager = UserManager;
        }
        public IActionResult Index()
        {       
            return View();
        }

        public IActionResult RoleManagement(string UserID)
        {
            RoleManagementVM RoleManagementVM = new()
            {
                ApplicationUser = _db.ApplicationUsers.Include(U => U.Company).FirstOrDefault(U => U.Id == UserID),
                CompaniesList = _db.Companies.ToList().Select(
                    C => new SelectListItem
                    {
                        Text = C.Name,
                        Value = C.Id.ToString()
                    }),
                RolesList = _db.Roles.ToList().Select(
                    R => new SelectListItem
                    {
                        Text = R.Name,
                        Value = R.Name
                    })
            };
            var RoleID = _db.UserRoles.FirstOrDefault(U => U.UserId == UserID).RoleId;
            RoleManagementVM.ApplicationUser.Role = _db.Roles.FirstOrDefault(U => U.Id == RoleID).Name;
            if (RoleManagementVM.ApplicationUser.CompanyID != null)
            {
                RoleManagementVM.Company = _db.Companies.FirstOrDefault(C => C.Id == RoleManagementVM.ApplicationUser.CompanyID).Name;
            }
            return View(RoleManagementVM);
        }

        [HttpPost]
        public IActionResult RoleManagement(RoleManagementVM RoleManagementVM)
        {
            string RoleID = _db.UserRoles.FirstOrDefault(R => R.UserId == RoleManagementVM.ApplicationUser.Id).RoleId;
            string OldRole = _db.Roles.FirstOrDefault(R => R.Id == RoleID).Name;

            if(!(RoleManagementVM.ApplicationUser.Role == OldRole))
            {
                ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(A => A.Id == RoleManagementVM.ApplicationUser.Id);
                if(RoleManagementVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyID = RoleManagementVM.ApplicationUser.CompanyID;
                }
                if(OldRole == SD.Role_Company)
                {
                    applicationUser.CompanyID = null;
                }
                _db.SaveChanges();
                _UserManager.RemoveFromRoleAsync(applicationUser, OldRole).GetAwaiter().GetResult();
                _UserManager.AddToRoleAsync(applicationUser, RoleManagementVM.ApplicationUser.Role).GetAwaiter().GetResult();

            }
            return RedirectToAction("Index");
        }

        #region Api Calls

        [HttpGet]
        public IActionResult Getall()
        {
            List<ApplicationUser> Users = _db.ApplicationUsers.Include(U=>U.Company).ToList();

            var Roles = _db.Roles.ToList();
            var UserRoles = _db.UserRoles.ToList();

            foreach(var user in Users)
            {
                var Roleid = UserRoles.FirstOrDefault(U => U.UserId == user.Id).RoleId;
                user.Role = Roles.FirstOrDefault(U => U.Id == Roleid).Name;
                if(user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }

            return Json(new {data = Users });
        }

        [HttpPost]
        public IActionResult LockUnlock(string id)
        {
            var ObjFromDB = _db.ApplicationUsers.FirstOrDefault(u=>u.Id == id);

            if(ObjFromDB == null)
            {
                return Json(new { success = false, message = "Error While Locking/Unlocking" });

            }
            if (ObjFromDB.LockoutEnd != null && ObjFromDB.LockoutEnd > DateTime.Now)
            {
                //then means its locked so we will unlock it
                ObjFromDB.LockoutEnd = DateTime.Now;
                _db.SaveChanges();
                return Json(new { success = true, message = "Account Unlocked Succefully !" });
            }
            else
            {
                //then means its unlocked so we will lock it
                ObjFromDB.LockoutEnd = DateTime.Now.AddYears(10);
                _db.SaveChanges();
                return Json(new { success = true, message = "Account Locked Succefully !" });
            }

            //if (ObjFromDB.LockoutEnd != null && ObjFromDB.LockoutEnd > DateTime.Now)
            //{
            //    ObjFromDB.LockoutEnd = DateTime.Now;
            //}
            //else
            //{
            //    ObjFromDB.LockoutEnd = DateTime.Now.AddYears(10);
            //}
            //_db.SaveChanges();
            //return Json(new { success = true, message = "Delete Tmam" });
        }

        #endregion

    }
}
