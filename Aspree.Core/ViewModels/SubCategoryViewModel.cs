using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class SubCategoryViewModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Name of SubCategory
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Guid
        /// </summary>
        public Guid Guid { get; set; }
        /// <summary>
        /// Type of subcategory
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Status of subcategory
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// Guid of ProjectId
        /// </summary>
        public Guid? ProjectId { get; set; }
        /// <summary>
        /// IsApproved true/false
        /// </summary>
        public bool IsApproved { get; set; }
        /// <summary>
        /// IsPublished true/false
        /// </summary>
        public bool IsPublished { get; set; }
        /// <summary>
        /// Subcategory CreatedBy
        /// </summary>
        public int CreatedBy { get; set; }
        /// <summary>
        /// Deactivated date of Subcategory
        /// </summary>
        public DateTime? DateDeactivated { get; set; }
        /// <summary>
        /// RepeatationCount
        /// </summary>
        public int? RepeatationCount { get; set; }
        /// <summary>
        /// Schedule type
        /// </summary>
        public int ScheduleType { get; set; }
        /// <summary>
        /// Start date
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// End date
        /// </summary>
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// RepeatationOffset
        /// </summary>
        public int? RepeatationOffset { get; set; }
        /// <summary>
        /// IsDefaultVariable
        /// </summary>
        public int IsDefaultVariable { get; set; }
        /// <summary>
        /// List of EntityType
        /// </summary>
        public List<Guid> EntityType { get; set; }
        /// <summary>
        /// Status of deployment
        /// </summary>
        public int? DeploymentStatus { get; set; }
        /// <summary>
        /// IsAllVariableApprovedOfActivity true/false
        /// </summary>
        public bool IsAllVariableApprovedOfActivity { get; set; }
    }
}
