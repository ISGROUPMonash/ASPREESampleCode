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
    public enum ActivityStatusTypes
    {
        /// <summary>
        /// 
        /// </summary>
        Active = 1,
        /// <summary>
        /// 
        /// </summary>
        InActive = 2,
        /// <summary>
        /// 
        /// </summary>
        Draft = 3,
    }
    /// <summary>
    /// 
    /// </summary>
    public enum ActivityCategories
    {
        /// <summary>
        /// 
        /// </summary>
        Default = 1,
        /// <summary>
        /// 
        /// </summary>
        Custom = 2,
    }
    /// <summary>
    /// 
    /// </summary>
    public enum DefaultActivityType
    {
        /// <summary>
        /// 
        /// </summary>
        Custom = 1,
        /// <summary>
        /// 
        /// </summary>
        Default = 0,

    }

    /// <summary>
    /// 
    /// </summary>
    public enum ActivityDeploymentStatus
    {
        /// <summary>
        /// 
        /// </summary>
        Created = 1,
        /// <summary>
        /// 
        /// </summary>
        Scheduled = 2,
        /// <summary>
        /// 
        /// </summary>
        Pushed = 3,
        /// <summary>
        /// 
        /// </summary>
        Deployed = 4,
    }
}
