using IaEapp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IaEapp.Controllers {
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager) {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index() {
            return View(_roleManager.Roles);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(string name) {
            if (ModelState.IsValid) {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                    return RedirectToAction("Index");
                Errors(result);
            }
            return View(name);
        }

        private void Errors(IdentityResult result) {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(string id) {
            IdentityRole roleToDelete = await _roleManager.FindByIdAsync(id);
            if (roleToDelete != null) {
                IdentityResult result = await _roleManager.DeleteAsync(roleToDelete);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                Errors(result);
            }
            ModelState.AddModelError(string.Empty, "Role nenalezena.");
            return View("Index", _roleManager.Roles);
        }

        public async Task<IActionResult> Edit(string id) {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            List<AppUser> members = new List<AppUser>();
            List<AppUser> nonMembers = new List<AppUser>();
            foreach (AppUser user in _userManager.Users) {
                var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                list.Add(user);
            }
            return View(new RoleEdit { Role = role, Members = members, NonMembers = nonMembers });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(RoleModification model) {
            IdentityResult result;
            if (ModelState.IsValid) {
                foreach (string userId in model.AddIds ?? new string[] { }) {
                    AppUser user = await _userManager.FindByIdAsync(userId);
                    if (user != null) {
                        result = await _userManager.AddToRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                            Errors(result);
                    }
                }
                foreach (string userId in model.DeleteIds ?? new string[] { }) {
                    AppUser user = await _userManager.FindByIdAsync(userId);
                    if (user != null) {
                        result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                        if (!result.Succeeded) Errors(result);
                    }
                }
                return RedirectToAction("Index");
            }
            return await Edit(model.RoleId);
        }
    }
}