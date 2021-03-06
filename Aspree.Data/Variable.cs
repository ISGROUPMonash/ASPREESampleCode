//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Aspree.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Variable
    {
        public Variable()
        {
            this.VariableRoles = new HashSet<VariableRole>();
        }
    
        public int Id { get; set; }
        public string VariableName { get; set; }
        public string VariableLabel { get; set; }
        public string Question { get; set; }
        public string Values { get; set; }
        public string ValueDescription { get; set; }
        public string HelpText { get; set; }
        public int VariableTypeId { get; set; }
        public string ValidationMessage { get; set; }
        public string RequiredMessage { get; set; }
        public Nullable<double> MinRange { get; set; }
        public Nullable<double> MaxRange { get; set; }
        public string RegEx { get; set; }
        public Nullable<bool> IsSoftRange { get; set; }
        public Nullable<int> ValidationRuleId { get; set; }
        public Nullable<int> DependentVariableId { get; set; }
        public bool IsRequired { get; set; }
        public bool CanCollectMultiple { get; set; }
        public Nullable<int> VariableCategoryId { get; set; }
        public bool IsApproved { get; set; }
        public string Comment { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> DeactivatedBy { get; set; }
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        public System.Guid Guid { get; set; }
        public Nullable<int> TenantId { get; set; }
        public string VariableValueDescription { get; set; }
        public int IsDefaultVariable { get; set; }
        public Nullable<bool> CanFutureDate { get; set; }
        public string VariableDetails { get; set; }
    
        public virtual ICollection<VariableRole> VariableRoles { get; set; }
        public virtual VariableCategory VariableCategory { get; set; }
        public virtual VariableType VariableType { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}
