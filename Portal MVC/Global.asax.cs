using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Configuration;

namespace Portal_MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //Models.GlobalVariables.CS = ConfigurationManager.ConnectionStrings["LocalConnection"].ConnectionString;

           Models.GlobalVariables.CS = ConfigurationManager.ConnectionStrings["DeployConnection"].ConnectionString;

            try
            {
                string tbl = "core.VisitLog";
                List<string> col = new List<string>();
                col.Add("_Date");

                List<string> pa = new List<string>();
                pa.Add("@_Date");

                List<object> va = new List<object>();
                va.Add(DateTime.Now);


                dbConn.dbConnection db = new dbConn.dbConnection();
                System.Data.DataTable dt = db.InsertCommand(Models.GlobalVariables.CS, tbl, col, pa, va);
            } catch { }
        }
    }
}
