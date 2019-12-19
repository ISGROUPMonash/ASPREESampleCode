using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.Enum
{
    /// <summary>
    /// 
    /// </summary>
    public enum AuthenticationTypes
    {
        /// <summary>
        /// 
        /// </summary>
        Local_Password = 1,
        /// <summary>
        /// 
        /// </summary>
        OpenID_Connect = 2,
        //Google = 3,
        //Monash_via_Okta = 2,
        //Okta = 2,

        //Password = 1,
        //Institutional = 2,
        //Google = 3
    }

    /// <summary>
    /// 
    /// </summary>
    public enum LoginAuthenticationTypes
    {
        /// <summary>
        /// 
        /// </summary>
        Local_Password = 1,
        /// <summary>
        /// 
        /// </summary>
        OpenID_Connect = 2,
        /// <summary>
        /// 
        /// </summary>
        Google = 3,
    }
    //public enum ManageAuthenticationTypes
    //{
    //    Local_Password = 1,
    //    Okta = 2,
    //}
}
