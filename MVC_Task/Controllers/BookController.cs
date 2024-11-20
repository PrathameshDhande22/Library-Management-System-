using MVC_Task.Models;
using MVC_Task.Models.ViewModels;
using MVC_Task.Repository;
using MVC_Task.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVC_Task.Controllers
{

    public class BookController : Controller
    {

        // GET: Book
        /// <summary>
        /// Home Page for the Books. Fetches and Displays the Books according to the passed filter.
        /// </summary>
        /// <param name="categoryby">To Filter by Category Provided by CategoryList takes the categoryid present in Category Table</param>
        /// <param name="sort">Sorting the Books According to Passed Sort number.</param>
        /// <param name="name">Searching the Book according to Author Name.</param>
        /// <param name="booktitle">Searching the Book According Book Title</param>
        /// <param name="pageno">For Which Page want to display the list.</param>
        /// <returns>View for displaying the books by passing the List of Books.</returns>
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index(int categoryby = 0, int sort = 0, string name = null, string booktitle = null, int pageno = 0)
        {
            try
            {
                Books books = new Books();
                int pageSize = 12;
                pageno = pageno <= 0 ? 1 : pageno;
                ViewBag.Pageno = pageno;
                ViewBag.Category = books.CategoryList;

                books.BookList = BookRepository.GetBooks<List<Books>>(0, User.Identity.IsAuthenticated ? Convert.ToInt32(User.Identity.Name) : 0, pageno - 1, 12, sort, booktitle, name, categoryby);

                int totalRecords = books.BookList.FirstOrDefault()?.TotalFilteredRecords ?? 0;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                ViewData["records"] = totalRecords;
                return View(books.BookList);
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        // GET: Book/Create
        /// <summary>
        /// These Action method Creates and Displays all the Books for Admin and Librarian Side. Also applied some filter on it.
        /// </summary>
        /// <param name="id">BookID to fetch the details of the Passed ID and autofill the details if these id is passed then it will act as Update Action Method.</param>
        /// <param name="pageno">Page no To for Displaying the records of the Passed pageno</param>
        /// <param name="categoryby">Filters the Books by the Category Id present in the Category Table.</param>
        /// <param name="sort">Number to be passed for Filtering based on A-z and many more.</param>
        /// <param name="name">Searching the Book Details by its Author Name</param>
        /// <returns>List of Books if Id is not passed else List Of Book along with the details of the Passed <paramref name="id"/> Book.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Librarian")]
        public ActionResult Create(int? id, int pageno = 1, int categoryby = 0, int sort = 0, string name = null)
        {
            try
            {
                int pageSize = 5;
                Books books = new Books();
                ViewData["toupdate"] = false;
                pageno = pageno <= 0 ? 1 : pageno;
                ViewBag.Category = books.CategoryList;

                books.BookList = BookRepository.GetBooks<List<Books>>(pageno: pageno - 1, sort: sort, authorname: name, category: categoryby);

                int totalRecoreds = books.BookList.FirstOrDefault()?.TotalFilteredRecords ?? 0;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecoreds / pageSize);
                ViewData["records"] = totalRecoreds;

                if (id.HasValue && id.Value != 0)
                {
                    Books fetchedBook = books.BookList.Where(book => book.BookId == id.Value).FirstOrDefault();
                    if (fetchedBook != null)
                    {
                        fetchedBook.BookList = books.BookList;
                        books = fetchedBook;
                        Session["toupdate"] = true;
                    }
                    else
                    {
                        TempData["Message"] = new Alert("Book Does Not Exist", Status.Warning);
                    }
                }
                return View(books);
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        // POST: Book/Create
        /// <summary>
        /// Creates or Updates the book and Image Upload File Validation with Persist Functions and their functions to handle.
        /// </summary>
        /// <param name="book">The Book Model which want to Update by its id. along with other details.</param>
        /// <returns>List of Book and Success Message if the update or Creation is successfull.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Librarian")]
        public ActionResult Create(Books book)
        {
            try
            {
                bool toupdate = Session["toupdate"] != null && bool.TryParse(Session["toupdate"].ToString(), out bool result) ? result : false;

                ViewBag.Category = book.CategoryList;

                int pageSize = 5;

                book.BookList = BookRepository.GetBooks<List<Books>>(pageno: 0);

                int totalRecoreds = book.BookList.FirstOrDefault()?.TotalFilteredRecords ?? 0;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecoreds / pageSize);
                ViewData["records"] = totalRecoreds;

                if (book.Category == null || book.Category.CategoryId == 0)
                {
                    ModelState.AddModelError("Category.CategoryId", "Select any One Category");
                }
                if (book.CoverImageFile != null && Utilities.IsFileValid(book.CoverImageFile))
                {
                    if (!String.IsNullOrWhiteSpace(book.CoverImage))
                    {
                        Utilities.DeleteFile(book.CoverImage, Server);
                    }
                    string savedPath = Utilities.SaveFile("~/Content/TempFiles", book.CoverImageFile, Server);
                    if (savedPath == null)
                    {
                        TempData["Message"] = new Alert("Failed to Upload File", Status.Danger);
                        return View(book);
                    }
                    else
                    {
                        book.TempImageURL = savedPath;
                    }
                }
                else if ((book.CoverImageFile == null && book.TempImageURL != null) || !String.IsNullOrEmpty(book.CoverImage))
                {
                    ModelState.Remove("CoverImageFile");
                }
                else
                {
                    ModelState.AddModelError("CoverImageFile", "You can only upload .jpg/.jpeg/.png files only");
                }
                if (toupdate)
                {
                    ModelState.Remove("OriginalQty");
                }
                else
                {
                    ModelState.Remove("AvailableQty");
                }
                if (ModelState.IsValid)
                {
                    if (book.TempImageURL != null || !String.IsNullOrEmpty(book.CoverImage))
                    {
                        int currentid = Convert.ToInt32(User.Identity.Name);
                        if (book.TempImageURL != null)
                        {
                            string savedPath = Utilities.SaveFileToPermanent("~/Content/BookImages", book.TempImageURL, Server);
                            if (savedPath == null)
                            {
                                TempData["Message"] = new Alert("Failed to Upload File", Status.Danger);
                                return View(book);
                            }
                            book.CoverImage = savedPath;
                        }
                        book.CreatedBy = currentid;
                        book.UpdatedBy = currentid;

                        if (BookRepository.CreateUpdateBooks(book, !toupdate))
                        {
                            TempData["Message"] = new Alert(toupdate ? "Book Updated Successfully !" : "Book Created Successfully !", Status.Success);
                            Session["toupdate"] = false;
                            return RedirectToAction("Create", new { id = 0 });
                        }
                    }
                    else
                    {
                        TempData["Message"] = new Alert("Some Error Occured at our End Please Try After Some Time", Status.Danger);
                    }
                }

                return View(book);
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        // GET: Book/CategoryAdd
        /// <summary>
        /// Adds the Category of the passed the Category name if the same category is passed then returns the Message according to it. If the inserting is successful then return the Category Inserted Id.
        /// </summary>
        /// <param name="category">Name of the Category to Add</param>
        /// <returns>Result in the form of JSON containing the status which can be either Failed or Success with an message and Inserted Category Id.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Librarian")]
        public JsonResult CategoryAdd(string category = null)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(category))
                {
                    return Json(new { status = "Failed", message = "Empty Category" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    RepositoryResult<bool> result = BookRepository.CreateOrGetCategories<bool>(category);
                    if (result.Success)
                    {
                        return Json(new { status = "Success", message = "Category Inserted", insertedid = result.ReturnResult }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { status = "Failed", message = result.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json(new { status = "Failed", message = "Server Issue At Our End" }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Book/DeleteBook/id
        /// <summary>
        /// Deletes the Book of the specified Book id
        /// </summary>
        /// <param name="id">The Id to be passed of the Book for which you want to delete the book.</param>
        /// <returns>Json Result containing Status which can be either Failed or Success with an Message if the deletion was Successful or not.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Librarian")]
        public JsonResult DeleteBook(int? id)
        {
            try
            {
                if (id.HasValue && id.Value != 0)
                {
                    int currentuser = Convert.ToInt32(User.Identity.Name);
                    string errormsg = null;
                    if (BookRepository.DeleteBook(id.Value, currentuser, out errormsg))
                    {
                        TempData["Message"] = new Alert("Book Deleted SuccessFully !", Status.Success);
                        return Json(new { status = "success", message = "Book Deleted Successfully" }, JsonRequestBehavior.AllowGet);
                    }
                    else if (!string.IsNullOrWhiteSpace(errormsg))
                    {
                        TempData["Message"] = new Alert(errormsg, Status.Danger);
                        return Json(new { status = "Success", message = errormsg }, JsonRequestBehavior.AllowGet);
                    }
                }
                TempData["Message"] = new Alert("Failed to Delete Book", Status.Danger);
                return Json(new { status = "failed", message = "Failed to Delete Book" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { status = "failed", message = "Error Occured While Deleting the Book" }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Book/Details/id
        /// <summary>
        /// Gives the Details of the Book Along with its Issue Details if the Role is User.
        /// </summary>
        /// <param name="id">Book Id to be passed to get the details</param>
        /// <returns>Details of the Book according to the LoggedIn User Role.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Librarian,User")]
        public ActionResult Details(int? id)
        {
            try
            {
                TempData["backurl"] = Request.UrlReferrer != null ? Request.UrlReferrer.AbsoluteUri.ToString() : Url.Action("Index", "Book");
                Books results = BookRepository.GetBooks<Books>(bookid: id.Value, Convert.ToInt32(User.Identity.Name));
                BookDetailsViewModel bookDetailsViewModel = new BookDetailsViewModel() { Book = results, Issue = results.Issues ?? new Issues() };

                return View(bookDetailsViewModel);
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        // POST: Book/IssueBook/id
        /// <summary>
        /// Issues the book for the Logged In User by passing the Book id as the parameter and redirects to the back url passed by Details Action method. 
        /// </summary>
        /// <param name="id">The Book ID Want to Issue the User</param>
        /// <returns>Doesn't return After Issuing is Successful redirects to the backurl otherwise redirect to Home Page.</returns>
        [HttpPost]
        [Authorize(Roles = "User")]
        public ActionResult IssueBook(int? id)
        {
            try
            {
                TempData["Message"] = new Alert("Cannot Process the Issue Request", Status.Danger);
                string backurl = TempData["backurl"]?.ToString() ?? Url.Action("Index", "Book");
                if (id.HasValue && User.IsInRole("User"))
                {
                    RepositoryResult<bool> results = BookRepository.IssueReturnBookNow(id.Value, Convert.ToInt32(User.Identity.Name), "spIssueBook");
                    TempData["Message"] = new Alert(results.Message, results.Success ? Status.Success : Status.Danger);
                }
                return Redirect(backurl);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Book");
            }
        }

        // POST: Book/ReturnBook/id
        /// <summary>
        /// Returns the Book Which was issued by the User. Can be called by User,Admin or Librarian by passing the Book Id which want to return then redirects to the Back URL passed by Details Action method. When Only Book Id is passed it will indirectly take the loginUser id and return the book to the Rack. 
        /// </summary>
        /// <param name="id">Book Id which want to Return after issuing.</param>
        /// <param name="userid">User ID for which the User want to Return Book.</param>
        /// <returns>After Successful or Unsuccessful redirects to the BackUrl Passed through TempData. Show message if Unsuccessful.</returns>
        [HttpPost]
        [Authorize(Roles = "User,Admin,Librarian")]
        public ActionResult ReturnBook(int? id, int? userid)
        {
            try
            {
                int currentuser = Convert.ToInt32(User.Identity.Name);
                string backurl = TempData["backurl"]?.ToString() ?? Url.Action("Index", "Book");
                if (!id.HasValue)
                {
                    TempData["Message"] = new Alert("Invalid Book Id", Status.Danger);

                }
                else if (userid.HasValue && userid.Value > 0 && id.HasValue && id.Value > 0)
                {
                    RepositoryResult<bool> results = BookRepository.IssueReturnBookNow(id.Value, userid.Value, "spReturnBook", currentuser);
                    TempData["Message"] = new Alert(results.Message, results.Success ? Status.Success : Status.Danger);
                    return RedirectToAction("Dashboard", "User");
                }
                else if (id.HasValue && User.IsInRole("User"))
                {

                    RepositoryResult<bool> results = BookRepository.IssueReturnBookNow(id.Value, currentuser, "spReturnBook");
                    TempData["Message"] = new Alert(results.Message, results.Success ? Status.Success : Status.Danger);
                }
                return Redirect(backurl);
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }


    }
}