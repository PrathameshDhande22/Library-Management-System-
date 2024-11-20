using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CompareAttribute = System.ComponentModel.DataAnnotations.CompareAttribute;

namespace MVC_Task.Models
{

    public class User
    {
        public int Id { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Username must be greater than 5 characters")]
        [Remote("IsUserNameExist", "Auth", ErrorMessage = "Username already exists Please use other username", AdditionalFields = "uid")]
        [RegularExpression("^[^\\s]+$", ErrorMessage = "Username must not contain white spaces")]
        public string Username { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be greater than 8 Characters")]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{5,}$", ErrorMessage = "Password Must Contain one Lowercase, one Uppercase, one Number, one Symbol")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Password Doesn't Match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        [RegularExpression("^[a-z][a-zA-Z0-9._]+\\@[a-z]+\\.[a-z]{2,3}", ErrorMessage = "Enter Correct Email Id")]
        [DataType(DataType.EmailAddress)]
        [Remote("IsUserNameExist", "Auth", ErrorMessage = "Email already exists.", AdditionalFields = "uid")]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [DataType(DataType.PostalCode)]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "Pincode must be 6 digits")]
        public int Pincode { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "The city name must not contain any numbers or special characters.")]
        public string City { get; set; }

        [Required]
        [DisplayName("Phone Number")]
        [RegularExpression("^[0-9\\s]+$", ErrorMessage = "Phone Number must be a number")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNo { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss tt}")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; }

        [DisplayName("Last Updated At")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss tt}")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? DeletedOn { get; set; }

        public bool IsDeleted { get; set; }

        public Roles Roles { get; set; }
    }
}