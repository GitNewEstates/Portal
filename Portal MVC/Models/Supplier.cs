using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal_MVC.Models
{
    //Uses API
    public class Supplier
    {
        public Supplier()
        {
            APIError = new APIError(ErrorType.None);
        }
        public int id { get; set; }
        public APIError APIError { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string PostCode { get; set; }
        public string SortCode { get; set; }
        public string AccountNumber { get; set; }
    }

    public static class SupplierMethods
    {
        public async static Task<List<Supplier>> GetSupplierListAsync()
        {
             string json = 
                await GlobalVariables.APIConnection.CallAPIGetEndPointAsync("SupplierCollection");

            return DeserializedJSONToSupplierList(json);

        }

        public static List<Supplier> DeserializedJSONToSupplierList(string json = "")
        {
            List<Supplier> obj = new List<Supplier>();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<List<Supplier>>(json);
                }
                catch (Exception ex)
                {
                    Supplier supplier = new Supplier();
                    supplier.APIError.errorType = ErrorType.APIValidationError;
                    supplier.APIError.HasError = true;
                    supplier.APIError.Message = $"Error Deserializing JSON to Supplier List. Error: {ex.Message}";
                    obj.Add(supplier);
                }
            }

            return obj;
        }
    }
}