using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Portal_MVC.Models
{
    public class Units : AddressBase
    {
        public int EstateID { get; set; }

        public DateTime OwnershipStartDate { get; set; }
        public DateTime OwnershipEndDate { get; set; }

        public double Balance { get; set; }
        public string UniqueID { get; set; }
    }

    public static class UnitMethods
    {
        public async static Task<List<Units>> GetUnitsAsync()
        {
            string json = await APIMethods.CallAPIGetEndPointAsync("UnitsList");

            return JsonDeserializeToList(json);
        }
        public async static Task<List<Units>> GetCurrentOwnedUnits(int ownerid, int EstateID = 0, bool includeBalance = false)
        {
            string json = "";
            
                json =
                    await APIMethods.CallAPIGetEndPointAsync($"CurrentOwnerUnits/{ownerid}/{EstateID}/{includeBalance}");
            
           
            return JsonDeserializeToList(json);
        }

        public async static Task<List<Units>> GetUnitsAsync(int EstateID = 0)
        {
            string json =
                    await APIMethods.CallAPIGetEndPointAsync($"UnitsList/{EstateID}");
           
            return JsonDeserializeToList(json);
        }
        public async static Task<Units> GetUnitAsync(string UniqueID = "")
        {
            string json =
                    await APIMethods.CallAPIGetEndPointAsync($"Units/{UniqueID}");

            return JsonDeserializeToUnit(json);
        }


        public static List<Units> JsonDeserializeToList(string json)
        {
            List<Units> obj = new List<Units>();
            try
            {
                obj = JsonConvert.DeserializeObject<List<Units>>(json);
            }
            catch (Exception ex)
            {
                APIError error = new APIError(ErrorType.JSONDeserializationError);
                error.HasError = true;
                error.Message = $"Error Deserializing JSON returned from API. Error:{ex.Message}";
                obj.Add(new Units
                {
                    APIError = error
                });
            }

            return obj;

        }

        public static Units JsonDeserializeToUnit(string json)
        {
            Units obj = new Units();
            try
            {
                obj = JsonConvert.DeserializeObject<Units>(json);
            }
            catch (Exception ex)
            {
                APIError error = new APIError(ErrorType.JSONDeserializationError);
                error.HasError = true;
                error.Message = $"Error Deserializing JSON returned from API. Error:{ex.Message}";
                obj.APIError = error;
            }

            return obj;

        }
    }

    public class UnitStatmentTransaction : IDBase
    {
        public DateTime date { get; set; }

        public string DateStr 
        { 
            get 
            {
                DateTime _date = new DateTime();
                if (date == _date)
                {
                    return "";
                }
                else
                {
                    return ControlsDLL.ControlActions.DateFormatLong(date);
                }
            } 
        }
        public string TransType { get; set; }
        public string Description { get; set; }

        public string ChargeAmount { get; set; }
        public string PaymentAmount { get; set; }
        public string Balance { get; set; }
        

    }

    public static class UnitStatementMethods
    {
        public async static Task<List<UnitStatmentTransaction>> GetunitStatementAsync(string unitid)
        {
            string json = await APIMethods.CallAPIGetEndPointAsync($"UnitStatement/{unitid}");
            return DeserializedJSONToStatementList(json);

        }

        public static List<UnitStatmentTransaction> DeserializedJSONToStatementList(string json = "")
        {
            List<UnitStatmentTransaction> obj = new List<UnitStatmentTransaction>();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<List<UnitStatmentTransaction>>(json);
                }
                catch (Exception ex)
                {

                    APIError error = new APIError(ErrorType.JSONDeserializationError)
                    {
                        HasError = true,
                        Message = $"Error Deserializing JSON to Unit Statement List. Error: {ex.Message}"
                    };

                    obj.Add(new UnitStatmentTransaction
                    {
                        APIError = error
                    });
                }
            }

            return obj;
        }
    }
}