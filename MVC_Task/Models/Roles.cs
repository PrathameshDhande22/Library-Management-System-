using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVC_Task.Models
{
    public class Roles
    {
        public int RoleId { get; set; }

        [Required]
        [DisplayName("Role")]
        public string RoleName { get; set; }
    }
}