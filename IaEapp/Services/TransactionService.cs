using IaEapp.DTO;
using IaEapp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IaEapp.Services {
    public class TransactionService {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        public TransactionService(ApplicationDbContext dbContext, UserManager<AppUser> userManager) {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<IEnumerable<TransactionDTO>> GetAllAsync(string userId) {
            var transactions = _dbContext.Transactions.Include(tr => tr.TransactionCategory).Include(tr => tr.User).Where(tr => tr.UserId == userId);
            var transactionDTOs = new List<TransactionDTO>();
            foreach (var transaction in transactions) {
                transactionDTOs.Add(modelToDTO(transaction));
            }
            return transactionDTOs.OrderByDescending(t => t.Date);
        }
        private TransactionDTO modelToDTO(Transaction transaction) {
            if (transaction != null) {
                return new TransactionDTO {
                    Id = transaction.Id,
                    Description = transaction.Description,
                    Amount = transaction.Amount,
                    Date = transaction.Date,
                    Income = transaction.Amount > 0,
                    TransactionCategoryId = transaction.TransactionCategoryId,
                    TransactionCategoryName = GetCategoryNameAsync(transaction.TransactionCategoryId),
                    UserId = transaction.UserId,
                    UserName = transaction.User?.UserName
                }; 
            }
            return null;
        }
        private string GetCategoryNameAsync(int id) {
            var category = _dbContext.TransactionCategories.FirstOrDefault(c => c.Id == id);
            if (category == null)
                return "";
            return category.Name;
        }
        internal async Task CreateTransactionAsync(TransactionDTO transactionDTO, string userId) {
            Transaction newTransaction = DtoToModel(transactionDTO);
            newTransaction.UserId = userId;
            await _dbContext.Transactions.AddAsync(newTransaction);
            await _dbContext.SaveChangesAsync();
        }

        private static Transaction DtoToModel(TransactionDTO transactionDTO) {
            if (transactionDTO != null) {
                return new Transaction() {
                    Id = transactionDTO.Id,
                    Description = transactionDTO.Description,
                    Amount = transactionDTO.Amount,
                    Date = transactionDTO.Date,
                    Income = transactionDTO.Amount > 0,
                    TransactionCategoryId = transactionDTO.TransactionCategoryId
                };
            };
            return null;
        }

        public TransactionCategoriesDropdownsViewModel GetCategoriesData() {
            var categoriesData = new TransactionCategoriesDropdownsViewModel {
                Categories = _dbContext.TransactionCategories.OrderBy(c => c.Name)
            };
            return categoriesData;
        }
        public async Task<TransactionDTO> GetByIdAsync(int id) {
            var transaction = await _dbContext.Transactions.FirstOrDefaultAsync(t => t.Id == id);
            return modelToDTO(transaction);
        }

        internal async Task<TransactionDTO> EditAsync(int id, TransactionDTO transactionToUpdate, string userId) {
            var trans = DtoToModel(transactionToUpdate);
            trans.Id = id;
            trans.UserId = userId;
            _dbContext.Transactions.Update(trans);
            await _dbContext.SaveChangesAsync();
            return transactionToUpdate;
        }

        internal async Task<TransactionDTO> DeleteAsync(int id) {
            var transactionToDelete = await _dbContext.Transactions.FindAsync(id);
            _dbContext.Transactions.Remove(transactionToDelete);
            await _dbContext.SaveChangesAsync();
            return(modelToDTO(transactionToDelete));
        }

        internal IQueryable<TransactionDTO> TransactionFilters(string sortOrder, IQueryable<TransactionDTO> transactions) {
            switch (sortOrder) {
                case "last_30_days":
                    transactions = transactions.Where(t => t.Date >= DateTime.Now.AddDays(-30));
                    break;
                case "income":
                    transactions = transactions.Where(t => t.Income);
                    break;
                case "expanse":
                    transactions = transactions.Where(t => !t.Income);
                    break;
                case "lastMonth":
                    var startDate = DateTime.Now.AddMonths(-1).Date;
                    startDate = new DateTime(startDate.Year, startDate.Month, 1);
                    var endDate = startDate.AddMonths(1);
                    transactions = transactions.Where(t => t.Date >= startDate && t.Date < endDate);
                    break;
                case "thisMonth":
                    var firstDayOfCurrentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    var firstDayOfNextMonth = firstDayOfCurrentMonth.AddMonths(1);
                    transactions = transactions.Where(t =>
                                    t.Date >= firstDayOfCurrentMonth &&
                                    t.Date < firstDayOfNextMonth);
                    break;

                default:
                    transactions = transactions.OrderByDescending(t => t.Date);
                    break;
            }

            return transactions;
        }
        public PaginatedList<TransactionDTO> Pagination(int? pageNumber, IQueryable<TransactionDTO> transactionsQuery) {
            var filteredTransactions = transactionsQuery.ToList();
            int pageSize = 20;
            int totalCount = filteredTransactions.Count;
            int skip = ((pageNumber ?? 1) - 1) * pageSize;
            var pagedTransactions = filteredTransactions
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            var paginatedList = new PaginatedList<TransactionDTO>(
                pagedTransactions,
                totalCount,
                pageNumber ?? 1,
                pageSize
            );
            return paginatedList;
        }

        public IQueryable GetChartData(string userId) {
            var data = _dbContext.Transactions
                        .Include(tr => tr.TransactionCategory)
                        .Include(tr => tr.User)
                        .Where(tr => tr.UserId == userId)
                        .GroupBy(t => t.TransactionCategoryId)
                        .Select(g => new {
                            TransactionCategoryId = g.Key,
                            CategoryName = g.First().TransactionCategory.Name,
                            Total = g.Sum(e => e.Amount) 
                        });
            return data;
        }

        public IQueryable GetChartDataLastMonth(string userId) {
            var startDate = DateTime.Now.AddMonths(-1).Date;
            startDate = new DateTime(startDate.Year, startDate.Month, 1);
            var endDate = startDate.AddMonths(1);
            var data = _dbContext.Transactions
                        .Include(tr => tr.TransactionCategory)
                        .Include(tr => tr.User)
                        .Where(tr => tr.UserId == userId)
                        .Where(t => t.Date >= startDate && t.Date < endDate)
                        .GroupBy(t => t.TransactionCategoryId)
                        .Select(g => new {
                            TransactionCategoryId = g.Key,
                            CategoryName = g.First().TransactionCategory.Name,
                            Total = g.Sum(e => e.Amount)
                        });
            return data;
        }
        public IQueryable GetChartDataThisMonth(string userId) {
            var firstDayOfCurrentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var firstDayOfNextMonth = firstDayOfCurrentMonth.AddMonths(1);
            var data = _dbContext.Transactions
                        .Include(tr => tr.TransactionCategory)
                        .Include(tr => tr.User)
                        .Where(tr => tr.UserId == userId)
                        .Where(t => t.Date >= firstDayOfCurrentMonth && t.Date < firstDayOfNextMonth)
                        .GroupBy(t => t.TransactionCategoryId)
                        .Select(g => new {
                            TransactionCategoryId = g.Key,
                            CategoryName = g.First().TransactionCategory.Name,
                            Total = g.Sum(e => e.Amount)
                        });
            return data;
        }
    }
}

