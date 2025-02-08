using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcomApi.Application.DTO
{
    public class UserCreateDTO
    {
        public string? User_Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string first_Name { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string Last_Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid Mobile Number")]
        public string Mobile { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Password is required")]
        [MinLength(5, ErrorMessage = "Password must be at least 5 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{5,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, one special character, and be at least 5 characters long.")]
        public string Password { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Confirm Password is required")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match")]
        public string Confirm_password { get; set; }

        public CreateAddressDTO Address { get; set; }
    }
}
