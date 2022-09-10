using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Portal_MVC.Models
{
    public class TextMessage : IDBase
    {
        public TextMessage(SMSSenders sender)
        {
            senderType = sender;
        }

        public string MessageBody { get; set; }
        public string ReceipientNumber { get; set; }
        public string ResponseID { get; set; }

        public SMSSenders senderType { get; set; }
        
    }

    public static class TextMessageMethods
    {
        public static string JSONSerialize(TextMessage obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public async static Task SendMessage(TextMessage message)
        {
            await APIMethods.CallAPIPostEndPointAsync("SendTextMessage", JSONSerialize(message));
        }
    }

    public enum SMSSenders
    {
        Query,
        Update,
        General
    }
}