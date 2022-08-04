using Newtonsoft.Json;
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

    public class PortalTransaction : Transaction
    {
        //Portal Properties Only
        public int SupplierID { get; set; }
        public string TransactionDate { get; set; }
        public string PayMethod { get; set; }

        public string[] ImageUrls { get; set; }
    }

    public static class PortalTransactionMethods
    {
        public static string JsonSerialize(PortalTransaction transaction)
        {

            return Newtonsoft.Json.JsonConvert.SerializeObject(transaction);
        }
        public static PortalTransaction DeserializedJSONToPortalTransaction(string json = "")
        {
            PortalTransaction obj = new PortalTransaction();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<PortalTransaction>(json);
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
            } else
            {
                obj.APIError = new APIError(ErrorType.APIValidationError)
                {
                    HasError = true,
                    Message = $"Error Deserializing JSON to Transaction."
                };
            }

            return obj;
        }

        public async static Task<Transaction> InsertAsync(Transaction transaction)
        {
            string TransJson = JsonSerialize(transaction);


             string json = await
                GlobalVariables.APIConnection.CallAPIPostEndPointAsync($"Transactions", TransJson);


            return DeserializedJSONToTransaction(json);
        }

    }

    //Api Transactions
    public class APITransactionBase :BaseClass
    {
        public DateTime Date { get; set; }
        public int BudgetID { get; set; }
        public string ScheduleName { get; set; }
        public int HeadingID { get; set; }
        public string Description { get; set; }
        public int TransactionTypeID { get; set; }

        public int PONumber { get; set; }
        public double Amount { get; set; }
        public string AmountStr
        {
            get
            {
                return ControlsDLL.ControlActions.DoubelToCurrencyString2DP(Amount);
            }
        }
        public bool InError { get; set; }
        public int UnitID { get; set; }
    }

    public class Expenditure : APITransactionBase
    {
        public Expenditure()
        {
            supplier = new Supplier();
        }
        public DateTime InvoiceDate { get; set; }
        public DateTime ProcessedDate { get; set; }
        public DateTime AuthorisedDate { get; set; }

        public string InvoiceRef { get; set; }

        public Supplier supplier { get; set; }



    }

    public static class ExpenditureMethods
    {
        public static List<Expenditure> DeserializedJSONToExpenditureList(string json = "")
        {
            List<Expenditure> obj = new List<Expenditure>();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<List<Expenditure>>(json);
                }
                catch (Exception ex)
                {
                    APIError er = new APIError(ErrorType.APIValidationError)
                    {
                        HasError = true,
                        Message = $"Error Deserializing JSON to Expenditure List. Error: {ex.Message}"
                    };

                    obj.Add(new Expenditure
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

                obj.Add(new Expenditure
                {
                    APIError = er
                });
            }

            return obj;
        }


        public async static Task<List<Expenditure>> GetUnauthorisedExpenditureAsync(int EstateID)
        {
            string json = await APIMethods.CallAPIGetEndPointAsync($"UnauthorisedInvoiceList/{EstateID}");

            return DeserializedJSONToExpenditureList(json);
        }

        public async static Task<List<Expenditure>> GetUnpaidExpenditureAsync(int EstateID)
        {
            string json = await APIMethods.CallAPIGetEndPointAsync($"UnpaidInvoicesList/{EstateID}");

            return DeserializedJSONToExpenditureList(json);
        }
    }

}