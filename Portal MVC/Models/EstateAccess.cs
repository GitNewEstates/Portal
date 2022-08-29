using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Portal_MVC.Models
{
    public class EstateAccessInstruction : BaseClass
    {
        public EstateAccessInstruction()
        {
            AccessType = new AccessTypes();
        }
        public int EstateID { get; set; }

        public AccessTypes AccessType { get; set; }

        public string KeyCode { get; set; }
        public string AccessInstructions { get; set; }
        public bool PortalViewable { get; set; }
        public string AccessPoint { get; set; }
        public string AccessCode { get; set; }
    }

    public static class EstateAccessInstructionMethods
    {
        public async static Task<List<EstateAccessInstruction>> GetEstateAccessInstructions(int Estateid)
        {
            string json = await APIMethods.CallAPIGetEndPointAsync($"EstateAccess/{Estateid}");

            return DeserializedJSONToEstateList(json);

        }

        public static List<EstateAccessInstruction> DeserializedJSONToEstateList(string json = "")
        {
            List<EstateAccessInstruction> obj = new List<EstateAccessInstruction>();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<List<EstateAccessInstruction>>(json);
                }
                catch (Exception ex)
                {
                    EstateAccessInstruction estateAccessInstruction = new EstateAccessInstruction();
                    estateAccessInstruction.APIError = new APIError(ErrorType.JSONDeserializationError);
                    estateAccessInstruction.APIError.HasError = true;
                    estateAccessInstruction.APIError.Message = $"Error Deserializing JSON to Estate List. Error: {ex.Message} ";


                    obj.Add(estateAccessInstruction);
                }
            }

            return obj;
        }
    }

    public class AccessTypes : BaseClass
    {
        public string AccessType { get; set; }
        public bool CodeRequired { get; set; }

    }
}