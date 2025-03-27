using System.ComponentModel.DataAnnotations;

namespace EcomAppUI.Models
{
    public class UserModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Your First Name")]
        public string first_Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Your Last Name")]
        public string Last_Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Your Email")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Your Mobile Number")]
        public string Mobile { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Your Password")]
        public string Password { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Your Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string Confirm_password { get; set; }

        public CreateAddress Address { get; set; } = new CreateAddress();
    }
}
