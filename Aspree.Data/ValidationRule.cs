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
    
    public partial class ValidationRule
    {
        public int Id { get; set; }
        public string RuleType { get; set; }
        public Nullable<double> MinRange { get; set; }
        public Nullable<double> MaxRange { get; set; }
        public Nullable<int> RegExId { get; set; }
        public string ErrorMessage { get; set; }
        public System.Guid Guid { get; set; }
    }
}
