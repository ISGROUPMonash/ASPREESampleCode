using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public int SecurityQuestionId { get; set; }
        public string Answer { get; set; }
        public string AccessToken { get; set; }
        public int TenantId { get; set; }
        public int AuthTypeId { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> DeactivatedBy { get; set; }
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        public System.Guid Guid { get; set; }
    }

    public class NewUserViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public int SecurityQuestionId { get; set; }
        public string Answer { get; set; }
    }

    public class EditUserViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Mobile { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public int SecurityQuestionId { get; set; }
        public string Answer { get; set; }
        public System.Guid Guid { get; set; }
    }
}
