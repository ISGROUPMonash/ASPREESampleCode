using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Aspree.Utility
{
    /// <summary>
    /// Get app setting values for the Web.config file
    /// </summary>
    public class ConfigSettings
    {
        /// <summary>
        /// Get web api url from the web.config file
        /// </summary>
        public static string WebApiUrl {
            get {
                return ConfigurationManager.AppSettings["WebApiUrl"].ToString();
            }
        }
    }
}