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
        public static string SerializeOwnerToJson(Owner owner)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(owner);
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
        public static List<Owner> JsonDeserializeToList(string json)
        {
            List<Owner> list = new List<Owner>();
            try
            {
                list = JsonConvert.DeserializeObject<List<Owner>>(json);
            }
            catch (Exception ex)
            {
                Owner owner = new Owner();

                owner.APIError.errorType = ErrorType.JSONDeserializationError;
                owner.APIError.HasError = true;

                owner.APIError.Message = $"Error Deserializing JSON returned from API. Error:{ex.Message}";
                list.Add(owner);
            }

            return list;

        }

        public async static Task<Owner> InsertAsync(Owner owner)
        {
            string json = OwnerMethods.SerializeOwnerToJson(owner);

            string returnjson = await APIMethods.CallAPIPostEndPointAsync("Owner", json);

            return JsonDeserialize(returnjson);
        }

        public async static Task<List<Owner>> GetAllAsync()
        {
            string json = await
                APIMethods.CallAPIGetEndPointAsync("OwnerList");

            return JsonDeserializeToList(json);

        }

        public async static Task<Owner> GetOwnerByID(int id)
        {
            string json = await
                APIMethods.CallAPIGetEndPointAsync($"Owner/{id}");

            return JsonDeserialize(json);
        }
    }
}