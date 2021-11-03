using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Portal_MVC.Models
{
    public static class GlobalVariables
    {
        public static int DbConfig { get; set; } = 2; //defaults to 2 being the live database
        public static string CS { get; set; }
        //public static int CustomerID { get; set; }
        //public static string CustomerName { get; set; }
        //public static string SelectedProperty { get; set; }
        //public static int SelectedPropertyID { get; set; }
        //public static List<Models.Properties> PropList { get; set; }
        public static DataTable PieChartDataTable { get; set; }
      //  public static string EstateName { get; set; }

        public static int LogUsage { get; set; } //0 = true 1 = false

      

        public static dbConn.DBConnectionObject GetConnection()
        {
            return new dbConn.DBConnectionObject(dbConn.ConnectionType.SQLDB, GlobalVariables.CS, dbConn.DBType.SQL);
           
        }

    }

    
}