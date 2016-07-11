using System;
using System.Net;
using System.Threading.Tasks;
using lawhands.Models;
using lawhands.Models.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace lawhands.Controllers
{
    public class UsersAdminController : AdminBaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersAdminController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var applicationUsers = await _userManager.Users.ToListAsync();
            
            return View(applicationUsers);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            var user = await _userManager.FindByIdAsync(id);

            ViewBag.RoleNames = await _userManager.GetRolesAsync(user);

            return View(user);
        }
        public async Task<IActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            ViewBag.RoleId = new SelectList(_roleManager.Roles, "Name", "Name");

            if (ModelState.IsValid)
            {

                var user = new ApplicationUser
                {
                    UserName = userViewModel.Email,
                    Email = userViewModel.Email,
                    Name = userViewModel.Name,
                    DateIn = DateTime.Now
                };
                var adminresult = await _userManager.CreateAsync(user, userViewModel.Password);

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    // user.CustomerId = user.UserDetail.Id;
                    // await _userManager.UpdateAsync(user);
                    if (selectedRoles != null)
                    {
                        var result = await _userManager.AddToRolesAsync(user, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First().Description);
                            return View();
                        }
                    }
                }
                else
                {
                    var errorMessage = adminresult.Errors.First().Description;
                    if (errorMessage != null)
                        ModelState.AddModelError("", errorMessage);
                    return View();

                }
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new NotFoundResult();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
          
            return View(new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                RolesList = _roleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Email,Id")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return new NotFoundResult();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;

                var userRoles = await _userManager.GetRolesAsync(user);

                selectedRole = selectedRole ?? new string[] { };

                var result = await _userManager.AddToRolesAsync(user, selectedRole.Except(userRoles).ToArray());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().Description);
                    return View();
                }
                result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRole).ToArray());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().Description);
                    return View();
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new BadRequestResult();
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return new NotFoundResult();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new BadRequestResult();
                }

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return new NotFoundResult();
                }
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().Description);
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
        private async Task AddRole(string roleName)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {

                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }

        }
        //  [AllowAnonymous]
        public async Task<string> GetNumberOfUsers()
        {
            var numberOfUsers = Task.Run(() => _userManager.Users.Count());
            return $"Users imported so far: {await numberOfUsers}";
        }
    }
}