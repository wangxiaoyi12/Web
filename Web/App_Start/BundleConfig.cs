using System.Web.Optimization;

namespace Web.App_Start
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/js1").Include(
                     "~/assets/global/plugins/bootstrap/js/bootstrap.min.js",
                     "~/assets/global/plugins/js.cookie.min.js",
                     "~/assets/global/plugins/bootstrap-hover-dropdown/bootstrap-hover-dropdown.min.js",
                     "~/assets/global/plugins/jquery-slimscroll/jquery.slimscroll.min.js",
                     "~/assets/global/plugins/jquery.blockui.min.js",
                     "~/assets/global/plugins/bootstrap-switch/js/bootstrap-switch.min.js"
                    //"~/assets/global/plugins/morris/morris.js",
                    //"~/assets/global/plugins/morris/raphael-min.js",
                    //"~/assets/global/scripts/app.min.js",
                    //"~/assets/pages/scripts/dashboard.min.js",
                    //"~/assets/layouts/layout/scripts/layout.min.js" 
                    ));
            bundles.Add(new ScriptBundle("~/bundles/js10").Include(
                    //"~/assets/global/plugins/bootstrap/js/bootstrap.min.js",
                    //"~/assets/global/plugins/js.cookie.min.js",
                    //"~/assets/global/plugins/bootstrap-hover-dropdown/bootstrap-hover-dropdown.min.js",
                    //"~/assets/global/plugins/jquery-slimscroll/jquery.slimscroll.min.js",
                    //"~/assets/global/plugins/jquery.blockui.min.js",
                    //"~/assets/global/plugins/bootstrap-switch/js/bootstrap-switch.min.js",
                    "~/assets/global/plugins/morris/morris.js",
                    "~/assets/global/plugins/morris/raphael-min.js",
                    "~/assets/global/scripts/app.min.js",
                    "~/assets/pages/scripts/dashboard.min.js",
                    "~/assets/layouts/layout/scripts/layout.min.js"
                   ));
            bundles.Add(new ScriptBundle("~/bundles/js11").Include(
                     //"~/assets/layouts/layout/scripts/demo.min.js",
                     "~/assets/layouts/global/scripts/quick-sidebar.min.js",
                     "~/assets/layouts/global/scripts/quick-nav.min.js",
                     "~/assets/scripts/jquery.unobtrusive-ajax.min.js",
                     "~/assets/global/plugins/bootbox/bootbox.min.js",
                     "~/assets/global/plugins/DataTables-1.10.12/media/js/jquery.dataTables.min.js",
                     "~/assets/global/plugins/datatables/plugins/bootstrap/datatables.bootstrap.js",
                     "~/assets/global/plugins/select2/js/select2.full.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/js2").Include(
                    "~/assets/scripts/clicksave.js",
                    "~/assets/scripts/Message.js",
                    "~/assets/global/plugins/Validform/Validform_v5.3.2_min.js",
                    "~/assets/global/plugins/counterup/jquery.waypoints.min.js",
                    "~/assets/global/plugins/counterup/jquery.counterup.min.js",
                    "~/assets/global/plugins/bootstrap-contextmenu/bootstrap-contextmenu.js",
                    "~/assets/pages/scripts/components-context-menu.js",
                    "~/assets/global/plugins/bootstrap-datetimepicker/js/bootstrap-datetimepicker.min.js",
                    "~/assets/global/plugins/bootstrap-datetimepicker/js/locales/bootstrap-datetimepicker.zh-CN.js"));
            //bundles.Add(new StyleBundle("~/bundles/components").Include(
            //              "~/assets/global/css/components.min.css", new CssRewriteUrlTransform()));
            bundles.Add(new StyleBundle("~/bundles/css").Include(
                          "~/assets/global/css/font-css.css", new CssRewriteUrlTransform()).Include(
                          "~/assets/global/plugins/font-awesome/css/font-awesome.min.css", new CssRewriteUrlTransform()).Include(
                          "~/assets/global/plugins/simple-line-icons/simple-line-icons.min.css", new CssRewriteUrlTransform()).Include(
                          "~/assets/global/plugins/bootstrap/css/bootstrap.min.css", new CssRewriteUrlTransform()).Include(
                          "~/assets/global/plugins/bootstrap-switch/css/bootstrap-switch.min.css", new CssRewriteUrlTransform()).Include(
                          "~/assets/global/plugins/bootstrap-datetimepicker/css/bootstrap-datetimepicker.min.css", new CssRewriteUrlTransform()).Include(
                          "~/assets/global/plugins/morris/morris.css", new CssRewriteUrlTransform()).Include(
                          //"~/assets/global/plugins/fullcalendar/fullcalendar.min.css", new CssRewriteUrlTransform()).Include(  //日历,暂时用不到
                          "~/assets/global/css/plugins.min.css", new CssRewriteUrlTransform()).Include(
                          "~/assets/layouts/layout/css/layout.min.css", new CssRewriteUrlTransform()).Include(
                          "~/assets/layouts/layout/css/themes/darkblue.min.css", new CssRewriteUrlTransform()).Include(
                          "~/assets/global/plugins/Validform/css/style.css", new CssRewriteUrlTransform())
                        );
            bundles.Add(new StyleBundle("~/bundles/css2").Include(
                         //"~/assets/layouts/layout/css/custom.min.css", new CssRewriteUrlTransform()).Include(  //自己的代码,不要和这些固定的放一起
                         "~/assets/global/plugins/datatables/datatables.min.css", new CssRewriteUrlTransform()).Include(
                         "~/assets/global/plugins/datatables/plugins/bootstrap/datatables.bootstrap.css", new CssRewriteUrlTransform()).Include(
                         "~/assets/global/plugins/sweetalert/jquery-confirm.min.css", new CssRewriteUrlTransform()).Include(
                         "~/assets/global/plugins/select2/css/select2.min.css", new CssRewriteUrlTransform()).Include(
                         "~/assets/global/plugins/select2/css/select2-bootstrap.min.css", new CssRewriteUrlTransform()));
        }
    }
}