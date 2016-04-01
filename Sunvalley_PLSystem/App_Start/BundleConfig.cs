using System.Web;
using System.Web.Optimization;

namespace Sunvalley_PLSystem
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.dataTables.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            /*Plantilla*/
                /*Estilos*/
            bundles.Add(new StyleBundle("~/app").Include(
                      "~/app/css/bootstrap.css"));


            bundles.Add(new StyleBundle("~/vendor").Include(
                      ""));

            /*Scripts*/
            bundles.Add(new ScriptBundle("~/app").Include(
                      ""));
            bundles.Add(new ScriptBundle("~/vendor").Include(
                      ""));

        }
    }
}
