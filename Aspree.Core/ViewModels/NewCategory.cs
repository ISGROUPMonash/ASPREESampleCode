using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class NewCategory
    {
        /// <summary>
        /// Category string
        /// </summary>
        [Required]
         public string Category { get; set; }

            public static void WriteLog(string logMessage,
            [System.Runtime.CompilerServices.CallerLineNumber] int lineNumber = 0,
            [System.Runtime.CompilerServices.CallerMemberName] string caller = null)
             {
            try 
            {
                logMessage = "Line:" + lineNumber + "#Caller:" + caller + "\t " + logMessage + "\t "+ GetLocalIPAddress();

                System.IO.FileStream objFilestream = new System.IO.FileStream(string.Format("{0}\\{1}", System.Web.Hosting.HostingEnvironment.MapPath("~/"), "log-aspree-core.log"), System.IO.FileMode.Append, System.IO.FileAccess.Write);
                //System.IO.FileStream objFilestream = new System.IO.FileStream(string.Format("{0}\\{1}", System.IO.Path.GetTempPath(), strFileName), System.IO.FileMode.Append, System.IO.FileAccess.Write);
                System.IO.StreamWriter objStreamWriter = new System.IO.StreamWriter((System.IO.Stream)objFilestream);
                objStreamWriter.WriteLine(logMessage);
                objStreamWriter.Close();
                objFilestream.Close();
                //return true;
            }
            catch (Exception ex)
            {
                //return false;
            }
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

    public class NewCategoryExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new NewCategory
            {
                Category = "Category example",
            };
        }
    }
}
