using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Neemah.Data;
using Neemah.Models;
using System.IO;
using System.Threading.Tasks;

namespace Neemah.Pages
{
    public class AddProductModel : PageModel
    {
        private readonly AppDbContext _db;

        public AddProductModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public string ItemNameArabic { get; set; }
        [BindProperty]
        public string ItemNameEnglish { get; set; }
        [BindProperty]
        public string SN { get; set; }
        [BindProperty]
        public string Unit { get; set; }
        [BindProperty]
        public IFormFile ImageFile { get; set; }


        public IActionResult OnGet()
        {
            if (!IsAdmin()) return RedirectToPage("/AccessDenied");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (string.IsNullOrWhiteSpace(ItemNameArabic) || string.IsNullOrWhiteSpace(ItemNameEnglish) || string.IsNullOrWhiteSpace(ItemNameArabic) || string.IsNullOrWhiteSpace(SN) ||
                string.IsNullOrWhiteSpace(Unit)
                )
                {
                    ModelState.AddModelError("", "Please fill all required fields.");
                    return Page();
                }
                else
            {
                var product = new Product
                {
                    ItemNameArabic = ItemNameArabic,
                    ItemNameEnglish = ItemNameEnglish,
                    SN = SN,
                    Unit = Unit
                };

                // Save the uploaded image if provided
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var imageFileName = $"{ItemNameEnglish}.png"; // Use English name as file name
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "products", "images", imageFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    product.ImagePath = $"/products/images/{imageFileName}";
                }
                else
                {
                    product.ImagePath = "/products/images/no-image.png";
                }

                double.TryParse(Request.Form["RawMaterials"], out double rawMaterials);
                product.RawMaterials = rawMaterials;

                double.TryParse(Request.Form["Additions"], out double additions);
                product.Additions = additions;

                double.TryParse( Request.Form["SpicesWeight"], out double spicesWeight);
                product.SpicesWeight = spicesWeight;

                double.TryParse(Request.Form["SaltWeight"], out double saltWeight);
                product.SaltWeight = saltWeight;

                double.TryParse(Request.Form["PlateWeight"], out double plateWeight);
                product.PlateWeight = plateWeight;

                _db.Products.Add(product);
                await _db.SaveChangesAsync();

                return RedirectToPage("/Index"); // Redirect back to the home page
            }

            return Page();
        }
        private bool IsAdmin()
        {
            var username = User.Identity?.Name;
            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            return user != null && user.UserType == "Admin";
        }
    }
}

