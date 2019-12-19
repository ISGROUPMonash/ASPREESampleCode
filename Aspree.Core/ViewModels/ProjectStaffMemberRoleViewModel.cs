using System;
using Swashbuckle.Examples;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class ProjectStaffMemberRoleViewModel
    {
        /// <summary>
        /// Id of Project staff member 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ProjectId of project
        /// </summary>
        public int ProjectId { get; set; }
        /// <summary>
        /// UserId of Project
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// RoleId of Project
        /// </summary>
        public int RoleId { get; set; }
        /// <summary>
        /// Project CreatedBy
        /// </summary>
        public int CreatedBy { get; set; }
        /// <summary>
        /// Project Guid
        /// </summary>
        public System.Guid Guid { get; set; }
    }
    public class NewProjectStaffMemberRoleViewModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Guid of Project
        /// </summary>
        public System.Guid? ProjectGuid { get; set; }
        /// <summary>
        /// UserGuid
        /// </summary>
        public System.Guid? UserGuid { get; set; }
        /// <summary>
        /// RoleGuid
        /// </summary>
        public System.Guid? RoleGuid { get; set; }
        /// <summary>
        /// Guid
        /// </summary>
        public System.Guid Guid { get; set; }
        /// <summary>
        /// UserName of Project
        /// </summary>
        public string ProjectUserName { get; set; }
        /// <summary>
        /// Role Name of ProjectUser
        /// </summary>
        public string ProjectUserRoleName { get; set; }
        /// <summary>
        /// StaffCreatedDate
        /// </summary>
        public DateTime? StaffCreatedDate { get; set; }
        /// <summary>
        /// StaffCreatedDateString
        /// </summary>
        public string StaffCreatedDateString { get; set; }
    }


    public class ProjectStaffMemberRoleViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<ProjectStaffMemberRoleViewModel>()
            {
                new ProjectStaffMemberRoleViewModel
                {
                    Id = 1,
                    ProjectId = 1,
                    UserId = 1,
                    RoleId = 1,
                    CreatedBy = 1,
                    Guid = Guid.NewGuid()
                }
            };
        }
    }
}
