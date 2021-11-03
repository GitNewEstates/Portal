using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using dbConn;
using System.Globalization;

namespace Portal_MVC.Models
{
    public class Contractors
    {
        public string Name { get; set; }
    }

    public static class ContractorMethods
    {
        public static Contractors ContractorDetailsByPO(int PONumber)
        {
            string q = " select core.Contractors.Name from core.Contractors " +
                        "inner join core.PurchaseOrders on core.Contractors.id = core.PurchaseOrders.SupplierID " +
                        "where core.PurchaseOrders.ID = " + PONumber.ToString();

           DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = db.Connection.GetDataTable( q);
            Contractors returnC = new Contractors();
            returnC.Name = "";

            if(dt.Rows.Count > 0)
            {
                if(dt.Rows[0][0] != DBNull.Value || dt.Rows[0][0].ToString() != "Error")
                {
                    Contractors c = new Contractors { Name = dt.Rows[0][0].ToString() };
                    returnC = c;
                }
                

            }
         
            return returnC;
            
        }
    }
}