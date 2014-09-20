using System.Web;
using System.Web.Optimization;

namespace C2C.UI
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/scripts/C2C").Include(
                        "~/Scripts/C2C/jquery.simplemodal.js",
                        "~/Scripts/C2C/basic.js",
                        "~/Scripts/C2C/metrojs.js",
                        "~/Scripts/C2C/jquery.mousewheel.js",
                        "~/Scripts/C2C/global.js",
                        "~/Scripts/C2C/jquery.mCustomScrollbar.js",
                        "~/Scripts/C2C/c2c_properties.js",
                        "~/Scripts/C2C/c2c_browser_support.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/themes/base/styles/site.css",
                        "~/Content/themes/base/styles/modern.css",
                        "~/Content/themes/base/styles/modern-responsive.css",
                        "~/Content/themes/base/styles/metrojs.css",
                        "~/Content/themes/base/styles/jquery.mCustomScrollbar.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"));

            // TODO: Uncomment to enable optimization when moving to production.
            //BundleTable.EnableOptimizations = true;
        }
    }
}