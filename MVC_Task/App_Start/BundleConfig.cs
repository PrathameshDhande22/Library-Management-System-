using System.Web.Optimization;

namespace MVC_Task
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.*",
                      "~/Content/bootstrap-*",
                      "~/Content/site.css"));

            // Custom function which are available
            bundles.Add(new ScriptBundle("~/bundles/functions").Include("~/Scripts/index.js"));

            // Bundling the Script Files
            bundles.Add(new ScriptBundle("~/bundles/jsfiles").Include("~/Scripts/Book/CreateBook.js",
                                                                      "~/Scripts/Book/Details.js",
                                                                      "~/Scripts/User/ChangePassword.js",
                                                                      "~/Scripts/User/Dashboard.js",
                                                                      "~/Scripts/User/Listing.js",
                                                                      "~/Scripts/User/Profile.js",
                                                                      "~/Scripts/Auth/Login.js",
                                                                      "~/Scripts/Auth/Register.js"));

            // bundling the intelinput css
            //bundles.Add(new ScriptBundle("~/bundles/intelinput").Include("~/Scripts/IntlTelInput/intlTelInputWithUtils.min.js"));


            BundleTable.EnableOptimizations = true;
        }
    }
}
