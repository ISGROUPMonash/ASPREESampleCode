using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class FormDataEntryViewModel
    {
        /// <summary>
        /// Id of form data entry
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Guid of project
        /// </summary>
        public System.Guid ProjectId { get; set; }
        /// <summary>
        /// Guid of entity
        /// </summary>
        public System.Guid EntityId { get; set; }
        /// <summary>
        /// Id of subject
        /// </summary>
        public Nullable<int> SubjectId { get; set; }
        /// <summary>
        /// status of form data entry
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// Guid of entity that created the form data entry
        /// </summary>
        public System.Guid CreatedBy { get; set; }
        /// <summary>
        /// Date of form data entry creation
        /// </summary>
        public System.DateTime CreatedDate { get; set; }
        /// <summary>
        /// id of entity that edit the form data entry
        /// </summary>
        public Nullable<int> ModifiedBy { get; set; }
        /// <summary>
        /// Date of form data entry modification
        /// </summary>
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        /// <summary>
        /// id of entity that deleted the form data entry
        /// </summary>
        public Nullable<int> DeactivatedBy { get; set; }
        /// <summary>
        /// Date of deletion  of form data entry
        /// </summary>
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        /// <summary>
        /// Guid of form data entry
        /// </summary>
        public System.Guid Guid { get; set; }
        /// <summary>
        /// Guid of activity
        /// </summary>
        public System.Guid ActivityId { get; set; }
        /// <summary>
        /// Guid of tenant
        /// </summary>
        public Nullable<System.Guid> TenantId { get; set; }
        /// <summary>
        /// model of activity
        /// </summary>
        public ActivityViewModel Activity { get; set; }
        /// <summary>
        /// list of FormDataEntryVariableViewModel
        /// </summary>
        public List<FormDataEntryVariableViewModel> FormDataEntryVariable { get; set; }
        /// <summary>
        /// Guid of form
        /// </summary>
        public Guid FormId { get; set; }
        /// <summary>
        /// Id of entity
        /// </summary>
        public string ParticipantId { get; set; }
        /// <summary>
        /// Name of form
        /// </summary>
        public string FormTitle { get; set; }
        /// <summary>
        /// userlogin table id of form dataentry entity
        /// </summary>
        public int? ThisUserId { get; set; }
        /// <summary>
        /// entity id of parent entity
        /// </summary>
        public int? ParentEntityNumber { get; set; }
        /// <summary>
        /// status id of deployed project
        /// </summary>
        public Nullable<int> ProjectDeployStatus { get; set; }
        /// <summary>
        /// deployment id of project
        /// </summary>
        public string ProjectDeployedId { get; set; }
        /// <summary>
        /// version of deployed project
        /// </summary>
        public string ProjectDeployedVersion { get; set; }

        /// <summary>
        /// id of summary page activity
        /// </summary>
        public string SummaryPageActivityObjId { get; set; }
    }

    public class GetAllFormDataEntryViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<FormDataEntryViewModel>()
            {
                new FormDataEntryViewModel
                {
                Activity = new ActivityViewModel(),
                ActivityId = Guid.NewGuid(),
                CreatedBy = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                DateDeactivated = DateTime.UtcNow,
                DeactivatedBy = 1,
                EntityId = Guid.NewGuid(),
                FormDataEntryVariable = null,
                FormId = Guid.NewGuid(),
                FormTitle = "Example Form",
                Guid = Guid.NewGuid(),
                Id = 1,
                ModifiedBy = 1,
                ModifiedDate = DateTime.UtcNow,
                ParentEntityNumber = 1,
                ParticipantId = "1234",
                ProjectDeployedId = Guid.NewGuid().ToString(),
                ProjectDeployedVersion = "1",
                ProjectDeployStatus = 1,
                ProjectId = Guid.NewGuid(),
                Status = 1,
                SubjectId = null,
                SummaryPageActivityObjId = null,
                TenantId = Guid.NewGuid(),
                ThisUserId = 1,
                }

            };
        }
    }

    public class FormDataEntryViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new FormDataEntryViewModel
            {
                Activity = new ActivityViewModel(),
                ActivityId = Guid.NewGuid(),
                CreatedBy = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                DateDeactivated = DateTime.UtcNow,
                DeactivatedBy = 1,
                EntityId = Guid.NewGuid(),
                FormDataEntryVariable = null,
                FormId = Guid.NewGuid(),
                FormTitle = "Example Form",
                Guid = Guid.NewGuid(),
                Id = 1,
                ModifiedBy = 1,
                ModifiedDate = DateTime.UtcNow,
                ParentEntityNumber = 1,
                ParticipantId = "1234",
                ProjectDeployedId = Guid.NewGuid().ToString(),
                ProjectDeployedVersion = "1",
                ProjectDeployStatus = 1,
                ProjectId = Guid.NewGuid(),
                Status = 1,
                SubjectId = null,
                SummaryPageActivityObjId = null,
                TenantId = Guid.NewGuid(),
                ThisUserId = 1,
            };
        }
    }
}
