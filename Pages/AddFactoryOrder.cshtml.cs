using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Neemah.Data;
using Neemah.Models;
using System.Linq;

namespace Neemah.Pages
{
    public class AddFactoryOrderModel : PageModel
    {
        private readonly AppDbContext _db;

        public AddFactoryOrderModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public DateTime Date { get; set; } = DateTime.Today.AddDays(1); // Default to tomorrow
        [BindProperty]
        public string Branch { get; set; } // Selected branch
        public List<Product> Products { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var username = User.Identity?.Name;
            var user = _db.Users.FirstOrDefault(u => u.Username == username);

            if (user == null || !(user.UserType == "Admin" || user.UserType == "User1"))
            {
                return RedirectToPage("/AccessDenied");
            }
            
            Products = await _db.Products.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Branch))
            {
                ModelState.AddModelError("Branch", "Branch must be selected.");
                Products = await _db.Products.ToListAsync();
                return Page();
            }

            // Collect the orders for each product
            Products = await _db.Products.ToListAsync();
            var orderRequests = new List<FactoryOrder>();
            foreach (var product in Products)
            {
                // Check if the user filled in the order and stock fields for this product
                var orderQuantity = Request.Form[$"Order_{product.Id}"];
                var stockQuantity = Request.Form[$"Stock_{product.Id}"];

                if (int.TryParse(orderQuantity, out var order) && int.TryParse(stockQuantity, out var stock) && (order > 0 || stock > 0))
                {
                    var factoryOrder = new FactoryOrder
                    {
                        ProductId = product.Id,
                        Order = order,
                        Stock = stock,
                        Date = Date, // Use the selected date
                        Branch = Branch // Use the selected branch
                    };

                    orderRequests.Add(factoryOrder);
                }
            }

            // Add the valid orders to the database
            if (orderRequests.Any())
            {
                _db.FactoryOrders.AddRange(orderRequests);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage("ViewFactoryOrders"); // Redirect to the orders view page
        }
    }
}
