using IaEapp.DTO;
using IaEapp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IaEapp.Services {
    public class TransactionTempService {
        private readonly ApplicationDbContext _dbContext;
        private readonly TransactionService _transactionService;
        public TransactionTempService(ApplicationDbContext dbContext, TransactionService transactionService) {
            _dbContext = dbContext;
            _transactionService = transactionService;
        }
        public async Task<IEnumerable<TransactionTemp>> GetAllAsync(string userId) {
            var transactions = _dbContext.TransactionsTemp.Where(tr => tr.UserId == userId);
            var temp = new List<TransactionTemp>();
            foreach (var transaction in transactions) {
                temp.Add(transaction);
            }
            return temp;
        }
        internal async Task CreateTrancastionAsync(TransactionTemp newTransaction) {
            await _dbContext.TransactionsTemp.AddAsync(newTransaction);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TransactionTemp> GetByIdAsync(int id) {
            var tempT = await _dbContext.TransactionsTemp.FirstOrDefaultAsync(tr => tr.Id == id);
            if (tempT != null)
                return tempT;
            return null;
        }
        internal async Task DeleteAsync(int id) {
            var transactionToDelete = await _dbContext.TransactionsTemp.FindAsync(id);
            _dbContext.TransactionsTemp.Remove(transactionToDelete);
            await _dbContext.SaveChangesAsync();
        }
        public async Task CreateAllAsync(string userId) {
            var transactions = await GetAllAsync(userId);
            foreach (var temp in transactions) {
                var newTransaction = TempToDTO(temp);
                if (newTransaction.Description == null)
                    newTransaction.Description = string.Empty;
                try {
                    await _transactionService.CreateTransactionAsync(newTransaction, userId);
                } catch (Exception ex) { Console.WriteLine($"Error during creating transaction: {ex}"); }
                _dbContext.TransactionsTemp.Remove(temp);
            }
            await _dbContext.SaveChangesAsync();
        }
        private static TransactionDTO TempToDTO(TransactionTemp transactionTemp) {
            return new TransactionDTO() {
                Description = transactionTemp.Description,
                Amount = transactionTemp.Amount,
                Date = transactionTemp.Date,
                Income = transactionTemp.Income,
                TransactionCategoryId = transactionTemp.TransactionCategoryId
            };
        }
        internal async Task ClearTempTransactionsAsync(string userId) {
            var userTempTransactions = await _dbContext.TransactionsTemp
                .Where(t => t.UserId == userId)
                .ToListAsync();

            if (userTempTransactions.Any()) {
                _dbContext.TransactionsTemp.RemoveRange(userTempTransactions);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task CreateAsync(TransactionTemp transactionTemp, string userId) {
            TransactionDTO newTransaction = TempToDTO(transactionTemp);
            if (newTransaction.Description == null)
                newTransaction.Description = string.Empty;
            try {
                await _transactionService.CreateTransactionAsync(newTransaction, userId);
            } catch (Exception ex) { Console.WriteLine($"Error during creating transaction: {ex}"); }

            var deleteTempTr = await _dbContext.TransactionsTemp.FindAsync(transactionTemp.Id);
            _dbContext.TransactionsTemp.Remove(deleteTempTr);
            await _dbContext.SaveChangesAsync();
        }
    }
}
