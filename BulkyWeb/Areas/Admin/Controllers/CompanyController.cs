using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        public CompanyController(IUnitOfWork UnitOfWork) //Here Asp.net get the Db options and object from DI Container
        {
            _UnitOfWork = UnitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> Companys = _UnitOfWork.Company.GetAll().ToList();
            
            return View(Companys);
        }
        public IActionResult Upsert(int? ID)
        {

            if(ID == null || ID <= 0)
            {
                //Create
                return View(new Company());
            }
            else
            {
                //Update
                Company CompanyObj = _UnitOfWork.Company.Get(P => P.Id == ID);
                return View(CompanyObj);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company CompanyObj)
        {

            if (ModelState.IsValid)
            {               
               if(CompanyObj.Id == null || CompanyObj.Id == 0)
                {
                    _UnitOfWork.Company.Add(CompanyObj);
                    TempData["Success"] = "Company Added Successfully!";
                }
               else
                {
                    _UnitOfWork.Company.Update(CompanyObj);
                    TempData["Success"] = "Company Updated Successfully!";
                }
                _UnitOfWork.Save();
                return RedirectToAction("Index");
            }
            else
            {
                return View(CompanyObj);
            }

        }

        //public IActionResult Delete(int? ID)
        //{
        //    if (ID == null || ID <= 0)
        //    {
        //        return NotFound();
        //    }
        //    Company? Company = _UnitOfWork.Company.Get(c => c.ID == ID);
        //    if (Company == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(Company);
        //}

        //[HttpPost,ActionName("Delete")]
        //public IActionResult DeletePOST(int? ID)
        //{
        //    Company? Company = _UnitOfWork.Company.Get(c => c.ID == ID);
        //    if (Company == null)
        //    {
        //        return NotFound();
        //    }
        //    _UnitOfWork.Company.Remove(Company);
        //    _UnitOfWork.Save();
        //    TempData["Success"] = "Company Deleted Successfully !";
        //    return RedirectToAction("Index");
        //}

        #region Api Calls

        [HttpGet]
        public IActionResult Getall()
        {
            List<Company> Companys = _UnitOfWork.Company.GetAll().ToList();
            return Json(new {data = Companys});
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Company CompanyToDelete = _UnitOfWork.Company.Get(P =>P.Id == id);

            if(CompanyToDelete == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }

            _UnitOfWork.Company.Remove(CompanyToDelete);
            _UnitOfWork.Save();

            return Json(new { success = true, message = "Deleted Successfully" });
        }

        #endregion

    }
}
