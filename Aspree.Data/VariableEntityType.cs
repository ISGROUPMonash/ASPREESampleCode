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
    
    public partial class VariableEntityType
    {
        public int Id { get; set; }
        public int VariableId { get; set; }
        public int EntityTypeId { get; set; }
        public Nullable<int> EntitySubTypeId { get; set; }
        public System.Guid Guid { get; set; }
    
        public virtual EntitySubType EntitySubType { get; set; }
        public virtual EntityType EntityType { get; set; }
        public virtual Variable Variable { get; set; }
    }
}
