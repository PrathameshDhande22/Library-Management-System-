using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVC_Task.Models.ViewModels
{
    public class LoginViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Username must be greater than 5 characters")]
        [DisplayName("Username / Email")]
        [RegularExpression("^[^\\s]+$", ErrorMessage = "Username must not contain white spaces")]
        public string Username { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Password must be greater than 5 Characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}