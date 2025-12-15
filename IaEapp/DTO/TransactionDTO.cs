using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IaEapp.DTO {
    public class TransactionDTO {
        public int Id { get; set; }
        [DisplayName("Popis"), StringLength(128)]
        public string Description { get; set; } = string.Empty;
        [DisplayName("Částka"), DataType(DataType.Currency), Column(TypeName = "decimal(18, 2")]
        public decimal Amount { get; set; }
        [DisplayName("Datum")]
        public bool Income { get; set; } = false;
        public DateTime Date { get; set; } = DateTime.Now;
        [DisplayName("Kategorie")]
        public int TransactionCategoryId { get; set; }
        [DisplayName("Název kategorie")] 
        public string TransactionCategoryName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
