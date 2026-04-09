using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _db;

        [BindProperty]
        public Category NewCategory { get; set; }
        public CreateModel(AppDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            _db.Categories.Add(NewCategory);
            _db.SaveChanges();
            return RedirectToPage("Index");
        }
    }
}
