using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Portal_MVC.Models
{
    public class APINottificationSettings : BaseClass
    {
        public int CustomerID { get; set; }
        public int UnitID { get; set; }
        public int EstateID { get; set; }
        public bool NewRepair { get; set; }
        public bool NewAccountCharge { get; set; }
        public bool NewAccountPayment { get; set; }

        public bool NewSCBudget { get; set; }

        public bool NewInsurance { get; set; }

        public bool AttendanceLog { get; set; }
    }

    public enum SettingType
    {
        NewRepair,
        NewInsurance,
        NewAttendnace,
        NewBudget,
        NewPayment,
        NewCharge
    }

    public static class APINottificationSettingMethods
    {
        public static APINottificationSettings DeserializedJSONToNottificationSetting(string json = "")
        {
            APINottificationSettings obj = new APINottificationSettings();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<APINottificationSettings>(json);
                }
                catch (Exception ex)
                {
                    obj.APIError = new APIError(ErrorType.APIValidationError)
                    {
                        HasError = true,
                        Message = $"Error Deserializing JSON to Notification Setting. Error: {ex.Message}"
                    };
                }
            }

            return obj;
        }

        public static string SerializeRepairToJson(APINottificationSettings setting)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(setting);
        }
        public async static Task<APINottificationSettings> UpdateSetting(SettingType type, 
            APINottificationSettings settingobj)
        {
            string json = SerializeRepairToJson(settingobj);
            switch (type)
            {
                case SettingType.NewRepair:
                    string returnJson =
                        await APIMethods.CallAPIPostEndPointAsync($"NewRepair", json);

                    return DeserializedJSONToNottificationSetting(returnJson);
                    
                case SettingType.NewInsurance:

                    break;
                case SettingType.NewAttendnace:

                    break;
                case SettingType.NewBudget:

                    break;
                case SettingType.NewPayment:

                    break;
                case SettingType.NewCharge:

                    break;

            }

            return settingobj;
        }
    }


}