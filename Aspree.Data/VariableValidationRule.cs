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
    
    public partial class VariableValidationRule
    {
        public int Id { get; set; }
        public int VariableId { get; set; }
        public Nullable<int> ValidationId { get; set; }
        public string ValidationMessage { get; set; }
        public string RegEx { get; set; }
        public Nullable<double> Min { get; set; }
        public Nullable<double> Max { get; set; }
        public string LimitType { get; set; }
        public System.Guid Guid { get; set; }
    
        public virtual ValidationRule ValidationRule { get; set; }
        public virtual Variable Variable { get; set; }
    }
}