using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IaEapp.Models {
    public class UserViewModel {
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [PasswordPropertyText]
        public string Password { get; set; }
    }
}
