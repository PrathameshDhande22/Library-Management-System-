using MVC_Task.Models;
using MVC_Task.Repository;
using Newtonsoft.Json;
using System.Web.Mvc;
using System;

namespace MVC_Task.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin/Setting
        /// <summary>
        /// Used for Fetching the Setting of the Library by passing the Setting Id of which we want to apply the setting.
        /// </summary>
        /// <returns>Views for getting the Setting which is updatable.</returns>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Setting()
        {
            try
            {
                RepositoryResult<LibrarySetting> result = AdminRepository.GetUpdateLibrarySetting<int, LibrarySetting>(1);
                if (result.Success)
                {
                    LibrarySetting setting = result.Result;
                    return View(setting);
                }
                else
                {
                    TempData["Message"] = new Alert("Some Errored Occurred While Fetching Records Please Try Again Later .", Status.Danger);
                }
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }

        // POST: Admin/Setting
        /// <summary>
        /// For Updating the Library Setting by passing the Library setting Model
        /// </summary>
        /// <param name="setting">Setting Which want to update by getting these through form by post request</param>
        /// <returns>The View if the Update is successfull or not</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Setting(LibrarySetting setting)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RepositoryResult<bool> result = AdminRepository.GetUpdateLibrarySetting<LibrarySetting, bool>(setting);

                    TempData["Message"] = result.Success
                        ? new Alert("Settings updated successfully!", Status.Success)
                        : new Alert("An error occurred while updating records. Please try again later.", Status.Danger);
                }
                return View(setting);
            }
            catch (Exception)
            {
                return RedirectToAction("ServerError", "Error");
            }
        }
    }
}