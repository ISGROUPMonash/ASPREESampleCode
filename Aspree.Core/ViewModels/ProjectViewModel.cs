using System;
using Swashbuckle.Examples;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Aspree.Core.ViewModels
{
    public class ProjectViewModel
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// ProjectName of Project
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// ProjectStatusId of Project
        /// </summary>
        public int ProjectStatusId { get; set; }
        /// <summary>
        /// State of Project 
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// Version of Project
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// PreviousProjectId of Project
        /// </summary>
        public Nullable<int> PreviousProjectId { get; set; }
        /// <summary>
        /// ProjectUrl of Project
        /// </summary>
        public string ProjectUrl { get; set; }
        /// <summary>
        /// CheckListID of Project
        /// </summary>
        public Nullable<int> CheckListID { get; set; }
        /// <summary>
        /// StartDate of Project
        /// </summary>
        public System.DateTime StartDate { get; set; }
        /// <summary>
        /// EndDate of Project
        /// </summary>
        public Nullable<System.DateTime> EndDate { get; set; }
        /// <summary>
        /// Project CreatedBy
        /// </summary>
        public Guid CreatedBy { get; set; }
        /// <summary>
        /// CreatedDate of Project
        /// </summary>
        public System.DateTime CreatedDate { get; set; }
        /// <summary>
        /// Project ModifiedBy
        /// </summary>
        public Nullable<Guid> ModifiedBy { get; set; }
        /// <summary>
        /// Project ModifiedDate
        /// </summary>
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        /// <summary>
        /// Project DeactivatedBy
        /// </summary>
        public Nullable<Guid> DeactivatedBy { get; set; }
        /// <summary>
        /// DateDeactivated of Project 
        /// </summary>
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        /// <summary>
        /// Guid of Project 
        /// </summary>
        public System.Guid Guid { get; set; }
        /// <summary>
        /// ProjectUsers of Project 
        /// </summary>
        public List<UserLoginViewModel> ProjectUsers { get; set; }
        /// <summary>
        /// Status of Project 
        /// </summary>
        public string Status { get { return ((Core.Enum.ProjectStatusTypes)this.ProjectStatusId).ToString(); } }

        /// <summary>
        /// ProjectUserId of Project 
        /// </summary>
        public Guid ProjectUserId { get; set; }//19102018
        /// <summary>
        /// Role list of Project 
        /// </summary>
        public List<System.Web.Mvc.SelectListItem> Role { get; set; }
        /// <summary>
        /// TenantId of Project 
        /// </summary>
        public Guid TenantId { get; set; }
        /// <summary>
        /// RoleId of variable
        /// </summary>
        public int RoleId { get; set; }
        /// <summary>
        /// UserList of Project 
        /// </summary>
        public List<System.Web.Mvc.SelectListItem> UserList { get; set; }
        /// <summary>
        /// RoleGuid of Project 
        /// </summary>
        public Guid RoleGuid { get; set; }
        //public Guid ProjectRoleUserId { get; set; }
        /// <summary>
        /// ProjectStaffMembersRoles of Project 
        /// </summary>
        public List<NewProjectStaffMemberRoleViewModel> ProjectStaffMembersRoles { get; set; }


        /// <summary>
        /// Recruitment Start Date of Project
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? RecruitmentStartDate { get; set; }
        /// <summary>
        /// Recruitment End Date of Project
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? RecruitmentEndDate { get; set; }
    }
    public class NewProjectViewModel
    {
        [Required]
        public string ProjectName { get; set; }
        public int Version { get; set; }
        public Nullable<int> PreviousProjectId { get; set; }
        public string ProjectUrl { get; set; }
        public Nullable<int> CheckListID { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }

        public Guid TenantId { get; set; }
        public int RoleId { get; set; }
        public Guid RoleGuid { get; set; }
        public Guid ProjectUserId { get; set; }
        public List<NewProjectStaffMemberRoleViewModel> ProjectStaffMembersRoles { get; set; }
    }

    public class EditProjectViewModel
    {
        [Required]
        public string ProjectName { get; set; }
        public int State { get; set; }
        public int Version { get; set; }
        public Nullable<int> PreviousProjectId { get; set; }
        public string ProjectUrl { get; set; }
        public Nullable<int> CheckListID { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public System.Guid Guid { get; set; }
        public System.Guid ProjectUserId { get; set; }
        public List<NewProjectStaffMemberRoleViewModel> ProjectStaffMembersRoles { get; set; }
    }

    public class ProjectBasicDetailsViewModel
    {
        /// <summary>
        /// Id of Project Basic details
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Guid of Project
        /// </summary>
        public Guid Guid { get; set; }
        /// <summary>
        /// Name of Project
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Logo of Project 
        /// </summary>
        public string ProjectLogo { get; set; }
        /// <summary>
        /// Color of project 
        /// </summary>
        public string ProjectColor { get; set; }
        /// <summary>
        /// Project Display Name
        /// </summary>
        public string ProjectDisplayName { get; set; }
        /// <summary>
        /// Text color of  Project Display Name
        /// </summary>
        public string ProjectDisplayNameTextColour { get; set; }
    }

    public class GetAllProjectViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<ProjectViewModel>()
            {
                new ProjectViewModel
                {
                   ProjectName="Example Project",
                   ProjectStatusId=1,
                   State=1,
                   Version=1,
                   PreviousProjectId=null,
                   ProjectUrl="Test Url",
                   CheckListID=1,
                   StartDate=DateTime.UtcNow,
                   EndDate=DateTime.UtcNow,
                   CreatedBy=Guid.NewGuid(),
                   CreatedDate=DateTime.UtcNow,
                   ModifiedBy=Guid.NewGuid(),
                   ModifiedDate=DateTime.UtcNow,
                   DeactivatedBy=Guid.NewGuid(),
                   DateDeactivated=DateTime.UtcNow,
                   Guid=Guid.NewGuid(),
                   ProjectUsers=new List<UserLoginViewModel>()
                  {
                        new UserLoginViewModel
                        {

                               FirstName="Example First Name",
                               LastName="Last Name",
                               Email="text@gmail.com",
                               Mobile="0215648965",
                               Address="Example Address",
                               SecurityQuestionId=Guid.NewGuid(),
                               Answer="Test Answer",
                               TenantId=Guid.NewGuid(),
                               AuthTypeId=1,
                              CreatedBy=Guid.NewGuid(),
                              CreatedDate=DateTime.UtcNow,
                              ModifiedBy=Guid.NewGuid(),
                              ModifiedDate=DateTime.UtcNow,
                              DeactivatedBy=Guid.NewGuid(),
                              DateDeactivated=DateTime.UtcNow,
                              Guid=Guid.NewGuid(),
                             RoleId=Guid.NewGuid(),
                             RoleName="Example Role Name",
                             Status=1,
                             Profile="Test Profile",
                             TempGuid=Guid.NewGuid(),
                             UserTypeId=1,
                             UserName="Text Box",
                             IsApiAccessEnabled=true,
                             IsUserApprovedBySystemAdmin=true

                        }

                  },
                     //public string Status { get { return ((Core.Enum.ProjectStatusTypes)this.ProjectStatusId).ToString(); } }
                     ProjectUserId=Guid.NewGuid(),
                     Role=new List<System.Web.Mvc.SelectListItem>(),
                     TenantId=Guid.NewGuid(),
                     RoleId=1,
                     UserList=new List<System.Web.Mvc.SelectListItem>(),
                     RoleGuid=Guid.NewGuid(),
                     ProjectStaffMembersRoles=new List<NewProjectStaffMemberRoleViewModel>()
                     {
                         new NewProjectStaffMemberRoleViewModel
                         {
                         ProjectGuid=Guid.NewGuid(),
                         UserGuid=Guid.NewGuid(),
                         RoleGuid=Guid.NewGuid(),
                         Guid=Guid.NewGuid(),
                         ProjectUserName="Example User Name",
                         ProjectUserRoleName="Example Project User Role",
                         StaffCreatedDate=DateTime.UtcNow,
                         StaffCreatedDateString="Test Created Date String"

                        }

                     }
                 }
            };
        }
    }
    public class ProjectBasicDetailsViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<ProjectBasicDetailsViewModel>()
            {
                new ProjectBasicDetailsViewModel
                {
                      Id = 1,
                      Guid = Guid.NewGuid(),
                      Name="Example Project Basic Details",
                      ProjectLogo="Logo",
                      ProjectColor="Blue",
                      ProjectDisplayName="Text Box",
                      ProjectDisplayNameTextColour="Text Colour",
                 }
            };
        }
    }

}
