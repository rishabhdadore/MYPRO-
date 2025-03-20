using System.ComponentModel.DataAnnotations;

namespace MYPRODUCTPRO.Models
{
    public class CreateEmployeeViewModel
    {
        public int? EmployeeID { get; set; }


        [Required]

        public string FirstName { get; set; }

        [Required]

        public string Email { get; set; }

        [Required]

        public string Mobile { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]

        public string Password { get; set; }

        [Required]

        public string ConfirmPassword { get; set; }

        [Required]
        public string Role { get; set; }  // "manager" or "employee"

        [Required]
        public string Status { get; set; }  // "active" or "inactive"

        public IFormFile ProfilePicture { get; set; }



        public DateTime ?CreatedAt { get; set; }  // "active" or "inactive"



    }
}

