using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Aspree.Services
{
    /// <summary>
    /// WebApiHandler is common class containing methods to call the APIs 
    /// </summary>
    /// <param name="message">Sucess Message</param>
    public class WebApiHandler
    {
        private readonly string _baseUrl;
        private readonly string _testBaseUrl;
        private readonly string _baseOktaUrl;
        private Utility.ResponseMessage _responseMessage;
        private readonly string _accessToken;

        public WebApiHandler(string accessToken = "")
        {
            this._baseUrl = Utility.ConfigSettings.WebApiUrl;
            this._responseMessage = new Utility.ResponseMessage();
            //Setter method to check and set the access token for api request
            if (string.IsNullOrEmpty(this._accessToken) && HttpContext.Current.Session["AccessToken"] != null)
            {
                this._accessToken = HttpContext.Current.Session["AccessToken"].ToString();
            }
        }
        
        /// <summary>
        /// Generic method to handle api of get type
        /// </summary>
        /// <param name="url"></param>
        /// <returns>Returns ResponseMessage ViewModel</returns>
        public Utility.ResponseMessage Get(string url)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(_baseUrl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                HttpResponseMessage Res = client.GetAsync(url).Result;

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api 
                    _responseMessage.MessageType = "Success";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;
                    _responseMessage.StatusCode = Res.StatusCode;
                }
                else
                {
                    _responseMessage.MessageType = "Error";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;
                    _responseMessage.StatusCode = Res.StatusCode;
                }

            }

            return _responseMessage;
        }
    }
}