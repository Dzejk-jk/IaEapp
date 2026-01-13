using System.Diagnostics;
using IaEapp.Models;
using IaEapp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IaEapp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly TransactionService _transactionService;

        public HomeController(UserManager<AppUser> userManager, TransactionService transactionService)
        {
            _userManager = userManager;
            _transactionService = transactionService;
        }

        public async Task<IActionResult> IndexAsync() {
            AppUser user = await _userManager.GetUserAsync(User);
            return View("Index");
        }
        public IActionResult GetChart() {
            string userId = _userManager.GetUserId(User);
            var data = _transactionService.GetChartData(userId);
            return Json(data);
        }
        public IActionResult GetChartLastMonth() {
            string userId = _userManager.GetUserId(User);
            var data = _transactionService.GetChartDataLastMonth(userId);
            return Json(data);
        }
        public IActionResult GetChartThisMonth() {
            string userId = _userManager.GetUserId(User);
            var data = _transactionService.GetChartDataThisMonth(userId);
            return Json(data);
        }
        public IActionResult GetChartDataIncomeByMonth() {
            string userId = _userManager.GetUserId(User);
            var data = _transactionService.GetChartDataIncomeByMonth(userId);
            return Json(data);
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
