using Aspree.WebApi.ExceptionHander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Aspree.WebApi.Controllers
{
     
    [Utilities.LogAttribute]
    public class BaseController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid LoggedInUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid LoggedInUserTenantId { get; set; }


        /// <summary>
        /// write logs
        /// </summary>
        /// <param name="logMessage">message to be write</param>
        /// <param name="lineNumber">line no. from this method is called</param>
        /// <param name="caller">caller method name</param>
        /// <returns></returns>
        [NonAction]
        public void WriteLog(string logMessage,
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
            [System.Runtime.CompilerServices.CallerMemberName] string caller = null)
        {
            try
            {
                logMessage = "Line:" + lineNumber + "#Caller:" + caller + "\t " + logMessage + "\t " + GetLocalIPAddress();

                System.IO.FileStream objFilestream = new System.IO.FileStream(string.Format("{0}\\{1}", System.Web.Hosting.HostingEnvironment.MapPath("~/"), "log-aspree-webapi.log"), System.IO.FileMode.Append, System.IO.FileAccess.Write);
                System.IO.StreamWriter objStreamWriter = new System.IO.StreamWriter((System.IO.Stream)objFilestream);
                objStreamWriter.WriteLine(logMessage);
                objStreamWriter.Close();
                objFilestream.Close();
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Exception handler for GUID type parameter
        /// </summary>
        [NonAction]
        public void GuidExceptionHandler()
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)
            {
                Content = new StringContent("Type of requested parameter is invalid."),
            });
        }



        public static string GetLocalIPAddress()
        {

            var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return ("No network adapters with an IPv4 address in the system!");
        }

    }
}
