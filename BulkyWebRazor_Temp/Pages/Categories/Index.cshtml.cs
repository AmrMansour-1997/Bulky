using BulkyWebRazor_Temp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BulkyWebRazor_Temp.Models;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;
        public List<Category> CategoriesList { get; set; }
        public IndexModel(AppDbContext db)
        {
            _db = db;
        }
        public void OnGet()
        {
            CategoriesList = _db.Categories.ToList();
        }
    }
}
