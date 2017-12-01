using System.Web;
using System.Web.Optimization;

namespace ProyectoPersonal
{
    public class BundleConfig
    {
        // Para obtener más información sobre Bundles, visite http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // preparado para la producción y podrá utilizar la herramienta de compilación disponible en http://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/jquery-1.10.16.DataTables.min.js",
                      "~/Scripts/DataTables-1.10.16.bootstrap4.min.js",
                      "~/Scripts/Template/jsTemplatewidgetapi.js",
                      //"~/Scripts/Template/jquery.themepunch.tools.min.js",
                      //"~/Scripts/Template/jquery.themepunch.revolution.min.js",
                      //"~/Scripts/Template/isotope.pkgd.min.js",
                      //"~/Scripts/Template/owl.carousel.js",
                      //"~/Scripts/Template/jquery.magnific-popup.min.js",
                      //"~/Scripts/Template/jquery.appear.js",
                      //"~/Scripts/Template/jquery.countTo.js",
                      //"~/Scripts/Template/jquery.parallax-1.1.3.js",
                      "~/Scripts/Template/jquery.validate.js",
                      "~/Scripts/Template/jquery.browser.js",
                      "~/Scripts/Template/SmoothScroll.js",
                      "~/Scripts/Template/template.js",
                      "~/Scripts/Template/style-switcher.js"
                      
                      ));
            //nuevas librerias por el template
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      
                "~/Content/bootstrap.css",
                "~/Content/DataTables-1.10.16.bootstrap4.min.css",
                      "~/Content/site.css",
                      "~/fonts/FontTemplate/font-awesome.css",
                      "~/Content/Template/fontello.css",
                      "~/Content/Template/settings.css",
                      "~/Content/Template/style.css",
                      "~/Content/Template/dark_cyan.css",
                      "~/Content/Template/style-switcher.css",
                      "~/Content/Template/custom.css"
                      ));
            BundleTable.EnableOptimizations = false;
        }
    }
}
