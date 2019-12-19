using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class DeployProjectJsonViewModel
    {
        public Guid ProjectGuid { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int ProjectVersion { get; set; }
        public string State { get; set; }
        public List<JsonSchedulingViewModel> JsonSchedulingViewModelList { get; set; }
        public List<AddSummaryPageActivityViewModel> AddSummaryPageActivityViewModelList { get; set; }
        public List<JsonProjectStaffMembers> projectStaffMemberList { get; set; }
        public DeployProjectJsonViewModel()
        {
            projectStaffMemberList = new List<JsonProjectStaffMembers>();
            JsonSchedulingViewModelList = new List<JsonSchedulingViewModel>();
            AddSummaryPageActivityViewModelList = new List<AddSummaryPageActivityViewModel>();
        }
    }
    public class JsonSchedulingViewModel
    {
        public int schedulingId { get; set; }
        public System.Guid SchedulingGuid { get; set; }
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public int ScheduledToBeCompleted { get; set; }
        public int ActivityAvailableForCreation { get; set; }
        public List<Guid> RolesToCreateActivity { get; set; }
        public List<Guid> RoleToCreateActivityRegardlessScheduled { get; set; }
        public Nullable<Guid> OtherActivity { get; set; }
        public Nullable<int> OffsetCount { get; set; }
        public Nullable<int> OffsetType { get; set; }
        public Nullable<Guid> SpecifiedActivity { get; set; }
        public Nullable<int> CreationWindowOpens { get; set; }
        public Nullable<int> CreationWindowClose { get; set; }
        public Nullable<bool> IsScheduled { get; set; }
        public string ScheduleDate { get; set; }
        public int? Status { get; set; }
        public Guid ProjectId { get; set; }
        public JsonActivityViewModel JsonActivityViewModelData { get; set; }
    }
    public class JsonActivityViewModel
    {
        public int ActivityId { get; set; }
        public System.Guid ActivityGuid { get; set; }
        public string ActivityName { get; set; }
        //public Nullable<int> RepeatationType { get; set; }
        //public Nullable<int> RepeatationCount { get; set; }
        //public Nullable<int> RepeatationOffset { get; set; }
        //public Nullable<Guid> DependentActivityId { get; set; }
        //public Nullable<System.DateTime> StartDate { get; set; }
        //public Nullable<System.DateTime> EndDate { get; set; }
        public int ActivityCategoryId { get; set; }
        public Guid ActivityCategoryGuid { get; set; }
        public string ActivityCategoryName { get; set; }
        public int ScheduleType { get; set; }
        public Guid ActivityStatusId { get; set; }
        public Guid CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedDateString { get; set; }
        public System.Guid ProjectId { get; set; }
        public Nullable<Guid> TenantId { get; set; }
        public string EntityTypeName { get; set; }
        public int EntityTypeId { get; set; }
        public Guid EntityTypeGuid { get; set; }
        public bool PreviewSchedulingDone { get; set; }
        public int IsDefaultActivity { get; set; }
        public bool? IsActivityRequireAnEntity { get; set; }

        public List<FormViewModel> JsonFormViewModelList { get; set; }

    }
    //public class JsonFormViewModel
    //{
    //    public int FormId { get; set; }
    //    public Guid FormGuid { get; set; }
    //    public string FormTitle { get; set; }
    //    public Guid FormCategoryGuid { get; set; }
    //    public string FormCategoryName { get; set; }
    //    public int FormCategoryId { get; set; }
    //    public Nullable<Guid> ProjectGuid { get; set; }
    //    public bool IsPublished { get; set; }
    //    public Nullable<int> ApprovedBy { get; set; }
    //    public System.DateTime? ApprovedDate { get; set; }
    //    public string ApprovedDateString { get; set; }
    //    public bool IsTemplate { get; set; }
    //    public string FormStatusName { get; set; }
    //    public int FormStatusId { get; set; }
    //    public Nullable<int> Version { get; set; }
    //    public Nullable<int> PreviousVersion { get; set; }
    //    public Guid CreatedBy { get; set; }
    //    public string CreatedDateString { get; set; }
    //    public System.DateTime CreatedDate { get; set; }
    //    public int IsDefaultForm { get; set; }

    //    public List<EntityTypeViewModel> EntityTypeViewModelList { get; set; }

    //    public List<JsonVariableViewModel> JsonVariableViewModelList { get; set; }

    //    public Guid? FormDataEntryGuid { get; set; }

    //}
    //public class JsonVariableViewModel
    //{
    //    //variable
    //    public int VariableId { get; set; }
    //    public System.Guid VariableGuid { get; set; }
    //    public string VariableName { get; set; }
    //    public string VariableLabel { get; set; }
    //    public string Question { get; set; }
    //    public string HelpText { get; set; }
    //    public List<string> Values { get; set; }
    //    public List<string> VariableValueDescription { get; set; }
    //    public int IsDefaultVariable { get; set; }

    //    //variable type
    //    public int VariableTypeId { get; set; }
    //    public Guid VariableTypeGuid { get; set; }
    //    public string VariableTypeName { get; set; }

    //    //variable category
    //    public int VariableCategoryId { get; set; }
    //    public Guid VariableCategoryGuid { get; set; }
    //    public string VariableCategoryName { get; set; }

    //    //validation's
    //    public string ValidationMessage { get; set; }
    //    public string RequiredMessage { get; set; }
    //    public Nullable<double> MinRange { get; set; }
    //    public Nullable<double> MaxRange { get; set; }
    //    public string RegEx { get; set; }
    //    public bool IsRequired { get; set; }
    //    public string DateFormat { get; set; }
    //    public bool? CanFutureDate { get; set; }
    //    public Guid? UserNameVariableGuid { get; set; }
    //    public List<FormVariableRoleViewModel> formVariableRoleViewModelList { get; set; }

    //    //variable approval
    //    public bool IsApproved { get; set; }
    //    public string Comment { get; set; }

    //    //creation's
    //    public Guid CreatedBy { get; set; }
    //    public System.DateTime CreatedDate { get; set; }
    //    public string CreatedDateString { get; set; }

    //    //lookup variables
    //    public Guid? LookupEntityType { get; set; }
    //    public Guid? LookupEntitySubtype { get; set; }
    //    public string LookupEntityTypeName { get; set; }
    //    public string LookupEntitySubtypeName { get; set; }

    //    //variable data-entry
    //    public String VariableSelectedValues { get; set; }
    //    public Guid? FormDataEntryGuid { get; set; }


    //    //
    //    public Nullable<Guid> DependentVariableId { get; set; }
    //    public string ResponseOption { get; set; }
    //}





    public class JsonFormDataEntries
    {
        public int FormDataEntryId { get; set; }
        public System.Guid FormDataEntryGuid { get; set; }
        public int ActivityId { get; set; }
        public System.Guid ActivityGuid { get; set; }
        public string ActivityName { get; set; }
        public int ProjectId { get; set; }
        public System.Guid ProjectGuid { get; set; }
        public string ProjectName { get; set; }
        public int EntityId { get; set; }
        public System.Guid EntityGuid { get; set; }
        public string EntityName { get; set; }
        public int Status { get; set; }
        public int CreatedById { get; set; }
        public System.Guid CreatedByGuid { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedDateString { get; set; }
        public int? FormId { get; set; }
        public System.Guid FormGuid { get; set; }
        public string FormTitle { get; set; }
        public int? ThisUserId { get; set; }
        public List<JsonFormDataEntryVariables> JsonFormDataEntryVariableList { get; set; }
    }
    public class JsonFormDataEntryVariables
    {
        public int FormDataEntryTableId { get; set; }
        public int FormDataEntryVariableId { get; set; }
        public int VariableId { get; set; }
        public string VariableName { get; set; }
        public System.Guid VariableGuid { get; set; }
        public string SelectedValue { get; set; }
        public int? ParentId { get; set; }
    }

    public class JsonProjectStaffMembers
    {
        public int ProjectUserId { get; set; }
        public System. Guid ProjectUserGuid { get; set; }
        public String ProjectUserName { get; set; }
        public int ProjectUserRoleId { get; set; }
        public System.Guid ProjectUserRoleGuid { get; set; }
        public String ProjectUserRoleName { get; set; }

        public int ProjectId { get; set; }
        public System.Guid ProjectGuid { get; set; }
        public String ProjectName { get; set; }
    }
}
