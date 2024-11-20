using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVC_Task.Models
{
    public class LibrarySetting
    {

        public int SettingId { get; set; }

        [Required]
        [DisplayName("Max Issue Days")]
        [Range(2, 30, ErrorMessage = "You can assign between 5-30 days")]
        public int MaxIssueDays { get; set; }

        [Required]
        [DisplayName("Daily Fine Amount")]
        [Range(2, int.MaxValue, ErrorMessage = "Enter Fine greater than 5 Rs")]
        public int DailyFineAmount { get; set; }

        [Required]
        [DisplayName("Max Book Per User")]
        [Range(2, 15, ErrorMessage = "You can assign between 5-15 Books")]
        public int MaxBookPerUser { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy hh:mm:ss tt}")]
        [DisplayName("Last Updated At")]
        public DateTime? UpdatedOn { get; set; }

    }
}