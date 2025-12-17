using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IaEapp.DTO {
    public class TransactionDTO {
        public int Id { get; set; }
        [DisplayName("Popis"), StringLength(128)]
        [Required]
        public string Description { get; set; } = string.Empty;
        [DisplayName("Částka"), DataType(DataType.Currency), Column(TypeName = "decimal(18, 2")]
        public decimal Amount { get; set; }
        public bool Income { get; set; } = false;
        [DisplayName("Datum")]
        public DateTime Date { get; set; } = DateTime.Now;
        [DisplayName("Kategorie")]
        public int TransactionCategoryId { get; set; }
        [DisplayName("Název kategorie")] 
        public string TransactionCategoryName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
}
