using System.ComponentModel.DataAnnotations;

namespace EcomAppUI.Models
{
    public class LoginModel
    {

       // [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Your First Name")]
        public string? Email { get; set; }

        //[Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Your First Name")]
        public string? Password { get; set; }
    }
}
