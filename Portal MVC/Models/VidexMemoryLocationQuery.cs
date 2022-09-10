using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Portal_MVC.Models
{   
    public class VidexMemoryLocationQuery : BaseClass
    {
        public string body { get; set; }
        public string ToNumber { get; set; }
        public string MemoryLocation { get; set; }

    }

    public static class VidexMemoryLocationQueryMethods
    {
        public static string JSONSerialize(VidexMemoryLocationQuery obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
        public static VidexMemoryLocationQuery DeserializedJSONToMeMLocationQuery(string json = "")
        {
            VidexMemoryLocationQuery obj = new VidexMemoryLocationQuery();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<VidexMemoryLocationQuery>(json);
                }
                catch (Exception ex)
                {

                    APIError error = new APIError(ErrorType.JSONDeserializationError)
                    {
                        HasError = true,
                        Message = $"Error Deserializing JSON to object. Error: {ex.Message}"
                    };

                    obj.APIError = error;

                }
            }

            return obj;
        }
        public async static Task<VidexMemoryLocationQuery> InsertAsync(VidexMemoryLocationQuery obj)
        {
            string json = await APIMethods.CallAPIPostEndPointAsync("VidexMemoryLocationQuery", JSONSerialize(obj));


            return DeserializedJSONToMeMLocationQuery(json);
        }
        public async static Task<VidexMemoryLocationQuery> GetAsync(string To, string MemLocation)
        {
            string q = $"select * from core.VidexMemoryLocationQuery where Tonumber = '{To}' and MemoryLocation = '{MemLocation}'";

            DataTable dt = await GlobalVariables.GetConnection().Connection.GetDataTableAsync(q);
            VidexMemoryLocationQuery r = new VidexMemoryLocationQuery();
            if (dt.Rows.Count>0 && dt.Rows[0][0].ToString() != "Error")
            {
                int _id = 0;
                int.TryParse(dt.Rows[0][0].ToString(), out _id);
                r.id = _id;
                r.body = dt.Rows[0][1].ToString();
                r.ToNumber = To;
                r.MemoryLocation = MemLocation;

            }
            else
            {
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0][0].ToString() == "Error")
                    {
                        r.APIError = new APIError(ErrorType.APIDBInsertError);
                        r.APIError.HasError = true;
                        r.APIError.Message = "error obtaining memory location query object";
                    }
                }
            }

            return r;
        }

        public async static Task DeleteAsync(int id)
        {
            await APIMethods.CallAPIGetEndPointAsync($"DeleteMemoryQuery/{id}");
        }

    }


  
}