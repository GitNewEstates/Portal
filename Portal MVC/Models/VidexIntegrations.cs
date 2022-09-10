using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Portal_MVC.Models
{
    public class VidexIntercom : NameBase
    {

        public string Description { get; set; }
        public string Number { get; set; }
        public int MasterCode { get; set; }
        public int AdminCode { get; set; }
        public int EstateID { get; set; }
        public string EstateName { get; set; }

        public int MaxMemoryLocations { get; set; }
        public string MaxMemoryLocationsStr { get; set; }

        public bool CreateMemoryLocations { get; set; }

    }

    public static class VidexIntercomMethods
    {
        public async static Task<VidexIntercom> InsertAsync(VidexIntercom videxIntercom)
        {
           
            //serialise
            string Intercomjson = JSONSerialize(videxIntercom);

            string json = await APIMethods.CallAPIPostEndPointAsync("VidexIntercom", Intercomjson);

            return DeserializedJSONToIntercom(json);
        }
        public static string JSONSerialize(VidexIntercom obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
        public async static Task<List<VidexIntercom>> GetIntercomListAsync(string EstateID = "")
        {
            string json = await APIMethods.CallAPIGetEndPointAsync($"VidexIntercom/{EstateID}");

            return DeserializedJSONToIntercomList(json);
        }

        public async static Task<VidexIntercom> GetIntercomAsync(int id)
        {
            string json = await APIMethods.CallAPIGetEndPointAsync($"VidexIntercom/{id}");

            return DeserializedJSONToIntercom(json);
        }
        public static VidexIntercom DeserializedJSONToIntercom(string json = "")
        {
            VidexIntercom obj = new VidexIntercom();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<VidexIntercom>(json);
                }
                catch (Exception ex)
                {

                    APIError error = new APIError(ErrorType.JSONDeserializationError)
                    {
                        HasError = true,
                        Message = $"Error Deserializing JSON to Intercom. Error: {ex.Message}"
                    };

                    obj.APIError = error;
                }
            }

            return obj;
        }

        public static List<VidexIntercom> DeserializedJSONToIntercomList(string json = "")
        {
            List<VidexIntercom> obj = new List<VidexIntercom>();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<List<VidexIntercom>>(json);
                }
                catch (Exception ex)
                {

                    APIError error = new APIError(ErrorType.JSONDeserializationError)
                    {
                        HasError = true,
                        Message = $"Error Deserializing JSON to Intercom List. Error: {ex.Message}"
                    };

                    obj.Add(new VidexIntercom
                    {
                        APIError = error
                    });
                }
            }

            return obj;

        }
    }
    

    public class VidexIntercomMemoryLocation : IDBase
    {
        public int IntercomID { get; set; }
        public int LocationNumber { get; set; }
        public int AccessCode { get; set; }

        public string LocationName { get; set; }
        public string PrimaryNumber { get; set; }
        public string Divert1 { get; set; }
        public string Divert2 { get; set; }
        public string Divert3 { get; set; }

        public bool DialToOpen { get; set; }


    }

    public static class VidexIntercomMemoryLocationMethods
    {
       
        public static string JSONSerialize(VidexIntercomMemoryLocation obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public async static Task SendQueryMessage(VidexIntercomMemoryLocation locationobj)
        {
            string json = JSONSerialize(locationobj);

            string rjson = await APIMethods.CallAPIPostEndPointAsync("SendQueryMessage", json);
        }

        public async static Task SendUpdatePrimaryNumberMessage(VidexIntercomMemoryLocation locationobj)
        {
            string json = JSONSerialize(locationobj);

            string rjson = await APIMethods.CallAPIPostEndPointAsync("SendUpdatePrimaryNumber", json);
        }
        public async static Task<List<VidexIntercomMemoryLocation>> GetVidexIntercomMemoryLocationsAsync(int id)
        {
            string json = await APIMethods.CallAPIGetEndPointAsync($"VidexIntercomMemoryLocation/{id}");
            return DeserializedJSONToMeMLocationList(json);
        }
        public async static Task<VidexIntercomMemoryLocation> GetVidexIntercomMemoryLocationAsync(int id)
        {
            string json = await APIMethods.CallAPIGetEndPointAsync($"GetMemoryLocation/{id}");
            return DeserializedJSONToMeMLocation(json);
        }
        public static List<VidexIntercomMemoryLocation> DeserializedJSONToMeMLocationList(string json = "")
        {
            List<VidexIntercomMemoryLocation> obj = new List<VidexIntercomMemoryLocation>();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<List<VidexIntercomMemoryLocation>>(json);
                }
                catch (Exception ex)
                {

                    APIError error = new APIError(ErrorType.JSONDeserializationError)
                    {
                        HasError = true,
                        Message = $"Error Deserializing JSON to Memory Location List. Error: {ex.Message}"
                    };

                    obj.Add(new VidexIntercomMemoryLocation
                    {
                        APIError = error
                    });
                }
            }

            return obj;
        }
        public static VidexIntercomMemoryLocation DeserializedJSONToMeMLocation(string json = "")
        {
           VidexIntercomMemoryLocation obj = new VidexIntercomMemoryLocation();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<VidexIntercomMemoryLocation>(json);
                }
                catch (Exception ex)
                {

                    APIError error = new APIError(ErrorType.JSONDeserializationError)
                    {
                        HasError = true,
                        Message = $"Error Deserializing JSON to Memory Location List. Error: {ex.Message}"
                    };

                    obj.APIError = error;
                    
                }
            }

            return obj;
        }

        public static string SetMeMoryLoaction(int mem)
        {
            if (mem.ToString().Length == 1)
            {
                return $"00{mem}";
            }

            if (mem.ToString().Length == 2)
            {
                return $"0{mem}";
            }

            return mem.ToString();
        }
    }

   
}