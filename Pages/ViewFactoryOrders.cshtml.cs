using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Neemah.Data;
using Neemah.Models;

namespace Neemah.Pages
{
    public class ViewFactoryOrdersModel : PageModel
    {
        private readonly AppDbContext _db;

        public ViewFactoryOrdersModel(AppDbContext db)
        {
            _db = db;
        }

        public Dictionary<DateTime, List<FactoryOrder>> OrdersByDate { get; set; } = new();
        public List<Product> Products { get; set; } = new();
        public List<string> Branches { get; set; } = new() { "QATIF", "shubili", "SHATI", "TARUT", "SAIHAT", "HADLA" };
        

        [BindProperty(SupportsGet = true)]
        public string SelectedBranch { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterDate { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {

            var username = User.Identity?.Name;
            var user = _db.Users.FirstOrDefault(u => u.Username == username);

            if (user == null || !(user.UserType == "Admin" || user.UserType == "User1"))
            {
                return RedirectToPage("/AccessDenied");
            }

            var allOrders = await _db.FactoryOrders.Include(f => f.Product).ToListAsync();
            Products = await _db.Products.ToListAsync();

            if (string.IsNullOrEmpty(SelectedBranch))
            {
                SelectedBranch = Branches.First(); // Default to the first branch
            }

            var filtered = allOrders
            .Where(o => o.Branch == SelectedBranch &&
                        (!FilterDate.HasValue || o.Date.Date == FilterDate.Value.Date))
            .ToList();

            OrdersByDate = filtered
                .GroupBy(o => o.Date.Date)
                .OrderByDescending(g => g.Key)
                .ToDictionary(g => g.Key, g => g.ToList());

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            int id = Convert.ToInt32(Request.Form["Id"]);
            string action = Request.Form["action"];

            var order = await _db.FactoryOrders.FindAsync(id);

            if (order == null || order.Date.Date <= DateTime.Today)
                return RedirectToPage();

            if (action == "delete")
            {
                _db.FactoryOrders.Remove(order);
                await _db.SaveChangesAsync();
                return RedirectToPage();
            }

            order.ProductId = Convert.ToInt32(Request.Form["ProductId"]);
            order.Branch = Request.Form["Branch"];
            order.Order = Convert.ToInt32(Request.Form["Order"]);
            order.Stock = Convert.ToInt32(Request.Form["Stock"]);
            order.Date = DateTime.Parse(Request.Form["Date"]);

            _db.FactoryOrders.Update(order);
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
