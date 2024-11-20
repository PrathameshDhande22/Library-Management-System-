using MVC_Task.Filters;
using MVC_Task.Models;
using MVC_Task.Models.ViewModels;
using MVC_Task.Repository;
using System.Web.Mvc;
using System.Web.Security;
using System;
using System.Web;

namespace MVC_Task.Controllers
{

    public class AuthController : Controller
    {
        // GET: Auth/Login
        /// <summary>
        ///  To login View Form into the system using their Username and Password.
        /// </summary>
        /// <returns>View of Username textbox and Password Box</returns>
        [HttpGet]
        [CheckLogedIn]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Auth/Login
        /// <summary>
        /// Gain the access to the system by providing the username/email and password, based on role displays the view. 
        /// </summary>
        /// <param name="loginvw">LoginViewModel which contains the Username and password field which are binded by form.</param>
        /// <returns>Success Or UnSuccess Message if the Login is Success then Returns to the Home Page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckLogedIn]
        public ActionResult Login(LoginViewModel loginvw)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    LoginViewModel model = UserRepository.LoginUser(loginvw);
                    if (model != null)
                    {
                        HttpCookie httpCookie = new HttpCookie("details", model.Username);
                        FormsAuthentication.SetAuthCookie(model.Id.ToString(), true);
                        Response.Cookies.Add(httpCookie);
                        return RedirectToAction("Index", "Book");
                    }
                    else
                    {
                        ModelState.AddModelError("username", "Invalid Username");
                        ModelState.AddModelError("password", "Invalid Password");
                        return View(loginvw);
                    }
                }
                return View(loginvw);
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        // GET: Auth/Register
        /// <summary>
        /// Renders the Form required for Registering.
        /// </summary>
        /// <returns>View with the Register Form.</returns>
        [HttpGet]
        [CheckLogedIn]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Auth/Register
        /// <summary>
        /// Validates the Various fields which are required for registering.
        /// </summary>
        /// <param name="user">Model which Contains the Various Property required for registering the User into the System.</param>
        /// <returns>Registers the users if Model is valid then redirect to Home Page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [CheckLogedIn]
        public ActionResult Register(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool isUsernameExist = (bool)IsUserNameExist(username: user.Username).Data;
                    bool isEmailExist = (bool)IsUserNameExist(email: user.Email).Data;

                    if (isUsernameExist == false)
                    {
                        ModelState.AddModelError("username", "username already exists try new username");
                    }
                    if (isEmailExist == false)
                    {
                        ModelState.AddModelError("email", "Email already exists");
                    }
                    if (isUsernameExist && isEmailExist)
                    {
                        if (UserRepository.RegisterUser(user))
                        {

                            TempData["Message"] = new Alert("User Register Successfully ! Login Now", Status.Success);
                            return RedirectToAction(nameof(Login));
                        }
                        else
                        {
                            ViewData["Message"] = new Alert("Some Errored Occured While Registering Please Try Again Later .", Status.Danger);
                        }

                    }

                }
                return View(user);
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        // GET: Auth/IsUsernameExist
        /// <summary>
        /// Checks if the Passed Username or email exists in the system also to exclude the Username or email of passed user id
        /// </summary>
        /// <param name="uid">Userid for which you want to exlcude the checking the username or email</param>
        /// <param name="username">Username to check if the passed username exists in the system.</param>
        /// <param name="email">Email to Check if the Passed Email exists in the System.</param>
        /// <returns>The Result as Json if True then Passed the Username and Email is Available to register otherwise <paramref name="username"/>/<paramref name="email"/> is false then it is already taken by user. </returns>
        public JsonResult IsUserNameExist(int uid = 0, string username = null, string email = null)
        {

            UsernameEmailValidation usernameEmailValidation = UserRepository.IsUsernameOrEmailExist(username, email, uid);


            return Json(String.IsNullOrWhiteSpace(username) ? !usernameEmailValidation.IsEmailTaken : !usernameEmailValidation.IsUsernameTaken, JsonRequestBehavior.AllowGet);
        }


        // POST : Auth/Logout
        /// <summary>
        /// Logs out from the System
        /// </summary>
        /// <returns>Redirects to the Home Page after logging out.</returns>
        [Authorize]
        [HttpPost]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();

            TempData["Message"] = new Alert("LogOut successful !", Status.Success);
            return RedirectToAction("Index", "Book", null);
        }
    }
}