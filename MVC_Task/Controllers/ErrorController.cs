using System.Web.Mvc;

namespace MVC_Task.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        /// <summary>
        /// View to Display Common Error
        /// </summary>
        /// <returns>Error Page</returns>
        public ActionResult Index()
        {
            return View("Error");
        }

        // GET: Error/ServerError
        /// <summary>
        /// Server Error Page whenever any action method fails will be redirected to these page.
        /// </summary>
        /// <returns>Server Error Page</returns>
        public ActionResult ServerError()
        {
            return View("ServerError");
        }

        // GET: Error/NotFound
        /// <summary>
        /// 404 Page when ever user tries to manipulate the page or change the url these page will be visible
        /// </summary>
        /// <returns>Not Found or 404 Page.</returns>
        public ActionResult NotFound()
        {
            return View();
        }
    }
}