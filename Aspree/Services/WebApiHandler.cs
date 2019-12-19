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
    public class WebApiHandler
    {
        private readonly string _baseUrl;
        private readonly string _testBaseUrl;
        private readonly string _baseOktaUrl;
        private Utility.ResponseMessage _responseMessage;
        private readonly string _accessToken;

        public WebApiHandler(string accessToken = "")
        {
            this._accessToken = accessToken;
            this._baseUrl = Utility.ConfigSettings.WebApiUrl;
            this._testBaseUrl = Utility.ConfigSettings.TestWebApiUrl;
            this._baseOktaUrl = Utility.ConfigSettings.OktaDomain;
            this._responseMessage = new Utility.ResponseMessage();

            if (string.IsNullOrEmpty(this._accessToken) && HttpContext.Current.Session["AccessToken"] != null)
            {
                this._accessToken = HttpContext.Current.Session["AccessToken"].ToString();
            }
        }

        public Utility.ResponseMessage GetOktaToken(string code, string redirect)
        {
            using (var client = new HttpClient())
            {
                string url = _baseOktaUrl + "token";
                //Passing service base url  
                client.DefaultRequestHeaders.Clear();

                var credential = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Utility.ConfigSettings.OktaClientId + ":" + Utility.ConfigSettings.OktaClientSecret));
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + credential);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("redirect_uri", redirect),
                    new KeyValuePair<string, string>("code", code)
                });

                HttpResponseMessage Res = client.PostAsync(url, formContent).Result;

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    _responseMessage.MessageType = "Success";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    _responseMessage.MessageType = "Error";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;
                }
            }
            return _responseMessage;
        }

        public Utility.ResponseMessage GetOktaUserInfo(string endPoint, string accessToken)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url 
                string url = _baseOktaUrl + endPoint;
                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                HttpResponseMessage Res = client.GetAsync(url).Result;

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api 
                    _responseMessage.MessageType = "Success";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;

                }
                else
                {
                    _responseMessage.MessageType = "Error";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;
                }

            }

            return _responseMessage;
        }



        public Utility.ResponseMessage GetGoogleToken(string code, string redirect, string clientId, string clientSecret, string tokenEndpoint)
        {
            using (var client = new HttpClient())
            {
                string url = tokenEndpoint + "token";

                //Passing service base url  
                client.DefaultRequestHeaders.Clear();

                var credential = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(clientId + ":" + clientSecret));
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + credential);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("redirect_uri", redirect),
                    new KeyValuePair<string, string>("code", code)
                });

                HttpResponseMessage Res = client.PostAsync(url, formContent).Result;

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    _responseMessage.MessageType = "Success";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    _responseMessage.MessageType = "Error";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;
                }
            }
            return _responseMessage;
        }

        public Utility.ResponseMessage GetGoogleUserInfo(string endPoint, string accessToken, string userinfoEndpoint)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url 
                string url = userinfoEndpoint + endPoint;
                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

                HttpResponseMessage Res = client.GetAsync(url).Result;

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api 
                    _responseMessage.MessageType = "Success";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    _responseMessage.MessageType = "Error";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;
                }

            }

            return _responseMessage;
        }



        public Utility.ResponseMessage Login(string userName, string password, string grantType = "password", bool isTest = false)
        {
            using (var client = new HttpClient())
            {
                string url = _baseUrl + "accesstoken";
                //Passing service base url  
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders.Clear();

                var credential = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("client:secret"));
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + credential);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", grantType),
                    new KeyValuePair<string, string>("username", userName),
                    new KeyValuePair<string, string>("password", password)
                });
                HttpResponseMessage Res = new HttpResponseMessage();
                if (isTest)
                {
                    url = _testBaseUrl + "accesstoken";
                }
                Res = client.PostAsync(url, formContent).Result;

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    _responseMessage.MessageType = "Success";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    _responseMessage.MessageType = "Error";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;
                }
            }
            return _responseMessage;
        }

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

        public Utility.ResponseMessage Post(string url, object data)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(_baseUrl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                var myContent = JsonConvert.SerializeObject(data);
                var httpContent = new StringContent(myContent, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage Res = client.PostAsync(url, httpContent).Result;

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    _responseMessage.MessageType = "Success";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    _responseMessage.MessageType = "Error";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;
                }
            }
            return _responseMessage;
        }

        public Utility.ResponseMessage Put(string url, object data)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(_baseUrl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                var myContent = JsonConvert.SerializeObject(data);
                var httpContent = new StringContent(myContent, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage Res = client.PutAsync(url, httpContent).Result;

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api
                    _responseMessage.MessageType = "Success";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;

                }
                else
                {
                    _responseMessage.MessageType = "Error";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;
                }
            }
            return _responseMessage;
        }

        public Utility.ResponseMessage Delete(string url)
        {
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(_baseUrl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _accessToken);

                HttpResponseMessage Res = client.DeleteAsync(url).Result;

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api
                    _responseMessage.MessageType = "Success";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    _responseMessage.MessageType = "Error";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;
                }
            }
            return _responseMessage;
        }


        public Utility.ResponseMessage InternalLogin(string loginType, string userId_ProjectId, string grantType = "password", bool isTest = false)
        {
            using (var client = new HttpClient())
            {
                string url = _baseUrl + "accesstoken";
                //Passing service base url  
                client.BaseAddress = new Uri(_baseUrl);
                client.DefaultRequestHeaders.Clear();

                var credential = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("client:secret"));
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + credential);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", grantType),
                    new KeyValuePair<string, string>("username", "InternalProjectLogin"),
                    new KeyValuePair<string, string>("password", userId_ProjectId)
                });

                HttpResponseMessage Res = new HttpResponseMessage();
                
                if (isTest)
                {
                    url = _testBaseUrl + "accesstoken";
                }

                Res = client.PostAsync(url, formContent).Result;

                Aspree.Core.ViewModels.NewCategory.WriteLog("call access token response:" + Res.Content.ReadAsStringAsync().Result);
                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    _responseMessage.MessageType = "Success";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    _responseMessage.MessageType = "Error";
                    _responseMessage.Content = Res.Content.ReadAsStringAsync().Result;
                }
            }
            return _responseMessage;
        }
    }
}