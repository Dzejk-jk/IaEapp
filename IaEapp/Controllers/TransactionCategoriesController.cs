using IaEapp.DTO;
using IaEapp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IaEapp.Controllers {
    [Authorize(Roles = "Admin")]
    public class TransactionCategoriesController : Controller {
        private readonly TransactionCategoryService _transactionCategoryService;
        public TransactionCategoriesController(TransactionCategoryService transactionCategoryService) {
            _transactionCategoryService = transactionCategoryService;
        }
        public async Task<IActionResult> Index() {
            var categories = await _transactionCategoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        public async Task<IActionResult> Create() {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(TransactionCategoryDTO newCategory) {
            if (ModelState.IsValid) {
                await _transactionCategoryService.CreateCategoryAsync(newCategory);
                return RedirectToAction("Index");
            }
            return View(newCategory);
        }

        public async Task<IActionResult> Edit(int id) {
            var categoryToEdit = await _transactionCategoryService.GetCategoryIdAsync(id);
            if (categoryToEdit == null) {
                return View("NotFound");
            }
            return View(categoryToEdit);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync (int id, TransactionCategoryDTO category) {
            if (ModelState.IsValid) {
                await _transactionCategoryService.UpdateTransactionCategoryAsync(id, category);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id) {
            var categoryToDelete = await _transactionCategoryService.GetCategoryIdAsync(id);
            if (categoryToDelete == null)
                return View("NotFound");
            return View(categoryToDelete);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(int id) {
            await _transactionCategoryService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
