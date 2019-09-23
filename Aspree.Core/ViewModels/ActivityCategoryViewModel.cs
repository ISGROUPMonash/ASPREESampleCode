using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    /// <summary>
    /// ActivityCategory Model
    /// </summary>
    public class ActivityCategoryViewModel
    {
        /// <summary>
        /// Id of ActivityCategory
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name of ActivityCategory
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// CreatedBy Guid of ActivityCategory 
        /// </summary>
        public Guid CreatedBy { get; set; }
        /// <summary>
        /// Created Date of ActivityCategory
        /// </summary>
        public System.DateTime CreatedDate { get; set; }
        /// <summary>
        /// ModifiedBy Guid of ActivityCategory
        /// </summary>
        public Nullable<Guid> ModifiedBy { get; set; }
        /// <summary>
        /// Modification Date of ActivityCategory
        /// </summary>
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        /// <summary>
        /// DeactivatedBy Guid of ActivityCategory
        /// </summary>
        public Nullable<Guid> DeactivatedBy { get; set; }
        /// <summary>
        /// Deactivation Date of ActivityCategory
        /// </summary>
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        /// <summary>
        /// Guid of ActivityCategory
        /// </summary>
        public System.Guid Guid { get; set; }
        /// <summary>
        /// Activity List
        /// </summary>
        public IList<SubCategoryViewModel> Activities { get; set; }
        /// <summary>
        /// Guid of Tenant
        /// </summary>
        public Guid? TenantId { get; set; }
    }

}
