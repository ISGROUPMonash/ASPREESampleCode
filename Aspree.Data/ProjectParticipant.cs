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
    
    public partial class ProjectParticipant
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int EntityId { get; set; }
        public System.Guid Guid { get; set; }
    
        public virtual Project Project { get; set; }
    }
}
