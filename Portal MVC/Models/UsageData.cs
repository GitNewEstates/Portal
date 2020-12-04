using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Portal_MVC.Models
{
    public static class UsageData
    {
        public static void InsertNewUsage(int ActivityID, int CustomerID, string MiscData = null)
        {
            try
            {
                //0 =log data
                
                if (GlobalVariables.LogUsage == 0)
                {
                    //int customerID = 0;
                    //if (ActivityID != 3)
                    //{
                    //    customerID = CustomerID;
                    //}



                    List<string> c = new List<string>()
                {
                    "CustomerID",
                    "LogDate",
                    "ActivityID"
                };

                    List<string> p = new List<string>()
                {
                    "@CustomerID",
                    "@LogDate",
                    "@ActivityID"
                };

                    List<object> o = new List<object>()
                {
                   CustomerID,
                    DateTime.Now,
                    ActivityID
                };

                    if (!string.IsNullOrWhiteSpace(MiscData))
                    {
                        c.Add("MiscData");
                        p.Add("@MiscData");
                        o.Add(MiscData);
                    }

                    dbConn.dbConnection db = new dbConn.dbConnection();
                    db.InsertCommandCurrent(GlobalVariables.CS, "Core.PortalUsageData", c, p, o);
                }
            } catch { }
        }

    }
}