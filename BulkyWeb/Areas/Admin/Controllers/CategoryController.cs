using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        public CategoryController(IUnitOfWork UnitOfWork) //Here Asp.net get the Db options and object from DI Container
        {
            _UnitOfWork = UnitOfWork;
        }
        public IActionResult Index()
        {
            List<Category> categories = _UnitOfWork.Category.GetAll().ToList();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "Display Order Can not be Same as Name"); //Custom Error Added 
            }
            if (ModelState.IsValid)
            {
                _UnitOfWork.Category.Add(obj);
                _UnitOfWork.Save();
                TempData["Success"] = "Category Added Successfully !";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? ID)
        {
            if(ID == null || ID <=0)
            {
                return NotFound();
            }
            Category? category = _UnitOfWork.Category.Get(c => c.ID == ID);
            if(category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _UnitOfWork.Category.Update(obj);
                _UnitOfWork.Save();
                TempData["Success"] = "Category Edited Successfully !";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? ID)
        {
            if (ID == null || ID <= 0)
            {
                return NotFound();
            }
            Category? category = _UnitOfWork.Category.Get(c => c.ID == ID);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePOST(int? ID)
        {
            Category? category = _UnitOfWork.Category.Get(c => c.ID == ID);
            if (category == null)
            {
                return NotFound();
            }
            _UnitOfWork.Category.Remove(category);
            _UnitOfWork.Save();
            TempData["Success"] = "Category Deleted Successfully !";
            return RedirectToAction("Index");
        }
    }
}
