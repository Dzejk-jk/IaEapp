namespace IaEapp.Models {
    public class TransactionCategory {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Transaction> Transactions { get; set; } = new();
    }
}
