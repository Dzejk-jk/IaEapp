using IaEapp.DTO;
using IaEapp.Models;
using IaEapp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IaEapp.Controllers {
    [Authorize]
    public class TransactionsController : Controller {
        private readonly TransactionService _transactionService;
        private readonly UserManager<AppUser> _userManager;
        public TransactionsController(TransactionService transactionService, UserManager<AppUser> userManager) {
            _transactionService = transactionService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string sortOrder, string q, string currentFilter, int? pageNumber) {
            var userId = _userManager.GetUserId(User);
            var transactions = await _transactionService.GetAllAsync(userId);
            var transactionsQuery = transactions.AsQueryable();

            ViewData["CurrentSort"] = sortOrder;
            ViewData["Last30DaysSort"] = "last_30_days";
            ViewData["IncomeSort"] = "income";
            ViewData["ExpanseSort"] = "expanse";
            ViewData["All"] = "";
            ViewData["ThisMonth"] = "thisMonth";
            ViewData["LastMonth"] = "lastMonth";

            if (q != null)
                pageNumber = 1;
            else
                q = currentFilter;

            ViewData["CurrentFilter"] = q;

            if (!string.IsNullOrEmpty(q)) {
                transactionsQuery = transactionsQuery.Where(t => t.Description.ToUpper().Contains(q.ToUpper()) ||
                                                            t.TransactionCategoryName.ToUpper().Contains(q.ToUpper()) ||
                                                            t.Amount.ToString().Contains(q));
            }

            transactionsQuery = _transactionService.TransactionFilters(sortOrder, transactionsQuery);
            var paginatedList = _transactionService.Pagination(pageNumber, transactionsQuery);
            return View(paginatedList);
        }

        public IActionResult Create() {
            var dropdownsData = _transactionService.GetCategoriesData();
            ViewData["Categories"] = new SelectList(dropdownsData.Categories, "Id", "Name"); 
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TransactionDTO newTransaction) {
            if (ModelState.IsValid) {
                var userId = _userManager.GetUserId(User);
                await _transactionService.CreateTransactionAsync(newTransaction, userId);
            }
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Edit(int id) {
            var transactionToEdit = await _transactionService.GetByIdAsync(id);
            ViewData["Categories"] = new SelectList(_transactionService.GetCategoriesData().Categories, "Id", "Name");
            if (transactionToEdit == null) {
                return View("NotFound");
            }
            if (transactionToEdit.UserId != _userManager.GetUserId(User))
                return View("AccessDenied");
            return View(transactionToEdit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TransactionDTO transaction) {
            if (ModelState.IsValid) {
                var userId = _userManager.GetUserId(User);
                await _transactionService.EditAsync(id, transaction, userId);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id) {
            var transactionToDelete = await _transactionService.GetByIdAsync(id);
            if (transactionToDelete == null) {
                return View("NotFound");
            }
            if (transactionToDelete.UserId != _userManager.GetUserId(User))
                return View("AccessDenied");
            return View(transactionToDelete);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(int id) {
            await _transactionService.DeleteAsync(id);
            return RedirectToAction("Index");
        }

    }
}
