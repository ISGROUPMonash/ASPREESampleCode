using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class ProjectLinkageViewModel
    {
        public int Id { get; set; }
        public System.Guid LinkedProjectId { get; set; }
        public Int32 PersonEntityId { get; set; }
        public bool IsActiveProjectUser { get; set; }
        public System.Guid ProjectRoleId { get; set; }
        public String ProjectRoleName { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }
        public System.Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public System.Guid ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public System.Guid DeactivatedBy { get; set; }
        public DateTime DateDeactivated { get; set; }
        public System.Guid Guid { get; set; }
        public System.Guid FormId { get; set; }
        public System.Guid ActivityId { get; set; }
        public System.Guid ProjectId { get; set; }
        public string ProjectLinkageJson { get; set; }
    }

    public class NewProjectLinkageViewModel
    {
        public int Id { get; set; }
        public System.Guid LinkedProjectId { get; set; }
        public Int32 PersonEntityId { get; set; }
        public bool IsActiveProjectUser { get; set; }
        public System.Guid ProjectRoleId { get; set; }
        public String ProjectRoleName { get; set; }
        public DateTime ProjectStartDate { get; set; }
        public DateTime ProjectEndDate { get; set; }
        public System.Guid ProjectId { get; set; }
        public string ProjectLinkageJson { get; set; }
        public System.Guid FormId { get; set; }
        public System.Guid ActivityId { get; set; }
    }
}
