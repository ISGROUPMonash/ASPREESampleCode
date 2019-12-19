using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Aspree.Utility
{
    public class ConfigSettings
    {
        public static string WebApiUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["WebApiUrl"].ToString();
            }
        }
        public static string TestWebApiUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["TestWebApiUrl"].ToString();
            }
        }

        public static string GoogleClientId
        {
            get
            {
                return ConfigurationManager.AppSettings["GoogleClientId"].ToString();
            }
        }

        public static string GoogleClientSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["GoogleClientSecret"].ToString();
            }
        }

        public static string OktaClientId
        {
            get
            {
                return ConfigurationManager.AppSettings["OktaClientId"].ToString();
            }
        }

        public static string OktaClientSecret
        {
            get
            {
                return ConfigurationManager.AppSettings["OktaClientSecret"].ToString();
            }
        }

        public static string OktaDomain
        {
            get
            {
                return ConfigurationManager.AppSettings["OktaDomain"].ToString();
            }
        }

        public static string OktaRedirectUri
        {
            get
            {
                return ConfigurationManager.AppSettings["OktaRedirectUri"].ToString();
            }
        }

        public static string OktaPostLogoutRedirectUri
        {
            get
            {
                return ConfigurationManager.AppSettings["OktaPostLogoutRedirectUri"].ToString();
            }
        }

        public static string DateFormat
        {
            get { return ConfigurationManager.AppSettings["DateFormat"].ToString(); }
        }

        public static string WebApiUploadsPath
        {
            get { return ConfigurationManager.AppSettings["WebApiUploadsPath"].ToString(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static string WebUrlTestSite
        {
            get { return ConfigurationManager.AppSettings["WebUrlTestSite"].ToString(); }
        }
        public static string TestSiteKeyword
        {
            get {
                return
                    !string.IsNullOrEmpty(ConfigurationManager.AppSettings["TestSiteKeyword"].ToString())
                    ? ConfigurationManager.AppSettings["TestSiteKeyword"].ToString().ToLower()
                    : ConfigurationManager.AppSettings["TestSiteKeyword"].ToString();
            }
        }
    }
}