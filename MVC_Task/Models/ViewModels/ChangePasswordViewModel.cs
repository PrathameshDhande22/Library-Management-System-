using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVC_Task.Models.ViewModels
{
    public class ChangePasswordViewModel
    {
        public int Id { get; set; }


        [Required]
        [MinLength(5, ErrorMessage = "Password must be greater than 5 Characters")]
        [DataType(DataType.Password)]
        [DisplayName("Old Password")]
        public string OldPassword { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Password must be greater than 5 Characters")]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{5,}$", ErrorMessage = "Password Must Contain one Lowercase, one Uppercase, one letter, one Symbol")]
        [DataType(DataType.Password)]
        [DisplayName("New Password")]
        public string Password { get; set; }

        [Required]
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Password Doesn't Match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}