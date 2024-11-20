using MVC_Task.Models;
using MVC_Task.Models.ViewModels;
using MVC_Task.Repository;
using MVC_Task.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;


namespace MVC_Task.Controllers
{
    public class UserController : Controller
    {

        // GET: User/ChangePassword
        /// <summary>
        /// Changes the Password of the Currently LoggedIn user. Passing the LoggedIn User id and Username for sending the Post request to verify the details.
        /// </summary>
        /// <returns>View with Old Password, New Password fields to validate</returns>
        [HttpGet]
        [Authorize]
        public ActionResult ChangePassword()
        {
            try
            {
                int userid = Convert.ToInt32(User.Identity.Name);
                ChangePasswordViewModel changePasswordviewmodel = new ChangePasswordViewModel() { Id = userid };
                return View(changePasswordviewmodel);
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        // POST: User/ChangePassword
        /// <summary>
        /// Validates the Data Passed by User for the ChangePassword Request by validating the Old and New password, updates to New password for the currently logged in User.
        /// </summary>
        /// <param name="changepassword">ChangePassword Model for the Data Filled by User</param>
        /// <returns>Validates the Passwords and according to it returns the Message and redirects to the Login Page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult ChangePassword(ChangePasswordViewModel changepassword)
        {
            try
            {
                if (changepassword.Id == 0)
                {
                    FormsAuthentication.SignOut();
                    return RedirectToAction("Index", "Error");
                }
                else if (ModelState.IsValid)
                {
                    if (!UserRepository.IsPasswordCorrect(changepassword.OldPassword, changepassword.Id))
                    {
                        ModelState.AddModelError("oldpassword", "Password is Incorrect");
                    }
                    else if (changepassword.OldPassword == changepassword.Password)
                    {
                        TempData["Message"] = new Alert("Old Password and New Password must be different", Status.Warning);
                    }
                    else
                    {
                        if (UserRepository.UpdatePassword(changepassword.Id, changepassword.Password, changepassword.OldPassword))
                        {
                            TempData["Message"] = new Alert("Password Changed Successfully ! Login with New Password", Status.Success);
                            FormsAuthentication.SignOut();
                            return RedirectToAction("Login", "Auth");
                        }
                        else
                        {
                            ViewData["Message"] = new Alert("Some Errored Occured While Changing Password Please Try Again Later .", Status.Danger);
                        }
                    }
                }
                return View(changepassword);
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        // GET: User/Profile
        /// <summary>
        /// Fetches the Currently LoggedIn User Profile and Displays it.
        /// </summary>
        /// <returns>Currently LoggedIn User Profile.</returns>
        [HttpGet]
        [ActionName("Profile")]
        [Authorize]
        public ActionResult UserProfile()
        {
            try
            {
                int userid = Convert.ToInt32(User.Identity.Name);
                User user = UserRepository.GetUsersOrSingleDetails<User>(userid, true);
                if (user == null)
                {
                    TempData["Message"] = new Alert("Some Errored Occurred While Fetching Records Please Try Again Later .", Status.Danger);
                }
                return View("UserProfile", user);
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        // POST: User/Profile
        /// <summary>
        /// Updates the User Profile for the Currently LoggedIn User by getting the User Id.
        /// </summary>
        /// <param name="user">User Model With Its Updated Details to Update.</param>
        /// <returns>Successful Message if Update is Successful.</returns>
        [HttpPost]
        [ActionName("Profile")]
        [Authorize]
        public ActionResult UserProfile([Bind(Exclude = "Password, ConfirmPassword")] User user)
        {
            try
            {
                ModelState.Remove("Password");
                ModelState.Remove("ConfirmPassword");
                if (ModelState.IsValid)
                {
                    UsernameEmailValidation usernameEmailValidation = UserRepository.IsUsernameOrEmailExist(user.Username, user.Email, Convert.ToInt32(User.Identity.Name));

                    if (usernameEmailValidation.IsUsernameTaken)
                    {
                        Utilities.AddCookie(Response, "iseditable", "true");
                        ModelState.AddModelError("username", "Username already exist please try different username");
                    }
                    if (usernameEmailValidation.IsEmailTaken)
                    {
                        Utilities.AddCookie(Response, "iseditable", "true");
                        ModelState.AddModelError("email", "Email already exists");
                    }
                    if (!usernameEmailValidation.IsEmailTaken && !usernameEmailValidation.IsUsernameTaken)
                    {
                        Utilities.AddCookie(Response, "iseditable", "false");
                        if (UserRepository.UpdateUserProfile(user, 0, null))
                        {
                            FormsAuthentication.SetAuthCookie(user.Id.ToString(), true);
                            HttpCookie cookie = new HttpCookie("details", user.Username);
                            Response.Cookies.Add(cookie);
                            TempData["Message"] = new Alert("Profile updated Successfully ! ", Status.Success);
                            return RedirectToAction("Profile", "User");
                        }
                        else
                        {
                            TempData["Message"] = new Alert("Some Errored Occured While Updating. Please Try Again Later .", Status.Danger);
                        }
                    }
                }
                return View("UserProfile", user);
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        // GET: User/Listing
        /// <summary>
        /// Listing all the User execpt the currently LoggedIn Admin. Also applies the filter by role.
        /// </summary>
        /// <param name="role">Which User Want According to Their Role.</param>
        /// <returns>List of User with Filtering.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Listing(string role)
        {
            try
            {
                List<User> users = UserRepository.GetUsersOrSingleDetails<List<User>>();
                users = String.IsNullOrWhiteSpace(role) ? users : users.Where(u => u.Roles.RoleName == role).ToList();
                return View(users);
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        // POST: User/Delete/id
        /// <summary>
        /// Delete the User by passing the Id can be done by Admin only.
        /// </summary>
        /// <param name="id">Id of the user for which you want to delete.</param>
        /// <returns>Successful or Unsuccessful Json Message, if the Deletion was successful.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                int loggedinid = Convert.ToInt32(User.Identity.Name);
                if (loggedinid == id)
                {
                    TempData["Message"] = new Alert("You Cannot Delete Yourself", Status.Warning);
                    return Json(new { status = "Failed", message = "You cannot Delete Yourself" }, JsonRequestBehavior.AllowGet);
                }
                else if (UserRepository.DeleteUser(id))
                {
                    TempData["Message"] = new Alert("User Deleted Successfully", Status.Success);
                    return Json(new { status = "Success", message = "User Successfully Deleted" }, JsonRequestBehavior.AllowGet);
                }
                TempData["Message"] = new Alert("Cannot Delete the User, User has issued the books.", Status.Warning);
                return Json(new { status = "Failed", message = "Cannot Delete the User, User has issued the books." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                TempData["Message"] = new Alert("Error Occured while Deleting Record Please Try after some time", Status.Danger);
                return Json(new { status = "Failed", message = "Server Errored while processing" }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: User/Edit?userid=11
        /// <summary>
        /// Edit the User Details by Passing its User id. If the Userid is not present then shows User Details Not Found.
        /// </summary>
        /// <param name="userid">Fetches the Details of the User by passing the User id.</param>
        /// <returns>Details of the User based on its User id.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int userid = 0)
        {
            try
            {
                int currentlogin = Convert.ToInt32(User.Identity.Name);
                if (userid == currentlogin)
                {
                    return RedirectToAction("Profile");
                }
                else if (userid == 0)
                {
                    return View("UserProfile", null);
                }
                User user = UserRepository.GetUsersOrSingleDetails<User>(userid, true);

                return View("UserProfile", user ?? null);
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        // POST: User/Edit
        /// <summary>
        /// Edited User Details To update the User Profile on the admin side. 
        /// </summary>
        /// <param name="user">User Model to get the all the Edited/Updated Fields to update the User.</param>
        /// <returns>Updated User Records Message other wise Error.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            try
            {
                if (user.Id == 0)
                {
                    TempData["Message"] = new Alert("Some Errored occured while updating the User", Status.Danger);
                    return View("UserProfile", user);
                }
                ModelState.Remove("confirmpassword");
                if (ModelState.IsValid)
                {
                    int currentloginid = Convert.ToInt32(User.Identity.Name);
                    UsernameEmailValidation usernameEmailValidation = UserRepository.IsUsernameOrEmailExist(user.Username, user.Email, user.Id);

                    if (usernameEmailValidation.IsUsernameTaken)
                    {
                        Utilities.AddCookie(Response, "iseditable", "true");
                        ModelState.AddModelError("username", "Username already exist please try different username from backend");
                    }
                    if (usernameEmailValidation.IsEmailTaken)
                    {
                        Utilities.AddCookie(Response, "iseditable", "true");
                        ModelState.AddModelError("email", "Email already exists");
                    }
                    if (!usernameEmailValidation.IsEmailTaken && !usernameEmailValidation.IsUsernameTaken)
                    {

                        Utilities.AddCookie(Response, "iseditable", "false");
                        if (UserRepository.UpdateUserProfile(user, currentloginid, user.Roles.RoleName))
                        {
                            TempData["Message"] = new Alert("Profile updated Successfully ! ", Status.Success);
                        }
                        else
                        {
                            TempData["Message"] = new Alert("Some Errored Occured While Updating the user. Please Try Again Later .", Status.Danger);
                        }
                    }
                }
                else
                {
                    Utilities.AddCookie(Response, "iseditable", "true");
                }
                return View("UserProfile", user);
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        // GET: User/Dashboard
        /// <summary>
        /// To get all the Issue Details using which Librarian or Admin Can return the books based on User or can View the total Books Issued, Also Supports Various Filters.
        /// </summary>
        /// <param name="title">Searching the Books which were issued based on its title</param>
        /// <param name="status">Searching the Books which were Issued or LateReturn or Return</param>
        /// <param name="pageno">Getting the Details based on Pageno</param>
        /// <param name="name"><Searching the Issues by the Issuer Name.</param>
        /// <returns>List of Issues if there are Issues.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Librarian,User")]
        public ActionResult Dashboard(string title = null, string status = null, int pageno = 0, string name = null)
        {
            try
            {
                int totalfine = 0, totalissue = 0;
                pageno = Math.Max(pageno, 1);
                ViewBag.Pageno = pageno;

                int currentLoginid = Convert.ToInt32(User.Identity.Name);

                List<Issues> issues = BookRepository.GetAllIssues(out totalissue, out totalfine, title, string.IsNullOrWhiteSpace(status) ? null : status, pageno - 1, User.IsInRole("User") ? null : name, User.IsInRole("User") ? currentLoginid : 0);

                int totalRecoreds = issues.FirstOrDefault()?.TotalRecords ?? 0;
                int pageSize = 10;
                ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecoreds / pageSize);
                ViewData["records"] = totalRecoreds;
                ViewBag.Stats = new int[] { totalfine, totalissue };
                return View(issues);
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }
    }
}