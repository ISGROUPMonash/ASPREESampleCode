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
    
    public partial class LoginAuthTypeMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LoginAuthTypeMaster()
        {
            this.UserLogins = new HashSet<UserLogin>();
        }
    
        public int Id { get; set; }
        public string UserName { get; set; }
        public string AuthTypeName { get; set; }
        public int AuthType { get; set; }
        public string Domain { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string State { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> DeactivatedBy { get; set; }
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        public System.Guid Guid { get; set; }
        public string AuthenticationProviderClaim { get; set; }
        public string AuthorizeEndpoint { get; set; }
        public string TokenEndpoint { get; set; }
        public string IntrospectEndpoint { get; set; }
        public string RevokeEndpoint { get; set; }
        public string LogoutEndpoint { get; set; }
        public string KeysEndpoint { get; set; }
        public string UserinfoEndpoint { get; set; }
        public int Status { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserLogin> UserLogins { get; set; }
    }
}
