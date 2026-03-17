using System.Web;
using System.Web.Optimization;

namespace SneakRush
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Esto es un llamado a los paquetes NuGet que fui instalando
            bundles.Add(new Bundle("~/bundles/complementos").Include(
                        "~/Scripts/fontawesome/all.min.js",
                        "~/Scripts/DataTables/jquery.dataTables.js",
                        "~/Scripts/DataTables/dataTables.responsive.js", // Para que pueda ajustar a un entorno de celular
                        "~/Scripts/SuperposiciónDeCarga/loadingoverlay.min.js",
                        "~/Scripts/sweetalert.min.js",
                        "~/Scripts/jquery.validate.js",
                        "~/Scripts/jquery-ui-1-14.1.js",
                        "~/Scripts/jquery-ui.js",
                        "~/Scripts/loadingoverlay.min.js",
                        "~/Scripts/scripts.js"));


            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.bundle.js"));

            // Aca de llama a los estilos del paquetes NuGet
            bundles.Add(new StyleBundle("~/Content/css").Include
                ("~/Content/site.css",
                "~/Content/DataTables/css/jquery.dataTables.css",
                "~/Content/DataTables/css/responsive.dataTables.css",
                "~/Content/sweetalert.css",
                "~/Content/themes/base/jquery-ui.css",
                "~/Content/jquery-ui.css"
                ));

        }
    }
}
