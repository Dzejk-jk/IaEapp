namespace IaEapp.Models {
    public class TransactionCategoriesDropdownsViewModel {
        public IEnumerable<TransactionCategory> Categories { get; set; }
        public TransactionCategoriesDropdownsViewModel() { 
            Categories = new List<TransactionCategory>();
        }
    }
}
