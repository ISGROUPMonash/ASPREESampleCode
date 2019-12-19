using System;
using Swashbuckle.Examples;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Aspree.Core.ViewModels
{
    public class FormDataEntryProjectsViewModel
    {
        /// <summary>
        /// Id of Project
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name of Project
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// Subtype of Project
        /// </summary>
        public string ProjectSubtype { get; set; }
        /// <summary>
        /// CondData of project
        /// </summary>
        public string ConfData { get; set; }
        /// <summary>
        /// CnstModel
        /// </summary>
        public string CnstModel { get; set; }
        /// <summary>
        /// Ethics 
        /// </summary>
        public string Ethics { get; set; }
        /// <summary>
        /// DataStore
        /// </summary>
        public string DataStore { get; set; }
        /// <summary>
        /// ProDt
        /// </summary>
        public string ProDt { get; set; }
        /// <summary>
        /// Guid of Project
        /// </summary>
        public Guid Guid { get; set; }
        public List<NewProjectStaffMemberRoleViewModel> ProjectStaffMembersRoles { get; set; }
        /// <summary>
        /// Project created By
        /// </summary>
        public Guid CreatedBy { get; set; }
        /// <summary>
        /// Project Created date
        /// </summary>
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// Logo of Project
        /// </summary>

        public string ProjectLogo { get; set; }
        /// <summary>
        /// Color of project
        /// </summary>
        public string ProjectColor { get; set; }
        /// <summary>
        /// Display name of project
        /// </summary>
        public string ProjectDisplayName { get; set; }
        /// <summary>
        /// Text color of project display name
        /// </summary>
        public string ProjectDisplayNameTextColour { get; set; }


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
    public class GetAllFormDataEntryProjectsViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<FormDataEntryProjectsViewModel>()
            {
                new FormDataEntryProjectsViewModel
                {
                    Id = 1,
                    ProjectName = "Example Project Name",
                    ProjectSubtype = "Example Project Subtype",
                    ConfData = "Example ConfData",
                    CnstModel = "Example CnstModel",
                    Ethics = "Example Ethics",
                    DataStore = "Example DataStore",
                    ProDt = "Example ProDt",
                    Guid = Guid.NewGuid(),
                    ProjectStaffMembersRoles = new List<NewProjectStaffMemberRoleViewModel>()
                    {
                    new NewProjectStaffMemberRoleViewModel
                    {
                              Id=1,
                             ProjectGuid=Guid.NewGuid(),
                             UserGuid=Guid.NewGuid(),
                             RoleGuid=Guid.NewGuid(),
                             Guid=Guid.NewGuid(),
                             ProjectUserName="Example Project User Name",
                             ProjectUserRoleName="Example Project User Role Name",
                             StaffCreatedDate=DateTime.UtcNow,
                             StaffCreatedDateString="Example date",
                    }
                  },
                      CreatedBy=Guid.NewGuid(),
                      CreatedDate=DateTime.UtcNow,
                      ProjectLogo="Logo",
                      ProjectColor="Project Colour",
                      ProjectDisplayName="Example Display Name",
                      ProjectDisplayNameTextColour="Text Colour"


                 }
            };
        }
    }

    public class FormDataEntryProjectsViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
             return new FormDataEntryProjectsViewModel
            {
                Id = 1,
                ProjectName = "Example Project Name",
                ProjectSubtype = "Example Project Subtype",
                ConfData = "Example ConfData",
                CnstModel = "Example CnstModel",
                Ethics = "Example Ethics",
                DataStore = "Example DataStore",
                ProDt = "Example ProDt",
                Guid = Guid.NewGuid(),
                ProjectStaffMembersRoles = new List<NewProjectStaffMemberRoleViewModel>()
                    {
                    new NewProjectStaffMemberRoleViewModel
                    {
                              Id=1,
                             ProjectGuid=Guid.NewGuid(),
                             UserGuid=Guid.NewGuid(),
                             RoleGuid=Guid.NewGuid(),
                             Guid=Guid.NewGuid(),
                             ProjectUserName="Example Project User Name",
                             ProjectUserRoleName="Example Project User Role Name",
                             StaffCreatedDate=DateTime.UtcNow,
                             StaffCreatedDateString="Example date",
                    }
                  },
                CreatedBy = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                ProjectLogo = "Logo",
                ProjectColor = "Project Colour",
                ProjectDisplayName = "Example Display Name",
                ProjectDisplayNameTextColour = "Text Colour"


            };
            
        }
    }
}
