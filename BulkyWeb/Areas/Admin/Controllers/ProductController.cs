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
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IWebHostEnvironment _WebHostEnvironment;//used to access wwwroot to save our static files
        public ProductController(IUnitOfWork UnitOfWork, IWebHostEnvironment webHostEnvironment) //Here Asp.net get the Db options and object from DI Container
        {
            _UnitOfWork = UnitOfWork;
            _WebHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> Products = _UnitOfWork.Product.GetAll(IncludeProperties: "category").ToList();

            return View(Products);
        }
        public IActionResult Upsert(int? ID)
        {

            ProductVM ProductVM = new()
            {
                product = new Product(),
                Categories = _UnitOfWork.Category.GetAll().
                Select(C => new SelectListItem
                {
                    Text = C.Name,
                    Value = C.ID.ToString()
                })
            };

            if (ID == null || ID <= 0)
            {
                //Create
                return View(ProductVM);
            }
            else
            {
                //Update
                ProductVM.product = _UnitOfWork.Product.Get(P => P.ID == ID,IncludeProperties:"ProductImages");
                return View(ProductVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM ProductVM, List<IFormFile> files)
        {

            if (ModelState.IsValid)
            {
                if (ProductVM.product.ID == null || ProductVM.product.ID == 0)
                {
                    _UnitOfWork.Product.Add(ProductVM.product);
                    TempData["Success"] = "Product Added Successfully!";
                }
                else
                {
                    _UnitOfWork.Product.Update(ProductVM.product);
                    TempData["Success"] = "Product Updated Successfully!";
                }
                _UnitOfWork.Save();

                string wwwRootPath = _WebHostEnvironment.WebRootPath;

                if (files != null)
                {

                    foreach (IFormFile file in files)
                    {
                        string NewImageFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string ImageFilePath = @"Images\Products\Product-" + ProductVM.product.ID;
                        string FinalImagePath = Path.Combine(wwwRootPath, ImageFilePath);

                        if (!Directory.Exists(FinalImagePath))
                        {
                            Directory.CreateDirectory(FinalImagePath);
                        }

                        using (var FileStream = new FileStream(Path.Combine(FinalImagePath, NewImageFileName), FileMode.Create))
                        {
                            file.CopyTo(FileStream);
                        }

                        ProductImage productImage = new ProductImage()
                        {
                            ImageURL = @"\" + ImageFilePath + @"\" + NewImageFileName,
                            ProductID = ProductVM.product.ID,
                        };
                        if (ProductVM.product.ProductImages == null)
                        {
                            ProductVM.product.ProductImages = new List<ProductImage>();
                        }
                        ProductVM.product.ProductImages.Add(productImage);

                        _UnitOfWork.ProductImage.Add(productImage);
                    }

                    _UnitOfWork.Save();
                }

                return RedirectToAction("Index");
            }
            else
            {
                ProductVM.Categories = _UnitOfWork.Category.GetAll().
                Select(C => new SelectListItem
                {
                    Text = C.Name,
                    Value = C.ID.ToString()
                });
                return View(ProductVM);
            }

        }

        public IActionResult DeleteImage(int ImageID)
        {
            ProductImage ImageToDelete = _UnitOfWork.ProductImage.Get(PI => PI.ID == ImageID);
            int ProductID = ImageToDelete.ProductID;

            if(ImageToDelete != null)
            {
                if(!string.IsNullOrEmpty(ImageToDelete.ImageURL))
                {
                    var OldImagePath = Path.Combine(_WebHostEnvironment.WebRootPath, ImageToDelete.ImageURL.TrimStart('\\'));
                    if (System.IO.File.Exists(OldImagePath))
                    {
                        System.IO.File.Delete(OldImagePath);
                    }
                }

                _UnitOfWork.ProductImage.Remove(ImageToDelete);
                _UnitOfWork.Save();
                TempData["Success"] = "Image Deleted Successfully";
            }
            return RedirectToAction(nameof(Upsert), new { id = ProductID });
            
        }

        #region Api Calls

        [HttpGet]
        public IActionResult Getall()
        {
            List<Product> Products = _UnitOfWork.Product.GetAll(IncludeProperties: "category").ToList();
            return Json(new { data = Products });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Product ProductToDelete = _UnitOfWork.Product.Get(P => P.ID == id);

            if (ProductToDelete == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }

            //var OldImagePath = Path.Combine(_WebHostEnvironment.WebRootPath, ProductToDelete.ImageUrl.TrimStart('\\'));
            //if (System.IO.File.Exists(OldImagePath))
            //{
            //    System.IO.File.Delete(OldImagePath);
            //}

            string FolderPath = @"Images\Products\Product-" + id;
            string FinalPath = Path.Combine(_WebHostEnvironment.WebRootPath, FolderPath);

            if (Directory.Exists(FinalPath))
            {
                Directory.Delete(FinalPath,true); // true here makes it delete the directory and files inside that directory
            }

            _UnitOfWork.Product.Remove(ProductToDelete);
            _UnitOfWork.Save();

            return Json(new { success = true, message = "Deleted Successfully" });
        }

        #endregion

    }
}
