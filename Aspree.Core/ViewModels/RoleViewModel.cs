using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class RoleViewModel 
    {     
        /// <summary>
        /// Id of Role
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Guid of Role
        /// </summary>
        public Guid Guid { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }     
        /// <summary>
        /// List of Roles
        /// </summary>
        public List<string> Roles { get; set; }
        /// <summary>
        /// Name of Role
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Is system role true/false
        /// </summary>
        public bool IsSystemRole { get; set; }
        /// <summary>
        /// Guid of Tenat
        /// </summary>
        public Guid TenantId { get; set; }
       public RoleViewModel()
        {
            this.Roles = new List<string>();
        }
    }

    public class RoleModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Name of Role
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Is system role true/false
        /// </summary>
        public bool IsSystemRole { get; set; }
        /// <summary>
        /// guid of Tenat
        /// </summary>
        public Nullable<Guid> TenantId { get; set; }
        /// <summary>
        /// Role created by
        /// </summary>
        public Guid CreatedBy { get; set; }
        /// <summary>
        /// Role creation date
        /// </summary>
        public System.DateTime CreatedDate { get; set; }
        /// <summary>
        /// Role Modified by
        /// </summary>
        public Nullable<Guid> ModifiedBy { get; set; }
        /// <summary>
        /// Modified date of Role
        /// </summary>
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        /// <summary>
        /// Role deactivated by
        /// </summary>
        public Nullable<Guid> DeactivatedBy { get; set; }
        /// <summary>
        /// Deactivated date of role
        /// </summary>
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        /// <summary>
        /// Guid of role
        /// </summary>
        public System.Guid Guid { get; set; }
        /// <summary>
        /// Privileges of role
        /// </summary>
        public List<Guid> Privileges { get; set; }
    }

    public class NewRoleModel
    {
        /// <summary>
        /// Role Name
        /// </summary>
        [Required]
        public string Name { get; set; }
        [Required]
        public List<Guid> Privileges { get; set; }


        public NewRoleModel()
        {
            this.Privileges = new List<Guid>();
        }



        public Guid TenantId { get; set; }
    }

    public class EditRoleModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public List<Guid> Privileges { get; set; }
    }

    public class AllRolesViewModel
    {
        public string Name { get; set; }
    }

    public class RoleViewResponseModel 
    {
        public string Name { get; set; }    
    }

}
