using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Neemah.Data;
using Neemah.Models;

namespace Neemah.Pages
{
    public class EditDeleteUserModel : PageModel
    {
        private readonly AppDbContext _db;

        public EditDeleteUserModel(AppDbContext db)
        {
            _db = db;
        }

        public List<User> Users { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var currentUser = _db.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
            if (currentUser == null || currentUser.UserType != "Admin")
                return RedirectToPage("/AccessDenied");

            Users = await _db.Users.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var form = Request.Form;
            var action = form["action"];
            int id = int.Parse(form["Id"]);
            var user = await _db.Users.FindAsync(id);

            if (user == null)
                return RedirectToPage();

            if (action == "edit")
            {
                user.Username = form["Username"];
                user.UserType = form["UserType"];
                await _db.SaveChangesAsync();
            }
            else if (action == "delete")
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
