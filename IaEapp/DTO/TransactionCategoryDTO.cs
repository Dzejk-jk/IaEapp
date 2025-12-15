using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IaEapp.DTO {
    public class TransactionCategoryDTO {
        public int Id { get; set; }
        [DisplayName("Název kategorie"), StringLength(128, MinimumLength = 3)]
        [Required]
        public string Name { get; set; }
    }
}
