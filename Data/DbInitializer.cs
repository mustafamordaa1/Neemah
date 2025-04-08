using Microsoft.AspNetCore.Identity;
using Neemah.Models;

namespace Neemah.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Users.Any())
            {
                var hasher = new PasswordHasher<User>();
                var user = new User
                {
                    Username = "mustafa",
                    UserType = "Admin"
                };

                user.Password = hasher.HashPassword(user, "helloworld");

                context.Users.Add(user);
            }

            context.SaveChanges();
        }
    }
}
