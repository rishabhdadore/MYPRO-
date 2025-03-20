using System.ComponentModel.DataAnnotations;

namespace MYPRODUCTPRO.Models
{
    public class UsersClass
    {
        public int UserID { get; set; }  // Auto-incremented Unique ID
        [Required(ErrorMessage ="Please Enetr a User Name ")]
        public string UserName { get; set; } // Required Username

        [Required(ErrorMessage ="Please Enter Password")]
        public string UserPassword { get; set; } // Store Hashed Password

        public string Email { get; set; } // Unique Email

        public string UserRole { get; set; } = "User"; // Default Role is User

        public DateTime CreatedDate { get; set; } = DateTime.Now; // Account Creation Date

        public bool IsActive { get; set; } = true; // Active Status (true = Active, false = Inactive)
    }
}
