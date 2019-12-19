using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class FormViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }

        /// <summary>
        ///  Title of Form
        /// </summary>
        public string FormTitle { get; set; }

        /// <summary>
        /// IsPublished true/false
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// Form ApprovedBy
        /// </summary>
        public Nullable<int> ApprovedBy { get; set; }

        /// <summary>
        /// ApprovedDate of Form
        /// </summary>
        public Nullable<System.DateTime> ApprovedDate { get; set; }

        /// <summary>
        /// IsTemplate true/false
        /// </summary>
        public bool IsTemplate { get; set; }

        /// <summary>
        /// Category Id of form
        /// </summary>
        public Nullable<Guid> FormCategoryId { get; set; }

        /// <summary>
        /// Guid of Project
        /// </summary>
        public Nullable<Guid> ProjectId { get; set; }

        /// <summary>
        /// state of Form
        /// </summary>
        public int FormState { get; set; }

        /// <summary>
        /// Guid of Form status
        /// </summary>
        public Guid FormStatusId { get; set; }

        /// <summary>
        ///  Version of Form
        /// </summary>
        public Nullable<int> Version { get; set; }

        /// <summary>
        /// PreviousVersion of Form
        /// </summary>
        public Nullable<int> PreviousVersion { get; set; }

        /// <summary>
        /// Form CreatedBy 
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// CreatedDate of Form
        /// </summary>
        public System.DateTime CreatedDate { get; set; }

        /// <summary>
        /// Form ModifiedBy
        /// </summary>
        public Nullable<Guid> ModifiedBy { get; set; }

        /// <summary>
        /// ModifiedDate of Form
        /// </summary>
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        /// <summary>
        ///  Form DeactivatedBy
        /// </summary>
        public Nullable<Guid> DeactivatedBy { get; set; }

        /// <summary>
        /// Deactivated Date of Form
        /// </summary>
        public Nullable<System.DateTime> DateDeactivated { get; set; }

        /// <summary>
        /// Guid
        /// </summary>
        public System.Guid Guid { get; set; }

        /// <summary>
        /// Guid of Tenat
        /// </summary>
        public Nullable<Guid> TenantId { get; set; }

        /// <summary>
        /// List of variables
        /// </summary>
        public List<FormVariableViewModel> Variables { get; set; }

        /// <summary>
        /// List of Entity Type
        /// </summary>
        public List<Guid> EntityTypes { get; set; }

        /// <summary>
        /// IsDefaultForm
        /// </summary>
        public int IsDefaultForm { get; set; }

        /// <summary>
        /// Guid of FormDataEntry
        /// </summary>
        public Guid? FormDataEntryGuid { get; set; }
        /// <summary>
        /// FormUsedInActivityList
        /// </summary>
        public List<Guid> FormUsedInActivityList { get; set; }
        /// <summary>
        ///  UsedVariablesNameList
        /// </summary>
        public List<string> UsedVariablesNameList { get; set; }




        /// <summary>
        ///  FormIsInDeployeActiviti
        /// </summary>
        public bool FormIsInDeployeActiviti { get; set; }
        /// <summary>
        ///  IsFormContaindData
        /// </summary>
        public bool IsFormContaindData { get; set; }
        /// <summary>
        ///  UserTypeRole
        /// </summary>
        public string UserTypeRole { get; set; }








        /// <summary>
        ///  IsNewForm
        /// </summary>
        public bool IsNewForm { get; set; }


        /// <summary>
        /// Form ModifiedBy
        /// </summary>
        public string ModifiedByString { get; set; }

        /// <summary>
        /// ModifiedDate of Form
        /// </summary>
        public string ModifiedDateString { get; set; }



    }

    public class GetAllFormViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<FormViewModel>()
            {
                new FormViewModel
                {
                    Id = 1,
                    Guid = Guid.NewGuid(),
                    ApprovedBy=1,
                    ApprovedDate=DateTime.UtcNow,
                    CreatedBy=Guid.NewGuid(),
                    CreatedDate=DateTime.UtcNow,
                    DateDeactivated=DateTime.UtcNow,
                    DeactivatedBy=Guid.NewGuid(),
                    EntityTypes=new List<Guid>(),
                    FormCategoryId=Guid.NewGuid(),
                    FormDataEntryGuid=Guid.NewGuid(),
                    FormState=1,
                    FormStatusId=Guid.NewGuid(),
                    FormTitle="Text",
                    FormUsedInActivityList=new List<Guid>(),
                    IsDefaultForm=1,
                    IsNewForm=true,
                    IsPublished=true,
                    IsTemplate=true,
                    ModifiedBy=Guid.NewGuid(),
                    ModifiedDate=DateTime.UtcNow,
                    PreviousVersion=1,
                    ProjectId=Guid.NewGuid(),
                    TenantId=Guid.NewGuid(),
                    UsedVariablesNameList=new List<string>(),
                    Version=1,
                    Variables=new List<FormVariableViewModel>()
                    {
                        new FormVariableViewModel
                        {
                            DependentVariableId=Guid.NewGuid(),
                            FormVariableIsApprovedStatus=true,
                            FormVariableRoles=new List<Guid>(),
                            formVariableRoleViewModel=new List<FormVariableRoleViewModel>()
                            {
                                new FormVariableRoleViewModel
                                {
                                    CanCreate=true,
                                    CanDelete=true,
                                    CanEdit=true,
                                    CanView=true,
                                    FormVariableId=1,
                                    Guid=Guid.NewGuid(),
                                    RoleGuidId=Guid.NewGuid()
                                }
                            }

                        }
                    },
            }

        };
        }
    }

    public class FormViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new FormViewModel
            {
                Id = 1,
                Guid = Guid.NewGuid(),
                ApprovedBy = 1,
                ApprovedDate = DateTime.UtcNow,
                CreatedBy = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                DateDeactivated = DateTime.UtcNow,
                DeactivatedBy = Guid.NewGuid(),
                EntityTypes = new List<Guid>(),
                FormCategoryId = Guid.NewGuid(),
                FormDataEntryGuid = Guid.NewGuid(),
                FormState = 1,
                FormStatusId = Guid.NewGuid(),
                FormTitle = "Text",
                FormUsedInActivityList = new List<Guid>(),
                IsDefaultForm = 1,
                IsNewForm = true,
                IsPublished = true,
                IsTemplate = true,
                ModifiedBy = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow,
                PreviousVersion = 1,
                ProjectId = Guid.NewGuid(),
                TenantId = Guid.NewGuid(),
                UsedVariablesNameList = new List<string>(),
                Version = 1,
                Variables = new List<FormVariableViewModel>()
                    {
                        new FormVariableViewModel
                        {
                            DependentVariableId=Guid.NewGuid(),
                            FormVariableIsApprovedStatus=true,
                            FormVariableRoles=new List<Guid>(),
                            formVariableRoleViewModel=new List<FormVariableRoleViewModel>()
                            {
                                new FormVariableRoleViewModel
                                {
                                    CanCreate=true,
                                    CanDelete=true,
                                    CanEdit=true,
                                    CanView=true,
                                    FormVariableId=1,
                                    Guid=Guid.NewGuid(),
                                    RoleGuidId=Guid.NewGuid()
                                }
                            }

                        }
                    },
            };

       
        }
    }


    public class NewFormViewModel
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Title of Form
        /// </summary>
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Form Title")]
        public string FormTitle { get; set; }
        /// <summary>
        /// Is Published
        /// </summary>
        public bool IsPublished { get; set; }
        /// <summary>
        /// Form ApprovedBy
        /// </summary>
        public Nullable<int> ApprovedBy { get; set; }
        /// <summary>
        /// Form Approval date
        /// </summary>
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        /// <summary>
        /// IsTemplate
        /// </summary>
        public bool IsTemplate { get; set; }
        /// <summary>
        /// Category Id of Form
        /// </summary>
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Form Category")]
        
        public Nullable<Guid> FormCategoryId { get; set; }
        /// <summary>
        /// Guid of project
        /// </summary>
        public Nullable<Guid> ProjectId { get; set; }
        /// <summary>
        /// State of Form
        /// </summary>
        public int FormState { get; set; }
        /// <summary>
        /// Guid of Form status
        /// </summary>
        public Guid FormStatusId { get; set; }
        /// <summary>
        /// List of variables
        /// </summary>
        public List<FormVariableViewModel> Variables { get; set; }
        /// <summary>
        /// List of Entity Type
        /// </summary>
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Entity Types")]
        public List<Guid> EntityTypes { get; set; }
        //public Nullable<int> Version { get; set; }
        //public Nullable<int> PreviousVersion { get; set; }
    }

    public class EditFormViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Form Title")]
        public string FormTitle { get; set; }
        public bool IsPublished { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public bool IsTemplate { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Form Category")]
        public Nullable<Guid> FormCategoryId { get; set; }
        public Nullable<Guid> ProjectId { get; set; }
        public int FormState { get; set; }
        public Guid FormStatusId { get; set; }
        public System.Guid Guid { get; set; }
        public List<FormVariableViewModel> Variables { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Entity Types")]
        public List<Guid> EntityTypes { get; set; }
        //public Nullable<int> Version { get; set; }
        //public Nullable<int> PreviousVersion { get; set; }
    }

    public class EditFormViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new EditFormViewModel()
            {

                Id = 1,
                Guid = Guid.NewGuid(),
                ApprovedBy = 1,
                ApprovedDate = DateTime.UtcNow,
                EntityTypes = new List<Guid>(),
                FormCategoryId = Guid.NewGuid(),
                FormState = 1,
                FormStatusId = Guid.NewGuid(),
                FormTitle = "Text",
                IsPublished = true,
                IsTemplate = true,
                ProjectId = Guid.NewGuid(),
                Variables = new List<FormVariableViewModel>()
                    {
                        new FormVariableViewModel
                        {
                            DependentVariableId=Guid.NewGuid(),
                            FormVariableIsApprovedStatus=true,
                            FormVariableRoles=new List<Guid>(),
                            formVariableRoleViewModel=new List<FormVariableRoleViewModel>()
                            {
                                new FormVariableRoleViewModel
                                {
                                    CanCreate=true,
                                    CanDelete=true,
                                    CanEdit=true,
                                    CanView=true,
                                    FormVariableId=1,
                                    Guid=Guid.NewGuid(),
                                    RoleGuidId=Guid.NewGuid()
                                }
                            }

                        }
                    }


            };
        }
    }

    public class FormVariableViewModel
    {
        /// <summary>
        /// VariableId of Variable
        /// </summary>
        public Guid VariableId { get; set; }
        /// <summary>
        /// Name of Variable
        /// </summary>
        public string VariableName { get; set; }
        /// <summary>
        /// IsRequired true/false
        /// </summary>
        public bool IsRequired { get; set; }
        /// <summary>
        /// Help Text
        /// </summary>
        public string HelpText { get; set; }
        /// <summary>
        /// MinRange
        /// </summary>
        public int? MinRange { get; set; }
        /// <summary>
        /// MaxRange
        /// </summary>
        public int? MaxRange { get; set; }
        /// <summary>
        /// RegEx
        /// </summary>
        public int? RegEx { get; set; }
        /// <summary>
        /// ValidationRuleType
        /// </summary>
        public int ValidationRuleType { get; set; }
        /// <summary>
        /// guid of DependentVariable
        /// </summary>
        public Nullable<Guid> DependentVariableId { get; set; }
        /// <summary>
        /// ResponseOption
        /// </summary>
        public string ResponseOption { get; set; }
        /// <summary>
        /// ValidationMessage
        /// </summary>
        public string ValidationMessage { get; set; }
        /// <summary>
        /// List of FormVariableRoles
        /// </summary>
        public List<Guid> FormVariableRoles { get; set; }
        /// <summary>
        ///  Approval status of FormVariable
        /// </summary>
        public bool FormVariableIsApprovedStatus { get; set; }
        /// <summary>
        /// Type of Variable
        /// </summary>
        public string VariableType { get; set; }
        /// <summary>
        /// variableViewModel
        /// </summary>
        public VariableViewModel variableViewModel { get; set; }
        /// <summary>
        /// formVariableRoleViewModel
        /// </summary>
        public List<FormVariableRoleViewModel> formVariableRoleViewModel { get; set; }
        /// <summary>
        /// IsSearchVisible true/false
        /// </summary>
        public bool? IsSearchVisible { get; set; }
        /// <summary>
        /// SearchPageOrder
        /// </summary>
        public int? SearchPageOrder { get; set; }
        /// <summary>
        /// IsDefaultVariableType
        /// </summary>
        public int IsDefaultVariableType { get; set; }
        /// <summary>
        /// Text of Question
        /// </summary>
        public string QuestionText { get; set; }

        public bool? IsBlank { get; set; }

        public FormVariableViewModel()
        {
            this.FormVariableRoles = new List<Guid>();
        }
    }

    public class NewFormViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new NewFormViewModel
            {
                ApprovedBy = 1,
                ApprovedDate = DateTime.Now,
                FormCategoryId = Guid.NewGuid(),
                FormState = 1,
                FormStatusId = Guid.NewGuid(),
                FormTitle = "Example form title",
                Id = 1,
                IsPublished = false,
                IsTemplate = true,
                ProjectId = Guid.NewGuid(),
            };
        }
    }
}
