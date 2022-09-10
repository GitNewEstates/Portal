using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Portal_MVC.Models
{
    public class APINotes : IDBase
    {
        public APINotes()
        {
            Date = new DateTime();
        }
        public DateTime Date { get; set; }
        public string Note { get; set; }

        public int ObjectID { get; set; }
    }

    public static class APINotesMethods
    {
        public async static Task<List<APINotes>> GetNotesAsync(string tablename, int objectid)
        {
            string json = await APIMethods.CallAPIGetEndPointAsync($"Notes/{tablename}/{objectid}");

            return DeserializedJSONNotesList(json);
        }

        public static List<APINotes> DeserializedJSONNotesList(string json = "")
        {
            List<APINotes> obj = new List<APINotes>();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<List<APINotes>>(json);
                }
                catch (Exception ex)
                {

                    APIError error = new APIError(ErrorType.JSONDeserializationError)
                    {
                        HasError = true,
                        Message = $"Error Deserializing JSON to Notes List. Error: {ex.Message}"
                    };

                    obj.Add(new APINotes
                    {
                        APIError = error
                    });
                }
            }

            return obj;
        }
    }
}