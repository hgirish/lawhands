using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lawhands.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lawhands.Controllers
{
    public class CreateTempAdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateTempAdminController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            await AddRole(RoleNames.Administrator);
            var role = await _roleManager.FindByNameAsync(RoleNames.Administrator);
            var applicationUsers = await _userManager.Users.ToListAsync();
            var adminUsers = await _userManager.GetUsersInRoleAsync(role.Name);

            if (!adminUsers.Any())
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    Name = "Admin Example",
                    DateIn = DateTime.Now
                };
                var createResult = await _userManager.CreateAsync(user, "SuperSecret123!");
                if (createResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, RoleNames.Administrator);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        private async Task AddRole(string roleName)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {

                await _roleManager.CreateAsync(new IdentityRole(roleName));

            }

        }
    }
}