using Dapper;
using MVC_Task.Database;
using MVC_Task.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace MVC_Task.Repository
{
    /// <summary>
    /// For managing the Operations based on Books like Creation of Category, Deletion of Book, Creation Of Book, Updation of Books and all methods related to Books.
    /// </summary>
    public class BookRepository
    {
        /// <summary>
        /// Creates or fetches the Categories, if categoryname is passed it will insert the category into the Category Table and returns the Last Inserted Category ID. If not passed it will fetch all the categories from the Category Table.
        /// </summary>
        /// <typeparam name="TResult">If Category Name Passed then pass the bool for indicating the Success of the Insert Query, otherwise List of Category Model to get all the category from the Table.</typeparam>
        /// <param name="catgoryname">Wants to Insert into the Category Table.</param>
        /// <returns>If no category name is provided, returns a list of categories (`IEnumerable<Category>`).
        /// If a category name is provided, returns a success flag and message along with the inserted category ID (`RepositoryResult<TResult>`).</returns>
        public static RepositoryResult<TResult> CreateOrGetCategories<TResult>(string catgoryname = null)
        {
            try
            {
                using (SqlConnection conn = Connection.GetConn())
                {

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@categoryname", catgoryname, DbType.String, ParameterDirection.Input);
                    parameters.Add("@issuccess", 0, DbType.Int32, ParameterDirection.Output);
                    parameters.Add("@insertedid", null, DbType.Int32, ParameterDirection.Output);


                    IEnumerable<Category> results = conn.Query<Category>("spCreateorgetCategory", parameters, commandType: CommandType.StoredProcedure);
                    int issuccess = parameters.Get<int>("@issuccess");
                    int insertedid = parameters.Get<int>("@insertedid");

                    if (issuccess == 1)
                    {
                        return new RepositoryResult<TResult>() { Result = typeof(TResult) == typeof(bool) ? (TResult)(object)true : (TResult)(object)results, Success = issuccess == 1, ReturnResult = insertedid };
                    }
                    return new RepositoryResult<TResult> { Success = false, Message = "Some Error Occured" };
                }
            }
            catch (Exception)
            {
                return new RepositoryResult<TResult> { Success = false, Message = "Some Error Occured" };
            }
        }

        /// <summary>
        /// Creates or updates book records.
        /// </summary>
        /// <param name="books">The book object containing book details.</param>
        /// <param name="toinsert">If true, inserts a new book. If false, updates an existing book.</param>
        /// <returns>
        /// Returns a boolean indicating whether the operation was successful (`true` if successful, `false` otherwise).
        /// </returns>
        public static bool CreateUpdateBooks(Books books, bool toinsert = false)
        {
            try
            {

                using (SqlConnection conn = Connection.GetConn())
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@title", books.Title, DbType.String);
                    parameters.Add("@isbn", books.Isbn, DbType.String);
                    parameters.Add("@categoryid", books.Category.CategoryId, DbType.Int32);
                    parameters.Add("@authorname", books.AuthorName, DbType.String);
                    parameters.Add("@coverimage", books.CoverImage, DbType.String);
                    parameters.Add("@publicationyear", books.PublicationYear, DbType.Int32);
                    parameters.Add("@originalQty", books.OriginalQty, DbType.Int32);
                    parameters.Add("@availableQty", books.AvailableQty, DbType.Int32);
                    parameters.Add("@updatedby", books.UpdatedBy, DbType.Int32);
                    parameters.Add("@createdby", books.CreatedBy, DbType.Int32);
                    parameters.Add("@toinsert", toinsert ? 1 : 0, DbType.Boolean);
                    parameters.Add("@bookid", books.BookId, DbType.Int32);
                    parameters.Add("@issuccess", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    int affectedRows = conn.Execute("spCreateUpdateBook", parameters, commandType: CommandType.StoredProcedure);
                    return parameters.Get<int>("@issuccess") == 1;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Retrieves book records, optionally filtered by various parameters.
        /// </summary>
        /// <param name="bookid">The ID of the book to retrieve.</param>
        /// <param name="userid">The ID of the user for filtering issued books.</param>
        /// <param name="issuedetails">Flag to include issue details.</param>
        /// <param name="pageno">Page number for pagination.</param>
        /// <param name="pagesize">Number of records per page.</param>
        /// <param name="sort">Sorting order.</param>
        /// <param name="title">Filter by book title.</param>
        /// <param name="authorname">Filter by author name.</param>
        /// <param name="category">Filter by category ID.</param>
        /// <typeparam name="TResult">Expected result type Can be List of Books if Book id is not passed other wise the Book object model.</typeparam>
        /// <returns>
        /// Returns a list of books List<Books> if no `bookid` is provided.
        /// Returns a single book Books if `bookid` is specified.
        /// If user and issue details are provided, returns a tuple of book and issue details Tuple<Books, Issues>.
        /// </returns>
        public static TResult GetBooks<TResult>(int bookid = 0, int userid = 0, int pageno = 0, int pagesize = 5, int sort = 0, string title = null, string authorname = null, int category = 0)
        {
            try
            {
                using (SqlConnection conn = Connection.GetConn())
                {

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@bookid", bookid, DbType.Int32);
                    parameters.Add("@userid", userid, DbType.Int32);
                    parameters.Add("@pageno", pageno, DbType.Int32);
                    parameters.Add("@pagesize", pagesize, DbType.Int32);
                    parameters.Add("@sort", sort, DbType.Int32);
                    parameters.Add("@title", title, DbType.String);
                    parameters.Add("@authorname", authorname, DbType.String);
                    parameters.Add("@category", category, DbType.Int32);

                    IEnumerable<Books> listofbooks = conn.Query<Books, Category, Issues, Books>("spGetBooks", (book, categoryofbook, issue) =>
                    {
                        book.Category = categoryofbook;
                        Issues issues = issue;
                        if (issues != null)
                        {
                            issues.BookId = book.BookId;
                            book.Issues = issues;
                        }
                        return book;
                    }, splitOn: "categoryid,id", param: parameters, commandType: CommandType.StoredProcedure);

                    return bookid != 0 ? (TResult)(object)listofbooks.FirstOrDefault() : (TResult)(object)listofbooks.ToList();
                }
            }
            catch (Exception)
            {
                return (TResult)(object)null;
            }
        }

        /// <summary>
        /// Deletes the Book From the Books Table 
        /// </summary>
        /// <param name="bookid">The Bookid which want to delete</param>
        /// <param name="userid">The userid which is deleting the Book</param>
        /// <returns>Returns boolean if the Deletion of the Book was Successful.</returns>
        public static bool DeleteBook(int bookid, int userid, out string errormsg)
        {
            try
            {
                using (SqlConnection conn = Connection.GetConn())
                {
                    conn.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@bookid", bookid, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@userid", userid, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@issuccess", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    int affectedRows = conn.Execute("spDeleteBook", parameters, commandType: CommandType.StoredProcedure);
                    errormsg = null;
                    return parameters.Get<int>("@issuccess") == 1;
                }
            }
            catch (Exception ex)
            {
                errormsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Issues or Returns the Book by providing the BookId which want to Issue and User id which is issuing or returning the Book.
        /// </summary>
        /// <param name="bookid">The Id of the Book to Issue Or Return</param>
        /// <param name="userid">The Id of the User for Issuing and Returning.</param>
        /// <param name="storedProcedure">The Name of the Stored Procedure to Execute to issue using 'spIssueBook' to Return use 'spReturnBook'</param>
        /// <returns>
        /// Repository<bool> result indicating whether Operation was successful also returns the Error Message if any.
        /// </returns>
        public static RepositoryResult<bool> IssueReturnBookNow(int bookid, int userid, string storedProcedure, int returnedbyid = 0)
        {
            try
            {
                using (SqlConnection conn = Connection.GetConn())
                {

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@bookid", bookid, DbType.Int32);
                    parameters.Add("@userid", userid, DbType.Int32);
                    parameters.Add("@returnerid", returnedbyid, DbType.Int32);
                    parameters.Add("@errormsg", "", dbType: DbType.String, direction: ParameterDirection.Output, size: 500);
                    parameters.Add("@issuccess", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    int affectedRows = conn.Execute(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
                    return new RepositoryResult<bool>() { Success = parameters.Get<int>("@issuccess") == 1, Message = parameters.Get<string>("@errormsg") };

                }
            }
            catch (Exception)
            {
                return new RepositoryResult<bool> { Success = false, Message = "Some Error Occured At Our End" };
            }
        }

        /// <summary>
        /// To List Down all the Books of the User which are issued.
        /// </summary>
        /// <param name="userid">The ID of User to Fetch the issued Books of it.</param>
        /// <returns>List of books issued by the user.</returns>
        public static List<Issues> GetIssuedBooksOfUser(int userid, string status = null)
        {
            try
            {
                using (SqlConnection con = Connection.GetConn())
                {
                    List<Issues> issues = con.Query<Issues, Books, Category, Issues>("spGetIssuedBooksByUser", (issue, book, category) =>
                    {
                        Books books = book;
                        books.Category = category;
                        issue.Books = books;
                        return issue;
                    }, new { userid = userid, status = status }, splitOn: "bookid,categoryid", commandType: CommandType.StoredProcedure).ToList();

                    return issues;
                }
            }
            catch (Exception)
            {
                return new List<Issues>();
            }
        }

        /// <summary>
        /// Retrieves all issues with optional filters, including title, status, and user name.
        /// Also returns the total number of issued books and total fine amount.
        /// </summary>
        /// <param name="totalissued">Outputs the total number of issued books.</param>
        /// <param name="totalfine">Outputs the total fine amount.</param>
        /// <param name="title">Filter by book title.</param>
        /// <param name="status">Filter by issue status .</param>
        /// <param name="pageno">Page number for pagination .</param>
        /// <param name="name">Filter by user name.</param>
        /// <returns>
        /// Returns a list of issues List<Issues>.
        /// Outputs the total issued books and total fine amounts via `totalissued` and `totalfine` parameters.
        /// </returns>
        public static List<Issues> GetAllIssues(out int totalissued, out int totalfine, string title = null, string status = null, int pageno = 0, string name = null, int userid = 0)
        {
            try
            {
                using (SqlConnection con = Connection.GetConn())
                {

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@status", status, DbType.String);
                    parameters.Add("@title", title, DbType.String);
                    parameters.Add("@name", name, DbType.String);
                    parameters.Add("@pageno", pageno, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@userid", userid, DbType.Int32, ParameterDirection.Input);
                    parameters.Add("@totalissued", 0, DbType.Int32, ParameterDirection.Output);
                    parameters.Add("@totalfine", 0, DbType.Int32, ParameterDirection.Output);

                    List<Issues> issues = con.Query<Issues>("spDashBoard", parameters, commandType: CommandType.StoredProcedure).ToList();

                    totalissued = parameters.Get<int>("@totalissued");
                    totalfine = parameters.Get<int>("@totalfine");
                    return issues;

                }
            }
            catch (Exception)
            {
                totalfine = 0;
                totalissued = 0;
                return new List<Issues>();
            }
        }
    }
}