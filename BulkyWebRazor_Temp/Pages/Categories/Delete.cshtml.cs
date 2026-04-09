using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
       
        private readonly AppDbContext _db;

        public Category? DeletedCategory { get; set; }
        public DeleteModel(AppDbContext db)
        {
            _db = db;
        }
        public void OnGet(int ID)
        {
            if (ID != null || ID != 0)
            {
                DeletedCategory = _db.Categories.FirstOrDefault(C => C.ID == ID);
            }
        }

        public IActionResult OnPost()
        {
            Category? Cat = _db.Categories.Find(DeletedCategory.ID);

            if (Cat != null)
            {
                _db.Categories.Remove(Cat);
                _db.SaveChanges();
                TempData["Success"] = "Category Deleted Successfully !";
                return RedirectToPage("Index");
            }
            return RedirectToPage();
        }

    }
}
