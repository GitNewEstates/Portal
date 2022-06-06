using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Identity.Client;

using Microsoft.Extensions.Configuration;
using System.Data;

namespace Portal_MVC.Models
{
    public class APIHelper
    {
    }
    public class APIConnectionObject
    {
        public void ResetAPIError()
        {
            APIError.errorType = ErrorType.None;
            APIError.HasError = false;
            APIError.Message = String.Empty;
        }
        public APIError APIError { get; set; }
        public APIConnectionObject()
        {
            AuthConfigObject = new APIAuthConfig();
            APIError = new APIError(ErrorType.None);
        }
        public string JWTBearerToken { get; set; }
        public APIAuthConfig AuthConfigObject { get; set; }

        private void DoAuth(HttpClient HttpClient)
        {
            var defaultrequestheaders = HttpClient.DefaultRequestHeaders;

            if (defaultrequestheaders.Accept == null ||
                !defaultrequestheaders.Accept.Any(m => m.MediaType == "application/json"))
            {
                HttpClient.DefaultRequestHeaders.Accept.Add(new
                    System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }

            defaultrequestheaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", JWTBearerToken);
        }
        public async Task<string> CallAPIGetEndPointAsync(string EndPoint)
        {
            try
            {
                if (GlobalVariables.APIConnection == null)
                {
                    await APIAuthExtensions.SetAPIConfigAsync();
                
                }

                //return json string
                var HttpClient = new HttpClient();

                DoAuth(HttpClient);

                HttpResponseMessage response =
                    await HttpClient.GetAsync(AuthConfigObject.BaseAddress + EndPoint);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    SetAPIErrorFromFailure(response);
                    return "";



                }
            }
            catch (Exception ex)
            {
                SetAPIErrorFromException(ex);
                return "";
            }
        }

        public async Task<string> CallAPIPostEndPointAsync(string EndPoint, string json)
        {

            try
            {
                if (GlobalVariables.APIConnection == null)
                {
                    await APIAuthExtensions.SetAPIConfigAsync();

                }
                HttpClient client = new HttpClient();

                DoAuth(client);

                StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                using (HttpResponseMessage response =
                    await client.PostAsync(AuthConfigObject.BaseAddress + EndPoint,
                    content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        SetAPIErrorFromFailure(response);
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                SetAPIErrorFromException(ex);
                return "";
            }



        }

        public async Task<string> CallAPIPutEndPointAsync(string EndPoint, string json)
        {

            try
            {
              
                HttpClient client = new HttpClient();

                DoAuth(client);

                StringContent content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                using (HttpResponseMessage response =
                    await client.PutAsync(AuthConfigObject.BaseAddress + EndPoint,
                    content))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        SetAPIErrorFromFailure(response);
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                SetAPIErrorFromException(ex);
                return "";
            }
        }



        private void SetAPIErrorFromFailure(HttpResponseMessage responseMessage)
        {
            this.APIError.errorType = ErrorType.APIConnectionError;
            this.APIError.HasError = true;
            this.APIError.Message = responseMessage.ReasonPhrase;
        }

        private void SetAPIErrorFromException(Exception ex)
        {
            this.APIError.errorType = ErrorType.APIConnectionError;
            this.APIError.HasError = true;
            this.APIError.Message = ex.Message;
        }


    }

    public class APIAuthConfig
    {
        public string Instance { get { return "https://login.microsoftonline.com/3579906a-9c37-4856-8cfa-ac07feeb3867"; } }
        public string TenantId { get { return "3579906a-9c37-4856-8cfa-ac07feeb3867"; } }

        public string Authority
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, Instance, TenantId);
            }
        }

        public string ClientId { get { return "34b94f15-90d1-4eba-96ac-eccd394e8938"; } }
        public string ClientSecret { get { return "ig18Q~Eb6UCCOgTEgJYE54_6T1tRuKXCqRMI5cbl"; } }
        public string BaseAddress { get { return "https://localhost:7104/api/"; } }
        public string ResourceId { get { return "api://3cf69ecd-aa7f-4048-ad19-26dc8823255c/.default"; } }




    }

    public static class APIAuthExtensions
    {
        //public static APIAuthConfig ReadFromFile(string path)
        //{
        //    IConfiguration configuration;

        //    var builder = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile(path);

        //    configuration = builder.Build();



        //    return configuration.Get<APIAuthConfig>();
        //}


        public async static Task SetAPIConfigAsync()
        {
            Models.APIAuthConfig authconfig = new APIAuthConfig();//Models.APIAuthExtensions.ReadFromFile("appsettings.json");
            GlobalVariables.APIConnection = new APIConnectionObject();
            GlobalVariables.APIConnection.AuthConfigObject = authconfig; 
            //IConfidentialClientApplication app;

            //app = (IConfidentialClientApplication)ConfidentialClientApplicationBuilder.Create(authconfig.ClientId)
            //    .WithClientSecret(authconfig.ClientSecret)
            //    .WithAuthority(new Uri(authconfig.Authority))
            //    .Build();

            //string[] resouceIds = new string[] { authconfig.ResourceId };

            //AuthenticationResult result = null;

            //try
            //{
                
            //    //result = await app.AcquireTokenForClient(resouceIds).ExecuteAsync();

            //    GlobalVariables.APIConnection.JWTBearerToken = result.AccessToken;

            //    //set the auth properties in the static apiauth object
            //    //****REmove Secret***** not needed once have jwt token
            //    //authconfig.ClientSecret = "";
            //    GlobalVariables.APIConnection.AuthConfigObject = authconfig;

            //}
            //catch (Exception ex)
            //{
            //    //ErrorMessages.ShowErrorMessage(ex, "Cannot acquire authenticated Token for API");

            //}
        }
    }
    public class APIError
    {
        public APIError(ErrorType _errorType)
        {
            errorType = _errorType;
        }
        public string Message { get; set; }
        public bool HasError { get; set; }

        public ErrorType errorType { get; set; }
    }

    public enum ErrorType
    {
        None,
        APIValidationError,
        APIDBInsertError,
        JSONDeserializationError,
        JSONSerializationError,
        APIConnectionError,
        ProcessingError

    }
    public static class DBInsertErrorMethods
    {
        public static APIError DBErrorCheck(DataTable dt)
        {
            if (dt.Rows.Count == 0 || dt.Rows[0][0].ToString() != "Error")
            {
                return new APIError(ErrorType.APIDBInsertError)
                {
                    HasError = true,
                    Message = "Error Inserting into DB"

                };
            }
            else
            {
                return new APIError(ErrorType.None);
            }


        }
    }
}