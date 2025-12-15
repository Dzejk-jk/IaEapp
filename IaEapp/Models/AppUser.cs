using Microsoft.AspNetCore.Identity;

namespace IaEapp.Models {
    public class AppUser : IdentityUser {
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
