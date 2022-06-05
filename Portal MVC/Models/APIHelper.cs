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
        public string Instance { get; set; }
        public string TenantId { get; set; }

        public string Authority
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, Instance, TenantId);
            }
        }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string BaseAddress { get; set; }
        public string ResourceId { get; set; }




    }

    public static class APIAuthExtensions
    {
        //public static APIAuthConfig ReadFromFile(string path)
        //{
        //    //IConfiguration configuration;

        //    //var builder = new ConfigurationBuilder()
        //    //    .SetBasePath(Directory.GetCurrentDirectory())
        //    //    .AddJsonFile(path);

        //    //configuration = builder.Build();



        //    //return configuration.Get<APIAuthConfig>();
        //}


        public async static Task SetAPIConfigAsync()
        {
            //Models.APIAuthConfig authconfig = new APIAuthConfig();//Models.APIAuthExtensions.ReadFromFile("appsettings.json");

            //IConfidentialClientApplication app;

            //app = (IConfidentialClientApplication)ConfidentialClientApplicationBuilder.Create(authconfig.ClientId)
            //    .WithClientSecret(authconfig.ClientSecret)
            //    .WithAuthority(new Uri(authconfig.Authority))
            //    .Build();

            //string[] resouceIds = new string[] { authconfig.ResourceId };

            //AuthenticationResult result = null;

            //try
            //{
            //    result =  AuthenticationResult("", true, "", new DateTimeOffset()); // await app.AcquireTokenForClient(resouceIds).ExecuteAsync();

            //    GlobalVariables.APIConnection.JWTBearerToken = result.AccessToken;

            //    //set the auth properties in the static apiauth object
            //    //****REmove Secret***** not needed once have jwt token
            //    authconfig.ClientSecret = "";
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