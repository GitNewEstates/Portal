using MimeKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Portal_MVC.Models
{
    public class MailService : BaseClass
    {
        public MailService()
        {

        }
        public MailService(string _subject, string _html,
            string _plainTextContent, dbConn.DBConnectionObject _conObj,
            int _userid, List<string> _to)
        {
            Subject = _subject;
            HTMLContent = _html;
            plainTextBody = _plainTextContent;
            conObj = _conObj;
            userid = _userid;
            ToList = _to;

        }

        int userid { get; set; }
        dbConn.DBConnectionObject conObj { get; set; }


        public string plainTextBody { get; set; }

        public string HTMLContent { get; set; } = "";

        public List<Attachment> Attachements { get; private set; }

        public enum SentStatus
        {
            Sent,
            Unsent
        }


        public SentStatus sentStatus { get; private set; }


        public List<string> ToList { get; set; }

        public string Subject { get; set; }
      

    }

    public static class MailServiceMethods
    {
        public static MailService DeserialiseJSONtoMailService(string json)
        {

            MailService obj = new MailService();
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    obj = JsonConvert.DeserializeObject<MailService>(json);
                }
                catch (Exception ex)
                {
                    obj.APIError = new APIError(ErrorType.JSONDeserializationError);

                    obj.APIError.Message = ex.Message;
                    obj.APIError.HasError = true;

                }
            }

            return obj;
        }

        public static string JsonSerialize(MailService mail)
        {

            return Newtonsoft.Json.JsonConvert.SerializeObject(mail);
        }

        public async static Task<MailService> SendMailAPI(MailService mail)
        {
            //serialize object
            string JSON = JsonSerialize(mail);
            string ReturnJson = await APIMethods.CallAPIPostEndPointAsync("SendEmail", JSON);



            //deserialise the returned json
            return DeserialiseJSONtoMailService(ReturnJson);


        }
    }

    public class Attachment
    {
        public Attachment(string filePath,
            DocType docType,
            string fileExtension)
        {
            FilePath = filePath;
            AttachmentType = docType;


            AttachmentObj = new MimePart(AttachmentType.ToString(), fileExtension)
            {
                Content = new MimeContent(File.OpenRead(FilePath), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = System.IO.Path.GetFileName(FilePath)
            };

        }

        public DocType AttachmentType { get; private set; }
        public string FilePath { get; private set; }
        public MimePart AttachmentObj { get; private set; }

        public enum DocType
        {
            Json,
            Text,
            Image,
            PDF,
            Doc
        }
    }
}