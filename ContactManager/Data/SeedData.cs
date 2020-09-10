using ContactManager.Authorization;
using ContactManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContactManager.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string seedUserPw)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                var adminId = await EnsureUser(serviceProvider, seedUserPw, "zhaosheng@administrator.com");
                await EnsureRole(serviceProvider, adminId, Constants.ContactAdministratorsRole);

                var managerID = await EnsureUser(serviceProvider, seedUserPw, "zhaosheng@manager.com");
                await EnsureRole(serviceProvider, managerID, Constants.ContactManagersRole);

                SeedDb(context, adminId);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string userPassword, string userName)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
            var user = await userManager.FindByNameAsync(userName);
            if (null == user)
            {
                user = new IdentityUser
                {
                    UserName = userName,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, userPassword);
            }

            if (null == user)
            {
                throw new Exception("The password is probably not strong enough!");
            }
            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string userId, string role)
        {
            IdentityResult identityResult = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();
            if (null == roleManager)
            {
                throw new Exception("roleManager is null");
            }

            if (!await roleManager.RoleExistsAsync(role))
            {
                identityResult = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();
            var user = await userManager.FindByIdAsync(userId);
            if (null == user)
            {
                throw new Exception("The seedUserPw was probably not strong enough!");
            }

            identityResult = await userManager.AddToRoleAsync(user, role);
            return identityResult;
        }

        public static void SeedDb(ApplicationDbContext context, string adminId)
        {
            if (context.Contact.Any())
            {
                return;
            }

            context.Contact.AddRange(
                new Contact
                {
                    FirstName = "Debra",
                    LastName = "Garcia",
                    Address = "1234 Main St",
                    City = "Redmond",
                    State = "WA",
                    Zip = "10999",
                    Email = "debra@example.com",
                    Status = ContactStatus.Approved,
                    OwnerId = adminId
                },
                new Contact
                {
                    FirstName = "Thorsten",
                    LastName = "Weinrich",
                    Address = "5678 1st Ave W",
                    City = "Redmond",
                    State = "WA",
                    Zip = "10999",
                    Email = "thorsten@example.com",
                    Status = ContactStatus.Submitted,
                    OwnerId = adminId
                },
                new Contact
                {
                    FirstName = "Yuhong",
                    LastName = "Li",
                    Address = "9012 State st",
                    City = "Redmond",
                    State = "WA",
                    Zip = "10999",
                    Email = "yuhong@example.com",
                    Status = ContactStatus.Rejected,
                    OwnerId = adminId
                },
                new Contact
                {
                    FirstName = "Jon",
                    LastName = "Orton",
                    Address = "3456 Maple St",
                    City = "Redmond",
                    State = "WA",
                    Zip = "10999",
                    Email = "jon@example.com",
                    Status = ContactStatus.Submitted,
                    OwnerId = adminId
                },
                new Contact
                {
                    FirstName = "Diliana",
                    LastName = "Alexieva-Bosseva",
                    Address = "7890 2nd Ave E",
                    City = "Redmond",
                    State = "WA",
                    Zip = "10999",
                    Email = "diliana@example.com",
                    OwnerId = adminId
                }
             );
            context.SaveChanges();
        }
    }
}