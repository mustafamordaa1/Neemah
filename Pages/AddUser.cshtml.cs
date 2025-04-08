using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Neemah.Models;
using Neemah.Data;
using Microsoft.AspNetCore.Identity;

namespace Neemah.Pages
{
    public class AddUserModel : PageModel
    {
        private readonly AppDbContext _db;

        public AddUserModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public User NewUser { get; set; }

        public string Message { get; set; }

        public IActionResult OnGet()
        {
            if (!IsAdmin()) return RedirectToPage("/AccessDenied");
            return Page();
        }

        public IActionResult OnPost()
            {
                if (!IsAdmin()) return RedirectToPage("/AccessDenied");

                if (!ModelState.IsValid) return Page();

                // ✅ Check for existing username
                if (_db.Users.Any(u => u.Username == NewUser.Username))
                {
                    ModelState.AddModelError("NewUser.Username", "This username is already taken.");
                    return Page();
                }

                // ✅ Hash the password
                var hasher = new PasswordHasher<User>();
                NewUser.Password = hasher.HashPassword(NewUser, NewUser.Password);

                _db.Users.Add(NewUser);
                _db.SaveChanges();

                Message = $"User '{NewUser.Username}' added successfully.";
                NewUser = new User(); // clears all fields
                ModelState.Clear(); // optional, clears validation messages


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
