namespace IaEapp.Models {
    public class TransactionTemp {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; } 
        public DateTime Date { get; set; }
        public bool Income { get; set; }
        public int TransactionCategoryId { get; set; }
        public string UserId { get; set; } = string.Empty;

    }
}
