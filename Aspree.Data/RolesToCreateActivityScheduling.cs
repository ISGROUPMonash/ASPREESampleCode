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
    
    public partial class RolesToCreateActivityScheduling
    {
        public int Id { get; set; }
        public int ActivitySchedulingId { get; set; }
        public int RoleId { get; set; }
        public System.Guid Guid { get; set; }
    
        public virtual ActivityScheduling ActivityScheduling { get; set; }
        public virtual Role Role { get; set; }
    }
}
