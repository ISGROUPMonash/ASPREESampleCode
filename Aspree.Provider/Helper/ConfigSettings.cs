using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Aspree.Provider.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfigSettings
    {
        private ConfigSettings()
        {

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
        public static string OktaAuthenticationProviderClaim
        {
            get
            {
                return ConfigurationManager.AppSettings["OktaAuthenticationProviderClaim"].ToString();
            }
        }
        public static string OktaAuthenticationProviderScope
        {
            get
            {
                return ConfigurationManager.AppSettings["OktaAuthenticationProviderScope"].ToString();
            }
        }
        public static string OktaAuthenticationProviderAuthTypeName
        {
            get
            {
                return ConfigurationManager.AppSettings["OktaAuthenticationProviderAuthTypeName"].ToString();
            }
        }
    }
}