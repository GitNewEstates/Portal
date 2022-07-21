using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Portal_MVC.Models
{
    public class Owner : PersonBase
    {
        public string Reference { get; set; }
    }

    public class OwnerMethods
    {
        public async static Task<Owner> GetOwnerByEmail(string email)
        {
            string json = await 
                APIMethods.CallAPIGetEndPointAsync($"GetOwnerByEmail/{email}");

            return JsonDeserialize(json); 
        }

        public static Owner JsonDeserialize(string json)
        {
            Owner repair = new Owner();
            try
            {
                repair = JsonConvert.DeserializeObject<Owner>(json);
            }
            catch (Exception ex)
            {
                repair.APIError.errorType = ErrorType.JSONDeserializationError;
                repair.APIError.HasError = true;
                repair.APIError.Message = $"Error Deserializing JSON returned from API. Error:{ex.Message}";
            }

            return repair;

        }
    }
}