using MVC_Task.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_Task.Models
{
    public class Books
    {

        private List<Books> _bookList;

        [DisplayName("ID")]
        public int BookId { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Book Title Must be Greater than 5 Characters")]
        public string Title { get; set; }

        [Required]
        [RegularExpression("^(?=(?:[^0-9]*[0-9]){10}(?:(?:[^0-9]*[0-9]){3})?$)[\\d-]+$", ErrorMessage = "Enter a Valid ISBN Number")]
        [DisplayName("ISBN")]
        public string Isbn { get; set; }

        [Required]
        [MinLength(5, ErrorMessage = "Author Name must be greater than 5 characters")]
        [DisplayName("Author Name")]
        public string AuthorName { get; set; }

        public Category Category { get; set; }

        [DataType(DataType.ImageUrl)]
        public string CoverImage { get; set; }

        public string TempImageURL { get; set; }

        [Required]
        [DisplayName("Cover Image")]
        public HttpPostedFileBase CoverImageFile { get; set; }

        [Required]
        [Range(1990, 2024, ErrorMessage = "Publication Year must be From 1990-2024")]
        [DisplayName("Publication Year")]
        public int? PublicationYear { get; set; }

        [DisplayName("Available Quantity")]
        [Required]
        [Range(1, 1000, ErrorMessage = "Enter Quantity Between 1-1000")]
        public int AvailableQty { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Enter Quantity Between 1-1000")]
        [DisplayName("Original Quantity")]
        public int OriginalQty { get; set; }

        [DisplayName("Created On")]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy HH:mm:ss tt}")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        [DisplayName("Last Updated At")]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy hh:mm:ss tt}")]
        [DataType(DataType.DateTime)]
        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        [DisplayName("Deleted On")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss tt}")]
        [DataType(DataType.DateTime)]
        public DateTime? DeletedOn { get; set; }

        public int? DeletedBy { get; set; }

        public int TotalPages { get; set; }

        public List<Books> BookList
        {
            get
            {
                return _bookList;
            }
            set { _bookList = value; }
        }

        /// <summary>
        /// Fetches the Category from the DB and converts them into SelectListItem for showing it into the DropDownlist for Filtering Purpose.
        /// </summary>
        public List<SelectListItem> CategoryList
        {
            get
            {
                List<Category> categories = BookRepository.CreateOrGetCategories<List<Category>>().Result;

                List<SelectListItem> selectListItems = categories.Select(category => new SelectListItem() { Text = category.CategoryName, Value = category.CategoryId.ToString() }).ToList();

                return selectListItems;

            }
        }

        public bool IsIssued { get; set; }

        public int TotalRecords { get; set; }

        public int TotalFilteredRecords { get; set; }

        public Issues Issues { get; set; }
    }
}