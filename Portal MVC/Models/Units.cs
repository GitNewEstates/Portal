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
    }

    public static class UnitMethods
    {
        public async static Task<List<Units>> GetCurrentOwnedUnits(int ownerid, int EstateID = 0, bool includeBalance = false)
        {
            string json = "";
            if (EstateID > 0)
            {
                json =
                    await APIMethods.CallAPIGetEndPointAsync($"CurrentOwnerUnits/{ownerid}/{EstateID}/{includeBalance}");
            }
            else
            {
                json =
                     await APIMethods.CallAPIGetEndPointAsync($"CurrentOwnerUnits/{ownerid}");
            }
            return JsonDeserializeToList(json);
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
    }
}