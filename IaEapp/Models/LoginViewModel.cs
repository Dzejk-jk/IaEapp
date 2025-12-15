using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IaEapp.Models {
    public class LoginViewModel {
        [EmailAddress]
        public string Email { get; set; }
        [DisplayName("Heslo")]
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
        public bool Remember {  get; set; }
    }
}
