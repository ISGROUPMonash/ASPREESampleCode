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
    
    public partial class Role
    {
        public Role()
        {
            this.VariableRoles = new HashSet<VariableRole>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSystemRole { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> DeactivatedBy { get; set; }
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        public System.Guid Guid { get; set; }
        public int Status { get; set; }
        public Nullable<int> TenantId { get; set; }
    
        public virtual ICollection<VariableRole> VariableRoles { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}
