using MeowForums.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeowForums.Data
{
    public class DataSeeder
    {
        private ApplicationDbContext context;
        public DataSeeder(ApplicationDbContext context)
        {
            this.context = context;
        }

        public Task SeedSuperUser()
        {
            var hasher = new PasswordHasher<ApplicationUser>();

            var user = new ApplicationUser
            {
                UserName = "ForumAdmin",
                NormalizedUserName = "forumadmin",
                Email = "admin@example.com",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString(),

            };
            var hashedPassword = hasher.HashPassword(user, "admin");
            user.PasswordHash = hashedPassword;

            var store = new RoleStore<IdentityRole>(context);
            string role = "Admin";
            bool hasRole = context.Roles.Any(roles => roles.Name == role);
            if (!hasRole)
            {
                store.CreateAsync(new IdentityRole
                {
                    Name = role,
                    NormalizedName = role.ToLower()
                });
            }

            var userStore = new UserStore<ApplicationUser>(context);

            var hasSuperUser = context.Users.Any(u => u.NormalizedUserName == user.UserName);
            if (!hasSuperUser)
            {
                userStore.CreateAsync(user);
                userStore.AddToRoleAsync(user, role);
            }

            context.SaveChangesAsync();

            return Task.CompletedTask;
        }
    }
}
