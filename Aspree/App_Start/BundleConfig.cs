using System.Web;
using System.Web.Optimization;

namespace Aspree
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

            bundles.Add(new StyleBundle("~/Content/site").Include(
                     "~/Content/site.css"));


            bundles.Add(new ScriptBundle("~/apps/common").Include(
                      "~/Content/app/common.js"));

            bundles.Add(new ScriptBundle("~/apps/roles").Include(
                     "~/Content/app/roles.js"));

            bundles.Add(new ScriptBundle("~/apps/users").Include(
                    "~/Content/app/users.js"));

            bundles.Add(new ScriptBundle("~/apps/dashboard").Include(
                    "~/Content/app/dashboard.js"));

            bundles.Add(new ScriptBundle("~/apps/emailtemplates").Include(
                    "~/Content/app/EmailTemplate.js"));

            bundles.Add(new ScriptBundle("~/apps/projects").Include(
                    "~/Content/app/project.js"));

            bundles.Add(new ScriptBundle("~/apps/entities").Include(
                   "~/Content/app/entity.js"));

            bundles.Add(new ScriptBundle("~/apps/variables").Include(
                   "~/Content/app/variable.js"));

            bundles.Add(new ScriptBundle("~/apps/forms").Include(
                   "~/Content/app/form.js"));

            bundles.Add(new ScriptBundle("~/apps/activities").Include(
                   "~/Content/app/activity.js"));

            bundles.Add(new ScriptBundle("~/apps/schedules").Include(
                   "~/Content/app/schedule.js"));

            bundles.Add(new ScriptBundle("~/apps/categories").Include(
                  "~/Content/app/category.js"));

            bundles.Add(new ScriptBundle("~/apps/deployments").Include(
                  "~/Content/app/deployment.js"));

            bundles.Add(new ScriptBundle("~/apps/searches").Include(
                  "~/Content/app/search.js"));

            bundles.Add(new ScriptBundle("~/apps/authenticationtypes").Include(
                  "~/Content/app/AuthenticationType.js"));

            bundles.Add(new ScriptBundle("~/apps/mongo/searches").Include(
                  "~/Content/app/mongo/search.js"));
            bundles.Add(new ScriptBundle("~/apps/mongo/summary").Include(
                  "~/Content/app/mongo/summary.js"));

            bundles.Add(new ScriptBundle("~/apps/actionlist").Include(
                  "~/Content/app/actionlist.js"));

#if DEBUG            
            BundleTable.EnableOptimizations = true;
#else
            BundleTable.EnableOptimizations = true;
#endif

        }
    }
}
