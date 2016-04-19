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
                        "~/Scripts/jquery-{version}.js"));

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
            bundles.Add(new StyleBundle("~/appEstilos").Include(
                      "~/app/css/bootstrap.css",
                      "~/app/css/app.css",
                      "~/app/css/print.css"));


            bundles.Add(new StyleBundle("~/vendorEstilos").Include(
                      "~/vendor/animo/animate_animo.css",
                      "~/vendor/csspinner/csspinner.min.css"));

            /*Scripts*/
            bundles.Add(new ScriptBundle("~/appScript").Include(
                      "~/app/js/app.js"));

            bundles.Add(new ScriptBundle("~/vendorScript").Include(
                        "~/vendor/modernizr/modernizr.js",
                      "~/vendor/fastclick/fastclick.js",
                      //"~/vendor/jquery/jquery.min.js",
                      "~/vendor/bootstrap/js/bootstrap.min.js",
                      //"~/vendor/chosen/chosen.jquery.min.js",
                      "~/vendor/slider/js/bootstrap-slider.js",
                      "~/vendor/filestyle/bootstrap-filestyle.min.js",
                      "~/vendor/animo/animo.min.js",
                      "~/vendor/sparklines/jquery.sparkline.min.js",
                      "~/vendor/slimscroll/jquery.slimscroll.min.js"));

        }
    }
}
