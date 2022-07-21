using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Portal_MVC.Models
{
    public class APIUser : PersonBase
    {
        public APIUser()
        {
            Role = new UserRole();
        }
        public UserRole Role { get; set; }
        public string UserName { get; set; }
        public string JobTitle { get; set; }
    }

    public class UserRole
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public static class UserMethods
    {
        public async static Task<APIUser> GetUserByEmail(string email)
        {
            string json = await
                APIMethods.CallAPIGetEndPointAsync($"GetUserByEmail/{email}");

            return JsonDeserialize(json);
        }

        public static APIUser JsonDeserialize(string json)
        {
            APIUser user = new APIUser();
            try
            {
                user = JsonConvert.DeserializeObject<APIUser>(json);
            }
            catch (Exception ex)
            {
                user.APIError.errorType = ErrorType.JSONDeserializationError;
                user.APIError.HasError = true;
                user.APIError.Message = $"Error Deserializing JSON returned from API. Error:{ex.Message}";
            }

            return user;

        }
    }
}