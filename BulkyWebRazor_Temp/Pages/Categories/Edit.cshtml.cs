using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _db;

        [BindProperty]
        public Category EditedCategory { get; set; }
        public EditModel(AppDbContext db)
        {
            _db = db;
        }
        public void OnGet(int ID)
        {
            if(ID != null || ID !=0)
            {
                EditedCategory = _db.Categories.SingleOrDefault(C => C.ID == ID);
            }
        }

        public IActionResult OnPost()
        {
            if(ModelState.IsValid)
            {
                _db.Categories.Update(EditedCategory);
                _db.SaveChanges();
                return RedirectToPage("Index");
            }
            return Page();
        }

    }
}
