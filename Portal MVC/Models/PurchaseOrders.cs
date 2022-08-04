using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using dbConn;
using System.Data;
using Portal_MVC;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

           DBConnectionObject db = GlobalVariables.GetConnection();
            DataTable dt = db.Connection.GetDataTable( q);

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

    public class APIPurchaseOrders : BaseClass
    {
        public APIPurchaseOrders()
        {
            LineItems = new List<PurchaseOrderLineItem>();
        }
        public DateTime RaiseDate { get; set; }
        public List<PurchaseOrderLineItem> LineItems { get; set; }
        public double EstCost { get; set; }

        public string POHeader { get; set; }
        public double TotalCost { get; set; }

        public string TotalCostStr
        {
            get { return ControlsDLL.ControlActions.DoubelToCurrencyString2DP(TotalCost); }
        }

    }

    public static class APIPurchaseOrderMethods
    {
        public async static Task<List<APIPurchaseOrders>> GetOpenPurchaseOrderList(int estateid)
        {
            string json =await APIMethods.CallAPIGetEndPointAsync($"OutstandingPOList/{estateid}");

            return DeserializedJSONToPurchaseOrderList(json);
        }

        public static List<APIPurchaseOrders> DeserializedJSONToPurchaseOrderList(string json = "")
        {
            List<APIPurchaseOrders> obj = new List<APIPurchaseOrders>();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<List<APIPurchaseOrders>>(json);
                }
                catch (Exception ex)
                {
                    APIError er = new APIError(ErrorType.APIValidationError)
                    {
                        HasError = true,
                        Message = $"Error Deserializing JSON to Expenditure List. Error: {ex.Message}"
                    };

                    obj.Add(new APIPurchaseOrders
                    {
                        APIError = er
                    });
                }
            }
            else
            {
                APIError er = new APIError(ErrorType.APIValidationError)
                {
                    HasError = true,
                    Message = $"Error Deserializing JSON to Expenditure List."
                };

                obj.Add(new APIPurchaseOrders
                {
                    APIError = er
                });
            }

            return obj;
        }
    }

    public class PurchaseOrderLineItem : BaseClass
    {
        public int PurchaseOrderID { get; set; }
        public string ItemDescription { get; set; }
        public double ItemCost { get; set; }
    }
}