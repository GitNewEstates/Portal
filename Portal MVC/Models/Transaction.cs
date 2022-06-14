using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Portal_MVC.Models
{
    public class Transaction : BaseClass
    {

        public DateTime Date { get; set; }
        public int BudgetID { get; set; }
        public string ScheduleName { get; set; }
        public int HeadingID { get; set; }
        public string Description { get; set; }
        public int TransactionTypeID { get; set; }

        public int PONumber { get; set; }
        public double Amount { get; set; }
        public bool InError { get; set; }
        public long DocID { get; set; }
        public int UnitID { get; set; }

        public bool Validated { get; set; }

       
    }

    public static class TransactionMethods 
    {
        public static string JsonSerialize(Transaction transaction)
        {

            return Newtonsoft.Json.JsonConvert.SerializeObject(transaction);
        }
        public static Transaction DeserializedJSONToTransaction(string json = "")
        {
            Transaction obj = new Transaction();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<Transaction>(json);
                }
                catch (Exception ex)
                {
                    obj.APIError = new APIError(ErrorType.APIValidationError)
                    {
                        HasError = true,
                        Message = $"Error Deserializing JSON to Transaction. Error: {ex.Message}"
                    };
                }
            }

            return obj;
        }

        public async static Task<Transaction> InsertAsync(Transaction transaction)
        {
             string json = await
                GlobalVariables.APIConnection.CallAPIPostEndPointAsync("Transactions",
                JsonSerialize(transaction));

            return DeserializedJSONToTransaction(json);
        }

    }

}