using System.Collections.Generic;

namespace MVC_Task.Models.ViewModels
{
    public class HistoryOfIssuedBooksViewModel
    {
        public List<Issues> Issues { get; set; }
        public List<Books> Books { get; set; }
    }
}