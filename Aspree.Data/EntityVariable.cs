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
    
    public partial class EntityVariable
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public int VariableId { get; set; }
        public System.Guid Guid { get; set; }
    
        public virtual Entity Entity { get; set; }
        public virtual Variable Variable { get; set; }
    }
}
