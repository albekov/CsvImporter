using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CsvImport.Database;

namespace CsvImport.Web
{
    public class MvcApplication : HttpApplication
    {
        public static readonly IDbManager DbManager = new DbManager();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DbManager.Init().Wait();
        }
    }
}