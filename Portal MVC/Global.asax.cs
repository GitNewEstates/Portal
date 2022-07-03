using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Configuration;
using Portal_MVC.Models;

namespace Portal_MVC
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBPh8sVXJ0S0d+XE9Cd1RDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS3xTcEdlWH1edXZSRmhaUQ==");
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


                dbConn.DBConnectionObject db = GlobalVariables.GetConnection();
                System.Data.DataTable dt = db.Connection.InsertCommand(tbl, col, pa, va);
            } catch { }
        }
    }
}
