using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dbConn;
using System.Data;
using Portal_MVC;

namespace Portal_MVC.Models
{
    public class PurchaseOrders
    {
        public int ID { get; set; }
        public int BudgetID { get; set; }
        public DateTime RaisedDate { get; set; }
        public string RaisedDateStr { get; set; }
        public string ScheduleName { get; set; }
        public string heading { get; set; }
        public int StatusID { get; set; }
        public int RepairID { get; set; }
        public int SupplierID { get; set; }
        public int DocID { get; set; }
        public double EstCost { get; set; }
        public string EstCoststr { get; set; }


    }

    public static class PurchaseOrderMethods
    {
        public static PurchaseOrders GetPurchaseOrderByID(int ID)
        {
            string q = "Select * from core.PurchaseOrders where ID = " + ID.ToString();

            dbConnection db = new dbConnection();
            DataTable dt = db.GetDataTable(GlobalVariables.CS, q);

            PurchaseOrders po = new PurchaseOrders();
            if (dt.Rows.Count > 0)
            {
                DataRow d = dt.Rows[0];
                if (d[0].ToString() != "Error")
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        PurchaseOrders p = new PurchaseOrders { ID = Convert.ToInt32(dr[0]), };
                    }
                }
            }

            return po;
        }
    }
}