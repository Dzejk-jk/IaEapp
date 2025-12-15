using IaEapp.DTO;
using IaEapp.Models;

namespace IaEapp.Services {
    public class TransactionCategoryService {
        private readonly ApplicationDbContext _dbContext;
        public TransactionCategoryService(ApplicationDbContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<TransactionCategoryDTO>> GetAllCategoriesAsync() {
            var transactionCategory = _dbContext.TransactionCategories;
            var transactionCatDTOs = new List<TransactionCategoryDTO>();
            foreach (var category in transactionCategory) {
                transactionCatDTOs.Add(ModelToDTO(category));
            }
            return transactionCatDTOs;
        }

        private static TransactionCategoryDTO ModelToDTO(TransactionCategory category) {
            if (category != null) {
                return new TransactionCategoryDTO {
                    Id = category.Id,
                    Name = category.Name
                };
            }
            return null;
        }
        private static TransactionCategory DTOToModel(TransactionCategoryDTO categoryDTO) {
            if (categoryDTO != null) {
                return new TransactionCategory {
                    Id = categoryDTO.Id,
                    Name = categoryDTO.Name
                };
            }
            return null;
        }
        public async Task CreateCategoryAsync(TransactionCategoryDTO newCategory) {
            await _dbContext.TransactionCategories.AddAsync(DTOToModel(newCategory));
            await _dbContext.SaveChangesAsync();

        }
        public async Task<TransactionCategoryDTO> GetCategoryIdAsync(int id) {
            var category = await _dbContext.TransactionCategories.FindAsync(id);
            return ModelToDTO(category);
        }

        internal async Task UpdateTransactionCategoryAsync(int id, TransactionCategoryDTO category) {
            var categoryToEdit = await _dbContext.TransactionCategories.FindAsync(id);
            if (categoryToEdit != null) {
                categoryToEdit.Name = category.Name;
                await _dbContext.SaveChangesAsync();
            }
        }

        internal async Task DeleteAsync(int id) {
            var categoryToDelete = await _dbContext.TransactionCategories.FindAsync(id);
            _dbContext.TransactionCategories.Remove(categoryToDelete);
            await _dbContext.SaveChangesAsync();
        }
    }
}
