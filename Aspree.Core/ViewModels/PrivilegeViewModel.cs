using System;
using System.Collections.Generic;
using Swashbuckle.Examples;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class PrivilegeViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Name of Privilege
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// IsProjectAdmin
        /// </summary>
        public bool IsProjectAdmin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int CreatedBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public System.DateTime CreatedDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Nullable<int> ModifiedBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Nullable<int> DeactivatedBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public System.Guid Guid { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class PrivilegeSmallViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }

        /// <summary>
        /// Name of Privilege
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Guid
        /// </summary>
        public System.Guid Guid { get; set; }
    }

    public class GetAllPrivilegeSmallViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<PrivilegeSmallViewModel>()
            {
                new PrivilegeSmallViewModel
                {
                    Name ="Example Name",
                    Guid=Guid.NewGuid()
                 }
            };
        }
    }
}
