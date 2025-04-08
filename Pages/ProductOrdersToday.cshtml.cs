using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Neemah.Data;
using Neemah.Models;

namespace Neemah.Pages
{
    public class ProductOrdersTodayModel : PageModel
    {
        private readonly AppDbContext _db;

        public ProductOrdersTodayModel(AppDbContext db)
        {
            _db = db;
        }

        public List<ProductOrderSummary> Summaries { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var username = User.Identity?.Name;
            var user = _db.Users.FirstOrDefault(u => u.Username == username);

            if (user == null || !(user.UserType == "Admin" || user.UserType == "User2"))
            {
                return RedirectToPage("/AccessDenied");
            }

            var today = DateTime.Today;

            Summaries = await _db.Products
                .Select(p => new ProductOrderSummary
                {
                    ProductNameEnglish = p.ItemNameEnglish,
                    RawMaterials = p.RawMaterials,
                    Additions = p.Additions,
                    SpicesWeight = p.SpicesWeight,
                    SaltWeight = p.SaltWeight,
                    PlateWeight = p.PlateWeight,
                    TotalOrdersToday = _db.FactoryOrders
                        .Where(o => o.ProductId == p.Id && o.Date.Date == today)
                        .Sum(o => (int?)o.Order) ?? 0
                })
                .ToListAsync();

            return Page();
        }

        public class ProductOrderSummary
        {
            public string ProductNameEnglish { get; set; }
            public double RawMaterials { get; set; }
            public double Additions { get; set; }
            public double SpicesWeight { get; set; }
            public double SaltWeight { get; set; }
            public double PlateWeight { get; set; }
            public int TotalOrdersToday { get; set; }
        }
    }
}