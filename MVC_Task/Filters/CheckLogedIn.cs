using System.Web.Mvc;

namespace MVC_Task.Filters
{
    /// <summary>
    /// Checks if the User is Logged In then redirect the Protected Route to NotFound Page. Thus Prevents the Unauthorized Access.
    /// </summary>
    public class CheckLogedIn : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("~/Error/NotFound");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}