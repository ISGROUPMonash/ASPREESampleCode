using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class VariableViewModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Name of Variable
        /// </summary>
        public string VariableName { get; set; }
        /// <summary>
        /// Label of Variable
        /// </summary>
        public string VariableLabel { get; set; }
        /// <summary>
        /// Question
        /// </summary>
        public string Question { get; set; }
        /// <summary>
        /// Values
        /// </summary>
        public List<string> Values { get; set; }
        /// <summary>
        /// Description of Values
        /// </summary>
        public string ValueDescription { get; set; }
        /// <summary>
        /// HelpText
        /// </summary>
        public string HelpText { get; set; }
        /// <summary>
        /// VariableTypeId of Variable
        /// </summary>
        public Guid VariableTypeId { get; set; }
        /// <summary>
        /// Validation Message
        /// </summary>
        public string ValidationMessage { get; set; }
        /// <summary>
        /// Required Message
        /// </summary>
        public string RequiredMessage { get; set; }
        /// <summary>
        /// MinRange of Variable
        /// </summary>
        public Nullable<double> MinRange { get; set; }
        /// <summary>
        /// Max range of Variable
        /// </summary>
        public Nullable<double> MaxRange { get; set; }
        /// <summary>
        /// RegEx of variable
        /// </summary>
        public string RegEx { get; set; }
        /// <summary>
        /// IsSoftRange
        /// </summary>
        public Nullable<bool> IsSoftRange { get; set; }
        /// <summary>
        /// Guid of ValidationRule
        /// </summary>
        public Nullable<Guid> ValidationRuleId { get; set; }
        /// <summary>
        /// Guid of  DependentVariable
        /// </summary>
        public Nullable<Guid> DependentVariableId { get; set; }
        /// <summary>
        /// IsRequired
        /// </summary>
        public bool IsRequired { get; set; }
        /// <summary>
        /// CanCollectMultiple
        /// </summary>
        public bool CanCollectMultiple { get; set; }
        /// <summary>
        /// Category Id of Variable
        /// </summary>
        public Nullable<Guid> VariableCategoryId { get; set; }
        /// <summary>
        /// IsApproved
        /// </summary>
        public bool IsApproved { get; set; }


        /// <summary>
        ///  UserTypeRole
        /// </summary>
        public string UserTypeRole { get; set; }



        /// <summary>
        /// Comment
        /// </summary>
        public string Comment { get; set; }
        /// <summary>
        /// Variable CreatedBy
        /// </summary>
        public Guid CreatedBy { get; set; }
        /// <summary>
        /// Created date of variable
        /// </summary>
        public System.DateTime CreatedDate { get; set; }
        /// <summary>
        /// variable ModifiedBy
        /// </summary>
        public Nullable<Guid> ModifiedBy { get; set; }
        /// <summary>
        /// ModifiedDate of variable
        /// </summary>
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        /// <summary>
        /// Variable DeactivatedBy
        /// </summary>
        public Nullable<Guid> DeactivatedBy { get; set; }
        /// <summary>
        /// Deactivated date of  variable
        /// </summary>
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        /// <summary>
        /// Guid
        /// </summary>
        public System.Guid Guid { get; set; }
        /// <summary>
        /// Roles of Variable
        /// </summary>
        public List<Guid> VariableRoles { get; set; }
        /// <summary>
        /// Guid of Tenat
        /// </summary>
        public Guid TenantId { get; set; }
        /// <summary>
        /// Name of Variable type
        /// </summary>
        public string VariableTypeName { get; set; }
        /// <summary>
        /// List of Guid of ValidationRule
        /// </summary>
        public List<Guid> ValidationRuleIds { get; set; }
        /// <summary>
        /// List of Validation Rule
        /// </summary>
        public List<ValidationRuleViewModel> validationRuleViewModel { get; set; }
        /// <summary>
        /// CustomRegEx of variable
        /// </summary>
        public string CustomRegEx { get; set; }
        public List<VariableValidationRuleViewModel> variableValidationRuleViewModel { get; set; }
        /// <summary>
        /// List of VariableValueDescription
        /// </summary>
        public List<string> VariableValueDescription { get; set; }
        /// <summary>
        /// IsVariableLogTable
        /// </summary>
        public bool IsVariableLogTable { get; set; }
        /// <summary>
        /// IsDefaultVariable
        /// </summary>
        public int IsDefaultVariable { get; set; }
        /// <summary>
        /// Selected value of Variables
        /// </summary>
        public String VariableSelectedValues { get; set; }
        /// <summary>
        /// Guid of FormDataEntry
        /// </summary>
        public Guid? FormDataEntryGuid { get; set; }
        /// <summary>
        /// OutsideRangeValidation of Variable 
        /// </summary>
        public string OutsideRangeValidation { get; set; }
        /// <summary>
        /// MissingValidation of Variable
        /// </summary>
        public string MissingValidation { get; set; }
        /// <summary>
        /// LookupEntityType of Variable
        /// </summary>
        public Guid? LookupEntityType { get; set; }
        /// <summary>
        /// LookupEntitySubtype of Variable
        /// </summary>
        public Guid? LookupEntitySubtype { get; set; }
        /// <summary>
        /// LookupEntityType of Variable
        /// </summary>
        public string LookupEntityTypeName { get; set; }
        /// <summary>
        /// LookupEntitySubtype of variable
        /// </summary>
        public string LookupEntitySubtypeName { get; set; }
        /// <summary>
        /// DateFormatm of variable
        /// </summary>
        public string DateFormat { get; set; }
        /// <summary>
        /// true/false for FutureDate 
        /// </summary>
        public bool? CanFutureDate { get; set; }
        /// <summary>
        /// Guid of UserNameVariable
        /// </summary>
        public Guid? UserNameVariableGuid { get; set; }
        /// <summary>
        /// List of VariableUsedInFormsList
        /// </summary>
        public List<Guid> VariableUsedInFormsList { get; set; }
    }

    public class NewVariableViewModel
    {
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Variable Name")]
        public string VariableName { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Variable Label")]
        public string VariableLabel { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Question")]
        public string Question { get; set; }
        [Required(ErrorMessage = "Required")]
        public List<string> Values { get; set; }
        public string ValueDescription { get; set; }
        public string HelpText { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Variable Type")]
        public Guid VariableTypeId { get; set; }
        public string ValidationMessage { get; set; }
        public string RequiredMessage { get; set; }
        public Nullable<double> MinRange { get; set; }
        public Nullable<double> MaxRange { get; set; }
        public string RegEx { get; set; }
        public Nullable<bool> IsSoftRange { get; set; }
        public Nullable<Guid> ValidationRuleId { get; set; }
        public Nullable<Guid> DependentVariableId { get; set; }
        public bool IsRequired { get; set; }
        public bool CanCollectMultiple { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Variable Category")]
        public Nullable<Guid> VariableCategoryId { get; set; }
        public bool IsApproved { get; set; }

        //[Required(ErrorMessage = "Required")]
        //[Display(Name = "Comment")]
        public string Comment { get; set; }

        [Required(ErrorMessage = "Required")]
        public List<Guid> VariableRoles { get; set; }

        public List<Guid> ValidationRuleIds { get; set; }
        public string CustomRegEx { get; set; }
        public List<string> VariableValueDescription { get; set; }


        public string OutsideRangeValidation { get; set; }
        public string MissingValidation { get; set; }

        public Guid? LookupEntityType { get; set; }
        public Guid? LookupEntitySubtype { get; set; }

        public string DateFormat { get; set; }
        public bool? CanFutureDate { get; set; }
    }

    public class EditVariableViewModel
    {
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Variable Name")]
        public string VariableName { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Variable Label")]
        public string VariableLabel { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Question")]
        public string Question { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Values")]
        public List<string> Values { get; set; }
        public string ValueDescription { get; set; }
        public string HelpText { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Variable Type")]
        public Guid VariableTypeId { get; set; }
        public string ValidationMessage { get; set; }
        public string RequiredMessage { get; set; }
        public Nullable<double> MinRange { get; set; }
        public Nullable<double> MaxRange { get; set; }
        public string RegEx { get; set; }
        public Nullable<bool> IsSoftRange { get; set; }
        public Nullable<Guid> ValidationRuleId { get; set; }
        public Nullable<Guid> DependentVariableId { get; set; }
        public bool IsRequired { get; set; }
        public bool CanCollectMultiple { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Variable Category")]
        public Nullable<Guid> VariableCategoryId { get; set; }
        public bool IsApproved { get; set; }
        public System.Guid Guid { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Variable Role")]
        public List<Guid> VariableRoles { get; set; }
        public string Comment { get; set; }

        public List<Guid> ValidationRuleIds { get; set; }
        public string CustomRegEx { get; set; }
        public List<string> VariableValueDescription { get; set; }
        public bool IsVariableLogTable { get; set; }

        public string OutsideRangeValidation { get; set; }
        public string MissingValidation { get; set; }

        public Guid? LookupEntityType { get; set; }
        public Guid? LookupEntitySubtype { get; set; }

        public string DateFormat { get; set; }
        public bool? CanFutureDate { get; set; }
    }

}
