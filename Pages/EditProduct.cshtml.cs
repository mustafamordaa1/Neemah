using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Neemah.Data;
using Neemah.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Neemah.Pages
{
    public class EditProductModel : PageModel
    {
        private readonly AppDbContext _db;

        public EditProductModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public List<Product> Products { get; set; } = new List<Product>();

        [BindProperty]
        public string SearchQuery { get; set; }

        public async Task<IActionResult> OnGetAsync(string search)
        {

            var username = User.Identity?.Name;
            var user = _db.Users.FirstOrDefault(u => u.Username == username);

            if (user == null || !(user.UserType == "Admin" || user.UserType == "User2"))
            {
                return RedirectToPage("/AccessDenied");
            }

            // Save the search query to be pre-populated in the input field
            SearchQuery = search;

            // If a search term is provided, filter products by SN or Name (English)
            if (!string.IsNullOrEmpty(search))
            {
                Products = await _db.Products
                    .Where(p => p.SN.Contains(search) || p.ItemNameEnglish.Contains(search))
                    .ToListAsync();
            }
            else
            {
                // Load all products if no search term is provided
                Products = await _db.Products.ToListAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            // Get the ProductId from the hidden input field
            var productId = Request.Form["ProductId"];
            var action = Request.Form["action"];

            // Find the product to edit based on the product ID
            var existingProduct = await _db.Products.FirstOrDefaultAsync(p => p.Id == int.Parse(productId));

            if (action == "delete")
            {
                _db.Products.Remove(existingProduct);
                await _db.SaveChangesAsync();
                return RedirectToPage();
            }

            if (existingProduct != null)
            {
                // Update the product properties with the new values from the form
                existingProduct.SN = Request.Form["SN"];
                existingProduct.ItemNameArabic = Request.Form["ItemNameArabic"];
                existingProduct.ItemNameEnglish = Request.Form["ItemNameEnglish"];
                existingProduct.Unit = Request.Form["Unit"];

                double.TryParse(Request.Form["RawMaterials"], out double rawMaterials);
                existingProduct.RawMaterials = rawMaterials;

                double.TryParse(Request.Form["Additions"], out double additions);
                existingProduct.Additions = additions;

                double.TryParse( Request.Form["SpicesWeight"], out double spicesWeight);
                existingProduct.SpicesWeight = spicesWeight;

                double.TryParse(Request.Form["SaltWeight"], out double saltWeight);
                existingProduct.SaltWeight = saltWeight;

                double.TryParse(Request.Form["PlateWeight"], out double plateWeight);
                existingProduct.PlateWeight = plateWeight;

                existingProduct.ImagePath = Request.Form["ImagePath"];

                // Update the product in the database
                _db.Products.Update(existingProduct);
                await _db.SaveChangesAsync();
            }

            // Redirect to the home page (or the page you want to show after saving the product)
            return RedirectToPage("/Index");
        }
    }
}
