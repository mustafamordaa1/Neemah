using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Neemah.Data;
using Neemah.Models;

namespace Neemah.Pages
{
    public class ProductBranchSummaryModel : PageModel
    {
        private readonly AppDbContext _db;

        public ProductBranchSummaryModel(AppDbContext db)
        {
            _db = db;
        }

        public List<string> Branches { get; set; } = new() { "QATIF", "shubili", "SHATI", "TARUT", "SAIHAT", "HADLA" };
        public List<Product> Products { get; set; } = new();
        public DateTime CurrentDate { get; set; }

        public Dictionary<int, Dictionary<string, int>> ProductBranchOrders { get; set; } = new(); // ProductId => (Branch => Order)

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public async Task<IActionResult> OnGetAsync()
        {

            var username = User.Identity?.Name;
            var user = _db.Users.FirstOrDefault(u => u.Username == username);

            if (user == null || !(user.UserType == "Admin" || user.UserType == "User1"))
            {
                return RedirectToPage("/AccessDenied");
            }

            CurrentDate = DateTime.Today.AddDays(-PageNumber + 1);
            Products = await _db.Products.ToListAsync();

            var orders = await _db.FactoryOrders
                .Where(o => o.Date.Date == CurrentDate)
                .ToListAsync();

            ProductBranchOrders = Products.ToDictionary(
                p => p.Id,
                p => Branches.ToDictionary(b => b, b => 
                    orders.Where(o => o.ProductId == p.Id && o.Branch == b).Sum(o => o.Order)
                )
            );

            return Page();
        }
    }
}
