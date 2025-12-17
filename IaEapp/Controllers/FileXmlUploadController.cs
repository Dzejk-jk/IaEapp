using IaEapp.DTO;
using IaEapp.Models;
using IaEapp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace IaEapp.Controllers {
    [Authorize]
    public class FileXmlUploadController : Controller {
        private readonly TransactionTempService _transactionTempService;
        private readonly TransactionService _transactionService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDbContext _context;
        public FileXmlUploadController(TransactionTempService transactionTempService, UserManager<AppUser> userManager, TransactionService transactionService,
            ApplicationDbContext context) {
            _transactionTempService = transactionTempService;
            _userManager = userManager;
            _transactionService = transactionService;
            _context = context;
        }

        public IActionResult Index() {
            return View();
        }
        public async Task<IActionResult> Upload() {
            var userId = _userManager.GetUserId(User);
            var tempTransactions = await _transactionTempService.GetAllAsync(userId);
            if (tempTransactions != null) 
                return View(tempTransactions);
            return View(new TransactionTemp());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAsync(IFormFile file) {
            if (file == null || file.Length == 0)
                return BadRequest("Soubor nebyl nahrán.");
            string tempFilePath = Path.GetTempFileName();
            using (var stream = new FileStream(tempFilePath, FileMode.Create)) {
                await file.CopyToAsync(stream);
            }
            
            var settings = new XmlReaderSettings { IgnoreWhitespace = true };
            using (var reader = XmlReader.Create(tempFilePath, settings)) {
                while (reader.ReadToFollowing("FINSTA05")) {
                    var transactionTemp = new TransactionTemp();
                    var userId = _userManager.GetUserId(User);

                    reader.ReadToFollowing("S61_DATUM");
                    reader.Read();
                    string dateString = reader.Value;
                    transactionTemp.Date = DateTime.ParseExact(dateString, "d.M.yyyy", CultureInfo.InvariantCulture);

                    reader.ReadToFollowing("S61_CASTKA");
                    reader.Read();
                    string amount = reader.Value;
                    try {
                        transactionTemp.Amount = decimal.Parse(amount);
                    } catch { Exception e; }

                    reader.ReadToFollowing("PART_ID1_1");
                    reader.Read();
                    transactionTemp.Description = reader.Value;
                    transactionTemp.TransactionCategoryId = 8; //Import - nesmí se smazat
                    if (transactionTemp.Amount > 0)
                        transactionTemp.Income = true;
                    else 
                        transactionTemp.Income = false;
                    transactionTemp.UserId = userId;
                    await _transactionTempService.CreateTrancastionAsync(transactionTemp);
                }
            }
            System.IO.File.Delete(tempFilePath);
            return RedirectToAction("Upload");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(TransactionTemp transactionTemp) {
            if (transactionTemp == null)
                return RedirectToAction("NotFound");
            var userId = _userManager.GetUserId(User);
            await _transactionTempService.CreateAsync(transactionTemp, userId);
            return RedirectToAction("Upload");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearTempTransactions() {
            var userId = _userManager.GetUserId(User);
            await _transactionTempService.ClearTempTransactionsAsync(userId);
            return View("Upload", new List<TransactionTemp>());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAllAsync() {
            var userId = _userManager?.GetUserId(User);
            await _transactionTempService.CreateAllAsync(userId);
            return RedirectToAction("Index", "Transactions");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(int id) {
            await _transactionTempService.DeleteAsync(id);
            return RedirectToAction("Upload");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dashboard() {
            await ClearTempTransactions();
            return RedirectToAction("Index", "Transactions");
        }
    }
}
