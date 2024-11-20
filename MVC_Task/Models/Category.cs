using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MVC_Task.Filters;

namespace MVC_Task.Models
{
    public class Category
    {
        [DisplayName("Category")]
        public int CategoryId { get; set; }

        [DisplayName("Category")]
        public string CategoryName { get; set; }
    }
}