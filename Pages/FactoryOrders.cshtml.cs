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
    public class FactoryOrdersModel : PageModel
    {
        private readonly AppDbContext _db;

        public FactoryOrdersModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public List<FactoryOrder> FutureOrders { get; set; } = new List<FactoryOrder>();

        [BindProperty]
        public List<FactoryOrder> PastOrders { get; set; } = new List<FactoryOrder>();

        [BindProperty]
        public List<FactoryOrder> AllOrders { get; set; } = new List<FactoryOrder>();

        [BindProperty]
        public List<Product> Products { get; set; } = new List<Product>();

        [BindProperty]
        public int ProductId { get; set; }

        [BindProperty]
        public string Branch { get; set; }

        [BindProperty]
        public int Order { get; set; }

        [BindProperty]
        public int Stock { get; set; }

        [BindProperty]
        public DateTime Date { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            AllOrders = await _db.FactoryOrders
            .Include(f => f.Product)  // Including related entities if needed
            .ToListAsync();
            Products = await _db.Products.ToListAsync();


            FutureOrders = AllOrders.Where(f => f.Date > DateTime.Today).ToList();
            PastOrders = AllOrders.Where(f => f.Date <= DateTime.Today).ToList();

            // Set the default date to tomorrow
            Date = DateTime.Now.AddDays(1);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ProductId == 0)
            {
                ModelState.AddModelError("ProductId", "Please select a product.");
                return Page();
            }

            var factoryOrder = new FactoryOrder
            {
                ProductId = ProductId,
                Branch = Branch,
                Order = Order,
                Stock = Stock,
                Date = Date
            };

            if (factoryOrder.Date < DateTime.Today)
            {
                // Return an error message if the date is in the past
                ModelState.AddModelError("FactoryOrder.Date", "The date cannot be in the past.");

                return Page();
            }

            _db.FactoryOrders.Add(factoryOrder);
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
