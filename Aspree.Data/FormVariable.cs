//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Aspree.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class FormVariable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FormVariable()
        {
            this.FormVariableRoles = new HashSet<FormVariableRole>();
        }
    
        public int Id { get; set; }
        public int FormId { get; set; }
        public int VariableId { get; set; }
        public bool IsRequired { get; set; }
        public string HelpText { get; set; }
        public Nullable<int> MinRange { get; set; }
        public Nullable<int> MaxRange { get; set; }
        public Nullable<int> RegEx { get; set; }
        public int ValidationRuleType { get; set; }
        public System.Guid Guid { get; set; }
        public Nullable<int> DependentVariableId { get; set; }
        public string ResponseOption { get; set; }
        public string ValidationMessage { get; set; }
        public Nullable<int> DeactivatedBy { get; set; }
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        public Nullable<bool> IsSearchVisible { get; set; }
        public Nullable<int> SearchPageOrder { get; set; }
        public string QuestionText { get; set; }
        public Nullable<int> VariableOrderNo { get; set; }
        public Nullable<bool> IsBlank { get; set; }
    
        public virtual Form Form { get; set; }
        public virtual Variable Variable { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormVariableRole> FormVariableRoles { get; set; }
    }
}
