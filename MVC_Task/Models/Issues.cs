using MVC_Task.Repository;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MVC_Task.Models
{

    /// <summary>
    /// Enum representing the various statuses of a book issue process.
    /// </summary>
    public enum IssueStatus
    {
        Issued,
        Return,
        LateReturn,
        DuePassed
    }

    public class Issues
    {
        [DisplayName("Issue ID")]
        public int Id { get; set; }
        public int UserId { get; set; }


        [DisplayName("Book ID")]
        public int BookId
        {
            get; set;
        }

        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy hh:mm:ss tt}")]
        [DisplayName("Issue Date")]
        public DateTime IssueDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy hh:mm:ss tt}")]
        [DisplayName("Due Date")]
        public DateTime? DueDate
        {
            get; set;
        }

        /// <summary>
        /// Represents the computed due date for an issued book, based on the maximum issue days from library settings.
        /// </summary>
        [DisplayName("Due Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy hh:mm:ss tt}")]
        public DateTime ComputedDueDate
        {
            get
            {
                return DateTime.Now.AddDays(LibrarySetting.MaxIssueDays);
            }
        }

        public string Status { get; set; }

        /// <summary>
        /// Enum representation of the issue status.
        /// Handles the conversion between string and IssueStatus enum.
        /// </summary>
        public IssueStatus IssueStatus
        {
            get
            {
                IssueStatus status;
                Enum.TryParse<IssueStatus>(Status, true, out status);
                return status;
            }
            set
            {
                this.Status = Enum.GetName(IssueStatus.GetType(), value);
            }
        }

        [DisplayName("Returned Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy hh:mm:ss tt}")]
        public DateTime? ReturnedDate { get; set; }

        [DisplayName("Fine Amount")]
        public int? FineAmount { get; set; }


        [DisplayName("Daily Fine Amount")]
        public int DailyFineAmount { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
        public int? DeletedBy { get; set; }

        public string Title { get; set; }

        [DisplayName("Issuer Name")]
        public string IssuedName { get; set; }

        public int TotalRecords { get; set; }

        /// <summary>
        /// Retrieves the library setting from the database using the AdminRepository.
        /// Applying the Setting ID as 1.
        /// </summary>
        public LibrarySetting LibrarySetting
        {
            get
            {
                RepositoryResult<LibrarySetting> results = AdminRepository.GetUpdateLibrarySetting<int, LibrarySetting>(1);
                return results.Result;
            }
        }

        /// <summary>
        /// Property to Calculate the Number of Days Passed after issuing the Book.
        /// </summary>
        [DisplayName("Days Passed After Due Date")]
        public int PassedDays
        {
            get
            {
                if (DueDate.HasValue)
                {

                    return (int)DateTime.Now.Subtract(DueDate.Value).TotalDays;
                }
                return 0;
            }
        }

        /// <summary>
        /// Computes the Fines Based on Passed Days and DailyFineAmount Property
        /// </summary>
        public int ComputedFineAmount { get => PassedDays * DailyFineAmount; }

        /// <summary>
        /// Displays Number of days left to return the books.
        /// </summary>
        [DisplayName("Returns In")]
        public int ReturnDays
        {
            get
            {
                if (DueDate.HasValue)
                {
                    return (int)DueDate.Value.Subtract(DateTime.Now).TotalDays;
                }
                return 0;
            }
        }

        public int ReturnedBy { get; set; }

        [DisplayName("Returned By")]
        public string ReturnedName { get; set; }
        public Books Books { get; set; }
    }

}