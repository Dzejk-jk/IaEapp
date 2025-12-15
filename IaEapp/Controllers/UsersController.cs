using IaEapp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IaEapp.Controllers {
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly IPasswordValidator<AppUser> _passwordValidator;
        public UsersController(UserManager<AppUser> userManager, IPasswordHasher<AppUser> passwordHasher, IPasswordValidator<AppUser> passwordValidator) {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _passwordValidator = passwordValidator;
        }

        public IActionResult Index() {
            return View(_userManager.Users);
        }
        [AllowAnonymous]
        public ViewResult Create() => View();

        [AllowAnonymous]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(UserViewModel user) {
            if (ModelState.IsValid) {
                AppUser newUser = new AppUser {
                    UserName = user.UserName,
                    Email = user.Email,
                };
                IdentityResult result = await _userManager.CreateAsync(newUser, user.Password);
                if (result.Succeeded) 
                    return RedirectToAction("Index");
                else {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            } 
            return View(user);
        }

        public async Task<IActionResult> Edit(string id) {
            AppUser userToEdit = await _userManager.FindByIdAsync(id);
            if (userToEdit != null)
                return View(userToEdit);
            return View("NotFound");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(string id, string email, string password) {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user != null) {
                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(email))
                    user.Email = email;
                else
                    ModelState.AddModelError("", "Email nemůže být prázdný");
                if (!string.IsNullOrEmpty(password)) {
                    validPass = await _passwordValidator.ValidateAsync(_userManager, user, password);
                    if (validPass.Succeeded)
                        user.PasswordHash = _passwordHasher.HashPassword(user, password);
                    else
                        Errors(validPass);
                } else
                    ModelState.AddModelError("", "Heslo nemůže být prázdné");
                if (!string.IsNullOrEmpty(email) && validPass != null && validPass.Succeeded) {
                    IdentityResult result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                        Errors(result);
                }
            } 
            else
                ModelState.AddModelError("", "Uživatel nenalezen.");
            return View(user);
        }
        private void Errors(IdentityResult result) { 
            foreach (IdentityError error in result.Errors) 
                ModelState.AddModelError("", error.Description);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(string id) {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user != null) {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            } else
                ModelState.AddModelError(string.Empty, "Uživatel nenalezen");
            return View("Index", _userManager.Users);
        }
    }
}
