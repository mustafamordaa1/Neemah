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
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _db;

        public IndexModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public List<Product> Products { get; set; }

        [BindProperty]
        public string NewItemNameArabic { get; set; }
        [BindProperty]
        public string NewItemNameEnglish { get; set; }
        [BindProperty]
        public string NewSN { get; set; }
        [BindProperty]
        public string NewUnit { get; set; }
        [BindProperty]
        public IFormFile NewImageFile { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var username = User.Identity?.Name;
            var user = _db.Users.FirstOrDefault(u => u.Username == username);

            if (user == null || !(user.UserType == "Admin" || user.UserType == "User1"))
            {
                return RedirectToPage("/login");
            }

            Products = await _db.Products.ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // Update existing products
                foreach (var product in Products)
                {
                    // Handle image upload for each product (if there's an updated image)
                    var imageFile = Request.Form.Files[$"Products[{Products.IndexOf(product)}].ImageFile"];
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var imageFileName = $"{product.ItemNameEnglish}.jpg"; // Use the English name as the image file name
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "products", "images", imageFileName);

                        // Save the new image
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        // Update the image path in the product
                        product.ImagePath = $"/products/images/{imageFileName}";
                    }

                    // Update other product details
                    product.ItemNameArabic = Request.Form[$"Products[{Products.IndexOf(product)}].ItemNameArabic"];
                    product.ItemNameEnglish = Request.Form[$"Products[{Products.IndexOf(product)}].ItemNameEnglish"];
                    product.SN = Request.Form[$"Products[{Products.IndexOf(product)}].SN"];
                    product.Unit = Request.Form[$"Products[{Products.IndexOf(product)}].Unit"];

                    _db.Products.Update(product);
                }

                // Add a new product
                if (!string.IsNullOrEmpty(NewItemNameArabic) && !string.IsNullOrEmpty(NewItemNameEnglish) && !string.IsNullOrEmpty(NewSN))
                {
                    var newProduct = new Product
                    {
                        ItemNameArabic = NewItemNameArabic,
                        ItemNameEnglish = NewItemNameEnglish,
                        SN = NewSN,
                        Unit = NewUnit
                    };

                    // Save the new image if uploaded
                    if (NewImageFile != null && NewImageFile.Length > 0)
                    {
                        var imageFileName = $"{NewItemNameEnglish}.jpg";
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "products", "images", imageFileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await NewImageFile.CopyToAsync(stream);
                        }

                        newProduct.ImagePath = $"/products/images/{imageFileName}";
                    }

                    _db.Products.Add(newProduct);
                }

                await _db.SaveChangesAsync();
                return RedirectToPage();
            }

            return Page();
        }
    }
}
