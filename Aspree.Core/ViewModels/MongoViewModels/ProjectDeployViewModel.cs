using Aspree.Core.Validators;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels.MongoViewModels
{
    /// <summary>
    /// 
    /// </summary>
    [BsonIgnoreExtraElements]
    public class ProjectDeployViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId Id { get; set; }
        /// <summary>
        /// ProjectGuid of project
        /// </summary>
        public Guid ProjectGuid { get; set; }
        /// <summary>
        /// ProjectId of project
        /// </summary>
        public int ProjectId { get; set; }
        /// <summary>
        /// ProjectName of project
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// ProjectVersion of project
        /// </summary>
        public string ProjectVersion { get; set; }
        /// <summary>
        /// ProjectInternalVersion of project
        /// </summary>
        public int ProjectInternalVersion { get; set; }
        /// <summary>
        /// ProjectState of project
        /// </summary>
        public string ProjectState { get; set; }
        /// <summary>
        /// ProjectCreatedDate of project
        /// </summary>
        public DateTime ProjectCreatedDate { get; set; }
        /// <summary>
        /// ProjectCreatedBy
        /// </summary>
        public string ProjectCreatedBy { get; set; }
        /// <summary>
        /// ProjectDeployDate of project
        /// </summary>
        public DateTime ProjectDeployDate { get; set; }
        /// <summary>
        /// ProjectActivitiesList of project
        /// </summary>
        public List<ActivitiesMongo> ProjectActivitiesList { get; set; }
        /// <summary>
        /// ProjectStaffListMongo of project
        /// </summary>
        public List<ProjectStaffMongo> ProjectStaffListMongo { get; set; }

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

        /// <summary>
        /// Does this project have ethics approval
        /// </summary>
        public bool? ProjectEthicsApproval { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ProjectStaffMongo
    {
        /// <summary>
        /// Guid of Staff
        /// </summary>
        public Guid StaffGuid { get; set; }
        /// <summary>
        /// Name of Staff
        /// </summary>
        public string StaffName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// is active project user
        /// </summary>
        public bool? IsActiveProjectUser { get; set; }
        /// <summary>
        /// Project Joined Date
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ProjectJoinedDate { get; set; }
        /// <summary>
        /// Project Left Date
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ProjectLeftDate { get; set; }

    }
    /// <summary>
    /// 
    /// </summary>
    public class ActivitiesMongo
    {
        /// <summary>
        /// ActivityId
        /// </summary>
        public int ActivityId { get; set; }
        /// <summary>
        /// Guid of Ativity
        /// </summary>
        public Guid ActivityGuid { get; set; }
        /// <summary>
        /// Name of Activity
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// Category Name of Activity
        /// </summary>
        public string ActivityCategoryName { get; set; }
        /// <summary>
        ///  Name of ActivityEntityType
        /// </summary>
        public string ActivityEntityTypeName { get; set; }
        /// <summary>
        /// ScheduledToBeCompleted
        /// </summary>
        public string ScheduledToBeCompleted { get; set; }
        /// <summary>
        /// Name of OtherActivity
        /// </summary>
        public string OtherActivityName { get; set; }
        /// <summary>
        /// OffsetCount
        /// </summary>
        public int? OffsetCount { get; set; }
        /// <summary>
        /// OffsetTypeName
        /// </summary>
        public string OffsetTypeName { get; set; }
        /// <summary>
        /// ActivityAvailableForCreation
        /// </summary>
        public string ActivityAvailableForCreation { get; set; }
        /// <summary>
        /// SpecifiedActivityName
        /// </summary>
        public string SpecifiedActivityName { get; set; }
        /// <summary>
        /// CreationWindowOpens
        /// </summary>
        public int? CreationWindowOpens { get; set; }
        /// <summary>
        /// CreationWindowClose
        /// </summary>
        public int? CreationWindowClose { get; set; }
        /// <summary>
        /// List of RolesToCreateActivity
        /// </summary>
        public List<string> RolesToCreateActivity { get; set; }
        /// <summary>
        /// List of RoleToCreateActivityRegardlessScheduled
        /// </summary>
        public List<string> RoleToCreateActivityRegardlessScheduled { get; set; }
        /// <summary>
        /// ScheduleDate
        /// </summary>
        public DateTime? ScheduleDate { get; set; }
        /// <summary>
        /// List of FormsListMongo
        /// </summary>
        public List<FormsMongo> FormsListMongo { get; set; }
        /// <summary>
        /// IsDefaultActivity true/false
        /// </summary>
        public int IsDefaultActivity { get; set; }
        /// <summary>
        /// CanCreatedMultipleTime true/false
        /// </summary>
        public bool CanCreatedMultipleTime { get; set; }


        /// <summary>
        /// GUID of activity deactivator
        /// </summary>
        public Nullable<Guid> DeactivatedBy { get; set; }
        /// <summary>
        /// Date of activity deactivation
        /// </summary>
        public Nullable<DateTime> DateDeactivated { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class FormsMongo
    {
        /// <summary>
        /// Id of form
        /// </summary>
        public int FormId { get; set; }
        /// <summary>
        /// Name of form
        /// </summary>
        public string FormTitle { get; set; }
        /// <summary>
        /// Category name of form
        /// </summary>
        public string FormCategoryName { get; set; }
        /// <summary>
        /// version of form
        /// </summary>
        public string FormVersion { get; set; }
        /// <summary>
        /// List of entity type name
        /// </summary>
        public List<string> FormEntityTypes { get; set; }
        /// <summary>
        /// List of variable model
        /// </summary>
        public List<VariablesMongo> VariablesListMongo { get; set; }
        /// <summary>
        /// Guid of form
        /// </summary>
        public Guid FormGuid { get; set; }
        /// <summary>
        /// form type (default/custom)
        /// </summary>
        public int IsDefaultForm { get; set; }
        /// <summary>
        /// dataentry id of form
        /// </summary>
        public string FormDataEntryId { get; set; }
        /// <summary>
        /// Count of form variables
        /// </summary>
        public int TotalFormVariableCount { get; set; }


        /// <summary>
        /// name of entity form Modifier
        /// </summary>
        public string ModifiedBy { get; set; }
        /// <summary>
        /// Date of entity form Modified
        /// </summary>
        public string ModifiedDate { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class VariablesMongo
    {
        /// <summary>
        /// id of variable
        /// </summary>
        public int VariableId { get; set; }
        /// <summary>
        /// guid of variable
        /// </summary>
        public Guid VariableGuid { get; set; }
        /// <summary>
        /// name of variable
        /// </summary>
        public string VariableName { get; set; }
        /// <summary>
        /// name of variable type
        /// </summary>
        public string VariableTypeName { get; set; }
        /// <summary>
        /// question text of variable
        /// </summary>
        public string Question { get; set; }
        /// <summary>
        /// help text for variable
        /// </summary>
        public string HelpText { get; set; }
        /// <summary>
        /// required message for variable
        /// </summary>
        public string VariableRequiredMessage { get; set; }
        /// <summary>
        /// variable options value list
        /// </summary>
        public List<string> Values { get; set; }
        /// <summary>
        /// variable options description list
        /// </summary>
        public List<string> ValueDescription { get; set; }
        /// <summary>
        /// minimum input range for variable
        /// </summary>
        public double? MinRange { get; set; }
        /// <summary>
        /// maximum input range for variable
        /// </summary>
        public double? MaxRange { get; set; }
        /// <summary>
        /// dependent variable id of variable
        /// </summary>
        public int? DependentVariableId { get; set; }
        /// <summary>
        /// dependent variable name of variable
        /// </summary>
        public string DependentVariableName { get; set; }
        /// <summary>
        /// response option for dependent variable
        /// </summary>
        public string ResponseOption { get; set; }
        /// <summary>
        /// name of lookup entity type
        /// </summary>
        public string LookupEntityTypeName { get; set; }
        /// <summary>
        /// name of lookup entity sub type
        /// </summary>
        public List<string> LookupEntitySubtypeName { get; set; }
        /// <summary>
        /// variable type id (default/custom)
        /// </summary>
        public int IsDefaultVariable { get; set; }
        /// <summary>
        /// is this variable is required or not
        /// </summary>
        public bool IsRequired { get; set; }
        /// <summary>
        /// list of variable roles
        /// </summary>
        public List<FormVariableRoleMongo> VariableRoleListMongo { get; set; }
        /// <summary>
        /// list of validation rule for variable
        /// </summary>
        public List<VariableValidationRuleMongo> VariableValidationRuleListMongo { get; set; }
        /// <summary>
        /// is this variable visible on "search" page (true/false)
        /// </summary>
        public bool? IsSearchVisible { get; set; }
        /// <summary>
        /// variable order number at search page
        /// </summary>
        public int? SearchPageOrder { get; set; }
        /// <summary>
        /// values entered while data entry
        /// </summary>
        public string SelectedValues { get; set; }
        /// <summary>
        /// guid of userlogin
        /// </summary>
        public Guid? UserLoginGuid { get; set; }
        /// <summary>
        /// can future date
        /// </summary>
        public bool? CanFutureDate { get; set; }
        public int? VariableOrderNo { get; set; }
        public int FormPageVariableId { get; set; }

        public  Tuple<List<string>, List<string>, List<string>> LinkedProjectListWithGroup { get; set; }

        public List<LinkedProjectGroupViewModel> LinkedProjectListWithGroupList { get; set; }
        public string FileName { get; set; }
        public bool? IsBlank { get; set; }
    }
    public class FormVariableRoleMongo
    {
        /// <summary>
        /// name of role
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// permission to create variable
        /// </summary>
        public bool CanCreate { get; set; }
        /// <summary>
        /// permission to view variable
        /// </summary>
        public bool CanView { get; set; }
        /// <summary>
        /// permission to edit variable
        /// </summary>
        public bool CanEdit { get; set; }
        /// <summary>
        /// permission to delete variable
        /// </summary>
        public bool CanDelete { get; set; }
    }
    public class VariableValidationRuleMongo
    {
        /// <summary>
        /// validation message for variable
        /// </summary>
        public string ValidationMessage { get; set; }
        /// <summary>
        /// reguler expression for variable validation
        /// </summary>
        public string RegEx { get; set; }
        /// <summary>
        /// minimum range/value for variable
        /// </summary>
        public double? Min { get; set; }
        /// <summary>
        /// maximum range/value for variable
        /// </summary>
        public double? Max { get; set; }
        /// <summary>
        /// limit type of variable validation
        /// </summary>
        public string LimitType { get; set; }
        /// <summary>
        /// name of validation type
        /// </summary>
        public string ValidationName { get; set; }
    }

    public class FormDataEntriesMongo
    {
        public int EntityId { get; set; }
        public int VariableId { get; set; }
        public string VariableName { get; set; }
        public string VariableSelectedValues { get; set; }
    }
    public class FormsMongoExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new FormsMongo
            {
                FormId = 1,
                FormGuid = Guid.NewGuid(),
                FormTitle = "Example Form",
                FormVersion = "1",
                IsDefaultForm = (int)Core.Enum.DefaultFormType.Custom,
                FormCategoryName = Core.Enum.FormCategories.System.ToString(),
                FormDataEntryId = ObjectId.GenerateNewId().ToString(),
                FormEntityTypes = new List<string> { "Person", "Participant" },
                VariablesListMongo = new List<VariablesMongo>()
                 {
                     new VariablesMongo()
                     {
                         VariableId = 1,
                         VariableGuid = Guid.NewGuid(),
                         VariableName = "Example Variable",
                         VariableTypeName = "Text Box",
                         Question= "Example Question Variable",
                         HelpText = "Example Helptext",
                         DependentVariableId = 2,
                         DependentVariableName = "Example ParentVariable",
                         IsDefaultVariable= (int)Core.Enum.DefaultVariableType.Custom,
                         IsRequired= true,
                         IsSearchVisible = false,
                         SearchPageOrder = null,
                         LookupEntityTypeName = null,
                         LookupEntitySubtypeName =null,
                         MinRange = 20,
                         MaxRange = 100,
                         ResponseOption = "23",
                         Values = new List<string>(),
                         ValueDescription=new List<string>(),
                         SelectedValues="Example Selected Value",
                         UserLoginGuid=Guid.NewGuid(),
                         VariableRequiredMessage ="Example Variable is required",
                         VariableRoleListMongo=new List<FormVariableRoleMongo>()
                         {
                             new FormVariableRoleMongo()
                             {
                                 RoleName = "Data Entry Supervisor",
                                 CanCreate = true,
                                 CanView =true,
                                 CanEdit = false,
                                 CanDelete = false,
                             }
                         },
                         VariableValidationRuleListMongo=new List<VariableValidationRuleMongo>()
                         {
                             new VariableValidationRuleMongo()
                             {
                                 ValidationName = "Numeric",
                                 LimitType = "Range",
                                 Max = 100,
                                 Min = 20,
                                 RegEx ="^[0-9]*$",
                                 ValidationMessage = "You have entered a number that is outside the data entry range.",
                             }
                         },
                     }
                 }
            };
        }
    }

    public class GetAllProjectDeployViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<ProjectDeployViewModel>()
            {
                new ProjectDeployViewModel
                {
                    ProjectGuid=Guid.NewGuid(),
                    ProjectId  = 1,
                    ProjectName = "Example Project",
                    ProjectVersion ="Version",
                    ProjectInternalVersion=1,
                    ProjectState   ="Text Box" ,
                    ProjectCreatedDate =DateTime.UtcNow,
                    ProjectCreatedBy="Text Box",
                    ProjectDeployDate=DateTime.UtcNow,
                    ProjectActivitiesList=new List<ActivitiesMongo>()
                    {
                        new ActivitiesMongo
                        {
                            ActivityId=1,
                            ActivityGuid = Guid.NewGuid(),
                            ActivityName = "Example Activity",
                            ActivityCategoryName="Example Activity Category Name",
                            ActivityEntityTypeName="Example Activity Category Name",
                            ScheduledToBeCompleted="Text Box",
                            OtherActivityName="Other activity name",
                            OffsetCount=1,
                            OffsetTypeName="",
                            ActivityAvailableForCreation="Text Box",
                            SpecifiedActivityName="",
                            CreationWindowOpens=1,
                            CreationWindowClose=1,
                            RolesToCreateActivity=new List<string>(),
                            RoleToCreateActivityRegardlessScheduled=new List<string>(),
                            ScheduleDate=DateTime.UtcNow,
                            FormsListMongo = new List<FormsMongo>()
                              {
                                new FormsMongo()
                                {
                                    FormId=1,
                                    FormTitle="Example Title",
                                    FormCategoryName ="",
                                    FormVersion="",
                                    FormEntityTypes=new List<string>(),
                                    VariablesListMongo=new List<VariablesMongo>()
                                    {
                                        new VariablesMongo
                                        {
                                            VariableId=1,
                                            VariableName="Example Variable Name",
                                            VariableTypeName="Text Box",
                                            Question="Example Question Variable",
                                            HelpText="Example Helptext",
                                            VariableRequiredMessage="Example Variable is Required",
                                            Values=new List<string>(),
                                            ValueDescription=new List<string>(),
                                            MinRange=1.01,
                                            MaxRange=9.99,
                                            DependentVariableId=2,
                                            DependentVariableName="Example ParentVariable",
                                            ResponseOption="23",
                                            LookupEntityTypeName=null,
                                            LookupEntitySubtypeName=null,
                                            IsDefaultVariable=(int)Core.Enum.DefaultVariableType.Custom,
                                            IsRequired=true,
                                            VariableRoleListMongo=new List<FormVariableRoleMongo>()
                                            {
                                                new FormVariableRoleMongo()
                                                {
                                                    RoleName="Example Role Name",
                                                    CanCreate=true,
                                                    CanView=true,
                                                    CanEdit=false,
                                                    CanDelete=false

                                                }


                                            },
                                            VariableValidationRuleListMongo=new List<VariableValidationRuleMongo>()
                                            {
                                                new VariableValidationRuleMongo()
                                                {
                                                     ValidationName = "Numeric",
                                                     LimitType = "Range",
                                                     Max = 100,
                                                     Min = 20,
                                                     RegEx ="^[0-9]*$",
                                                     ValidationMessage = "You have entered a number that is outside the data entry range.",
                                               }
                                            },
                                            IsSearchVisible=true,
                                            SearchPageOrder=1,
                                            SelectedValues="Search Text Box",
                                            UserLoginGuid=Guid.NewGuid()



                                        }

                                    },
                                    FormGuid=Guid.NewGuid(),
                                    IsDefaultForm=1,
                                    FormDataEntryId="Example Form Data Entry Id"

                                }
                            },
                            IsDefaultActivity=1,
                            CanCreatedMultipleTime=true

                        }
                 },
                    ProjectStaffListMongo=new List<ProjectStaffMongo>()
                    {
                    new ProjectStaffMongo
                    {
                        StaffGuid=Guid.NewGuid(),
                        StaffName="Example Staff",

                    }

                    }
                }
            };
        }
    }

    public class ProjectDeployViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ProjectDeployViewModel
            {
                ProjectGuid = Guid.NewGuid(),
                ProjectId = 1,
                ProjectName = "Example Project",
                ProjectVersion = "Version",
                ProjectInternalVersion = 1,
                ProjectState = "Text Box",
                ProjectCreatedDate = DateTime.UtcNow,
                ProjectCreatedBy = "Text Box",
                ProjectDeployDate = DateTime.UtcNow,
                ProjectActivitiesList = new List<ActivitiesMongo>()
                    {
                        new ActivitiesMongo
                        {
                            ActivityId=1,
                            ActivityGuid = Guid.NewGuid(),
                            ActivityName = "Example Activity",
                            ActivityCategoryName="Example Activity Category Name",
                            ActivityEntityTypeName="Example Activity Category Name",
                            ScheduledToBeCompleted="Text Box",
                            OtherActivityName="Other activity name",
                            OffsetCount=1,
                            OffsetTypeName="",
                            ActivityAvailableForCreation="Text Box",
                            SpecifiedActivityName="",
                            CreationWindowOpens=1,
                            CreationWindowClose=1,
                            RolesToCreateActivity=new List<string>(),
                            RoleToCreateActivityRegardlessScheduled=new List<string>(),
                            ScheduleDate=DateTime.UtcNow,
                            FormsListMongo = new List<FormsMongo>()
                              {
                                new FormsMongo()
                                {
                                    FormId=1,
                                    FormTitle="Example Title",
                                    FormCategoryName ="",
                                    FormVersion="",
                                    FormEntityTypes=new List<string>(),
                                    VariablesListMongo=new List<VariablesMongo>()
                                    {
                                        new VariablesMongo
                                        {
                                            VariableId=1,
                                            VariableName="Example Variable Name",
                                            VariableTypeName="Text Box",
                                            Question="Example Question Variable",
                                            HelpText="Example Helptext",
                                            VariableRequiredMessage="Example Variable is Required",
                                            Values=new List<string>(),
                                            ValueDescription=new List<string>(),
                                            MinRange=1.01,
                                            MaxRange=9.99,
                                            DependentVariableId=2,
                                            DependentVariableName="Example ParentVariable",
                                            ResponseOption="23",
                                            LookupEntityTypeName=null,
                                            LookupEntitySubtypeName=null,
                                            IsDefaultVariable=(int)Core.Enum.DefaultVariableType.Custom,
                                            IsRequired=true,
                                            VariableRoleListMongo=new List<FormVariableRoleMongo>()
                                            {
                                                new FormVariableRoleMongo()
                                                {
                                                    RoleName="Example Role Name",
                                                    CanCreate=true,
                                                    CanView=true,
                                                    CanEdit=false,
                                                    CanDelete=false

                                                }


                                            },
                                            VariableValidationRuleListMongo=new List<VariableValidationRuleMongo>()
                                            {
                                                new VariableValidationRuleMongo()
                                                {
                                                     ValidationName = "Numeric",
                                                     LimitType = "Range",
                                                     Max = 100,
                                                     Min = 20,
                                                     RegEx ="^[0-9]*$",
                                                     ValidationMessage = "You have entered a number that is outside the data entry range.",
                                               }
                                            },
                                            IsSearchVisible=true,
                                            SearchPageOrder=1,
                                            SelectedValues="Search Text Box",
                                            UserLoginGuid=Guid.NewGuid()



                                        }

                                    },
                                    FormGuid=Guid.NewGuid(),
                                    IsDefaultForm=1,
                                    FormDataEntryId="Example Form Data Entry Id"

                                }
                            },
                            IsDefaultActivity=1,
                            CanCreatedMultipleTime=true

                        }
                 },
                ProjectStaffListMongo = new List<ProjectStaffMongo>()
                    {
                    new ProjectStaffMongo
                    {
                        StaffGuid=Guid.NewGuid(),
                        StaffName="Example Staff",

                    }

                    }
            };

        }
    }

    public class LinkedProjectGroupViewModel
    {
        public string GroupName { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
    }
}
