using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Portal_MVC.Models
{
    public class APIEstates : AddressBase
    {
        public APIEstates()
        {
            OpenFundList = new List<Fund>();
        }
        public double OpenFundTotalBudget { get; set; }
        public double OpenFundTotalCost { get; set; }
        public double CurrentCashPosition { get; set; }
        public double UnauthorisedInvoiceValue { get; set; }
        public double InvoicesAwaitingPaymenteValue { get; set; }
        public double OutstandPOTotalValue { get; set; }

        public double NetCashPosition
        {
            get
            {
                return CurrentCashPosition - UnauthorisedInvoiceValue - InvoicesAwaitingPaymenteValue - OutstandPOTotalValue;
            }
        }
        public string Reference { get; set; }
        public List<Fund> OpenFundList { get; set; }

        public async Task GetInvoiceAwaitingPaymentValue()
        {
            string json = await 
                APIMethods.CallAPIGetEndPointAsync($"InvoicesAwaitingPaymentValue/{id}");
            double cash = 0;
            double.TryParse(json, out cash);
            this.InvoicesAwaitingPaymenteValue = cash;
        }
        public async Task GetOpenFundBVAList()
        {
            if (OpenFundList == null)
            {
                OpenFundList = new List<Fund>();
            }
            else
            {
                OpenFundList.Clear();
            }

            string json = await
                APIMethods.CallAPIGetEndPointAsync($"OpenFundBVAList/{id}");

            OpenFundList = DeserializedJSONToFundList(json);
        }
        public async Task GetCurrentCashPosition()
        {
            string json = await
                APIMethods.CallAPIGetEndPointAsync($"EstateCashPosition/{id}");

            double cash = 0;
            double.TryParse(json, out cash);
            this.CurrentCashPosition = cash;
        }

        public async Task GetUnauthorisedInvoiceValue()
        {
            string json = await
                APIMethods.CallAPIGetEndPointAsync($"UnauthorisedInvoiceValue/{id}");

            double cash = 0;
            double.TryParse(json, out cash);
            this.UnauthorisedInvoiceValue = cash;
        }

        public async Task GetOutstandingdPOValue()
        {
            string json = await
               APIMethods.CallAPIGetEndPointAsync($"OutstandingPOValue/{id}");

            double cash = 0;
            double.TryParse(json, out cash);
            this.OutstandPOTotalValue = cash;
        }

        public static List<Fund> DeserializedJSONToFundList(string json = "")
        {
            List<Fund> obj = new List<Fund>();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<List<Fund>>(json);
                }
                catch (Exception ex)
                {
                    Fund fund = new Fund();
                    fund.APIError.errorType = ErrorType.APIValidationError;
                    fund.APIError.HasError = true;
                    fund.APIError.Message = $"Error Deserializing JSON to Fund List. Error: {ex.Message}";
                    obj.Add(fund);
                }
            }

            return obj;
        }
    }

    public static class APIEstateMethods
    {
     
        public async static Task<List<APIEstates>> GetEstateListAsync()
        {
            string json = await APIMethods.CallAPIGetEndPointAsync($"EstateList");

            return DeserializedJSONToEstateList(json);
        }
        public async static Task<APIEstates> GetEstateAsync(int id, string uniqueid = "")
        {
            string json = "";
            if (id > 0)
            {
              
                 json =   await APIMethods.CallAPIGetEndPointAsync($"Estate/{id}");
            } else
            {
                json = await APIMethods.CallAPIGetEndPointAsync($"EstateByUniqueID/{uniqueid}");
            }

            return DeserializedJSONToEstate(json);
        }
        public async static Task<APIEstates> GetOpenFundBudgetAndSpendTotals(int EstateID)
        {
            string json =
                await APIMethods.CallAPIGetEndPointAsync($"FundTotalBudgetAndSpend/{EstateID}");

            return DeserializedJSONToEstate(json);
        }

        public static APIEstates DeserializedJSONToEstate(string json = "")
        {
            APIEstates obj = new APIEstates();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<APIEstates>(json);
                }
                catch (Exception ex)
                {
                    obj.APIError = new APIError(ErrorType.APIValidationError)
                    {
                        HasError = true,
                        Message = $"Error Deserializing JSON to Estate. Error: {ex.Message}"
                    };
                }
            }

            return obj;
        }

        public static List<APIEstates> DeserializedJSONToEstateList(string json = "")
        {
            List<APIEstates> obj = new List<APIEstates>();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<List<APIEstates>>(json);
                }
                catch (Exception ex)
                {

                    obj.Add(new APIEstates
                    {
                        APIError = new APIError(ErrorType.APIValidationError)
                        {
                            HasError = true,
                            Message = $"Error Deserializing JSON to Estate List. Error: {ex.Message}"
                        }
                    });
                }
            }

            return obj;
        }


    }

    
}