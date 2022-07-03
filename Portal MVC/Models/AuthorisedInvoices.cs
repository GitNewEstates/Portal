using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Portal_MVC.Models
{
    public class AuthorisedInvoices : BaseClass
    {
        public int TransactionId { get; set; }
        public DateTime AuthDate { get; set; }
        public int AuthUser { get; set; }
        public int AuthRequestUser { get; set; }
        public string InvoiceRef { get; set; }
        public DateTime ProcessedDate { get; set; }
        public int ProcessedUser { get; set; }
        public DateTime InvoiceDate { get; set; }

    }

    public static class AuthorisedInvoicesMethods
    {
        public static string JsonSerialize(AuthorisedInvoices authinvoice)
        {

            return Newtonsoft.Json.JsonConvert.SerializeObject(authinvoice);
        }
        public static AuthorisedInvoices DeserializedJSONToAuthInvoice(string json = "")
        {
            AuthorisedInvoices obj = new AuthorisedInvoices();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<AuthorisedInvoices>(json);
                }
                catch (Exception ex)
                {
                    obj.APIError = new APIError(ErrorType.APIValidationError)
                    {
                        HasError = true,
                        Message = $"Error Deserializing JSON to Authorised Invoice. Error: {ex.Message}"
                    };
                }
            }

            return obj;
        }

        public async static Task<AuthorisedInvoices> InsertAsync(AuthorisedInvoices authinvoice)
        {
            string json = JsonSerialize(authinvoice);

            return DeserializedJSONToAuthInvoice(await GlobalVariables.APIConnection.CallAPIPostEndPointAsync("AuthorisedInvoices", json));
        }
    }
}