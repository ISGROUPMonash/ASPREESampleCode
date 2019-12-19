using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Aspree.WebApi.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfigSettings
    {
        private ConfigSettings()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public static string WebUrl
        {
            get { return ConfigurationManager.AppSettings["WebUrl"].ToString(); }
        }


        /// <summary>
        /// 
        /// </summary>
        public static string WebUrlTestSite
        {
            get { return ConfigurationManager.AppSettings["WebUrlTestSite"].ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string ProfileImageBasePath
        {
            get { return ConfigurationManager.AppSettings["ProfileImageBasePath"].ToString(); }
        }


        /// <summary>
        /// 
        /// </summary>
        public static string UploadsBasePath
        {
            get { return ConfigurationManager.AppSettings["UploadsBasePath"].ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string UploadsDataEntryVariablePath
        {
            get { return ConfigurationManager.AppSettings["UploadsDataEntryVariablePath"].ToString(); }
        }


        /// <summary>
        /// 
        /// </summary>
        public static string SmtpHost
        {
            get { return ConfigurationManager.AppSettings["SmtpHost"].ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string EmailFrom
        {
            get { return ConfigurationManager.AppSettings["EmailFrom"].ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static int Port
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["Port"].ToString()); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string DateFormat
        {
            get { return ConfigurationManager.AppSettings["DateFormat"].ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string ReportAPIUrl
        {
            get { return ConfigurationManager.AppSettings["ReportAPIUrl"].ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string TestSiteKeyword
        {
            get { return  
                    !string.IsNullOrEmpty(ConfigurationManager.AppSettings["TestSiteKeyword"].ToString()) 
                    ? ConfigurationManager.AppSettings["TestSiteKeyword"].ToString().ToLower() 
                    : ConfigurationManager.AppSettings["TestSiteKeyword"].ToString();
            }
        }
    }
}