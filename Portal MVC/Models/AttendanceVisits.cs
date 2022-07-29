using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Portal_MVC.Models
{
    public class AttendanceVisits  : BaseClass
    {

        public DateTime VisitDate { get; set; }

       
        public string VisitDescription { get; set; }
        public int AttendingUser { get; set; }
        public int AttendanceTypeID { get; set; }
        public string urlString { get; set; }
        public int EstateID { get; set; }
        public List<string> UrlList { get; set; }


        public AttendanceType AttendanceType { get; set; }



        
        public bool NotifyCustomer { get; set; }


        public string GuidID { get; set; }


      
        public bool FireAlarmTest { get; set; } 

      
        public bool PortalViewable { get; set; }
        public string AttendanceStr { get; set; }




    }

    public static class AttendanceVisitMethods
    {
        public async static Task<List<AttendanceVisits>> GetAttendanceVisitListAsync(int EstateID)
        {
            string json = await
                APIMethods.CallAPIGetEndPointAsync($"AttendanceVisitList/{EstateID}");

            return DeserializedJSONToAttendanceList(json);

        }

        public async static Task<AttendanceVisits> GetAttendanceVisitsAsync(int id)
        {
            string json = await 
                APIMethods.CallAPIGetEndPointAsync($"AttendanceVisit/{id}");

            return DeserializedJSONToAttendance(json);
        }

        public static AttendanceVisits DeserializedJSONToAttendance(string json = "")
        {
            AttendanceVisits obj = new AttendanceVisits();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<AttendanceVisits>(json);
                }
                catch (Exception ex)
                {
                    obj.APIError = new APIError(ErrorType.APIValidationError)
                    {
                        HasError = true,
                        Message = $"Error Deserializing JSON to Attendance. Error: {ex.Message}"
                    };
                }
            }

            return obj;
        }

        public static List<AttendanceVisits> DeserializedJSONToAttendanceList(string json = "")
        {
            List<AttendanceVisits> obj = new List<AttendanceVisits>();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<List<AttendanceVisits>>(json);
                }
                catch (Exception ex)
                {

                    APIError error = new APIError(ErrorType.JSONDeserializationError)
                    {
                        HasError = true,
                        Message = $"Error Deserializing JSON to Repair List. Error: {ex.Message}"
                    };

                    obj.Add(new AttendanceVisits
                    {
                        APIError = error
                    });
                }
            }

            return obj;
        }

    }
    public class AttendanceType
    {

        public int id { get; set; }

       
        public string Name { get; set; }
    }
}