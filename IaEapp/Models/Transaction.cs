namespace IaEapp.Models {
    public class Transaction {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool Income { get; set; } = false;
        public DateTime Date { get; set; } = DateTime.Now;
        public int TransactionCategoryId { get; set; }
        public TransactionCategory? TransactionCategory { get; set; }
        public string UserId { get; set; } = string.Empty;
        public AppUser User { get; set; }
        }
    }
