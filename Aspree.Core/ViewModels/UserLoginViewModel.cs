using System;
using Swashbuckle.Examples;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class UserLoginViewModel
    {
        //[IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// First name of user
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Last name of user
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Email of user
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Mobile no. of user
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// Address of user
        /// </summary>
        public string Address { get; set; }
       [IgnoreDataMember]
        public string Password { get; set; }
        [IgnoreDataMember]
        public string Salt { get; set; }
        /// <summary>
        /// Guid of security question
        /// </summary>
        public Guid SecurityQuestionId { get; set; }
        /// <summary>
        /// Answer of security question
        /// </summary>
        public string Answer { get; set; }
        //public string AccessToken { get; set; }
        /// <summary>
        /// Guid of Tenat
        /// </summary>
        public Guid TenantId { get; set; }
        /// <summary>
        /// AuthTypeId of user
        /// </summary>
        public int AuthTypeId { get; set; }
        /// <summary>
        /// Created by
        /// </summary>
        public Guid CreatedBy { get; set; }
        /// <summary>
        /// Created date
        /// </summary>
        public System.DateTime CreatedDate { get; set; }
        /// <summary>
        /// Modified by
        /// </summary>
        public Nullable<Guid> ModifiedBy { get; set; }
        /// <summary>
        /// ModifiedDate
        /// </summary>
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        /// <summary>
        /// DeactivatedBy
        /// </summary>
        public Nullable<Guid> DeactivatedBy { get; set; }
        /// <summary>
        /// Date of deactivation
        /// </summary>
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        /// <summary>
        /// Guid
        /// </summary>
        public System.Guid Guid { get; set; }
        /// <summary>
        /// RoleId of User
        /// </summary>
        public Guid RoleId { get; set; }
        /// <summary>
        /// Role name of user
        /// </summary>
        public string RoleName { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// Profileof user
        /// </summary>
        public string Profile { get; set; }
        /// <summary>
        /// TemoGuid
        /// </summary>
        public Guid? TempGuid { get; set; }
        /// <summary>
        /// UserTypeId of user
        /// </summary>
        public Nullable<int> UserTypeId { get; set; }
        /// <summary>
        /// Username of user
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// IsApiAccessEnabled
        /// </summary>
        public Nullable<bool> IsApiAccessEnabled { get; set; }
        /// <summary>
        /// IsUserApprovedBySystemAdmin
        /// </summary>
        public bool IsUserApprovedBySystemAdmin { get; set; }
    }

    public class NewUserViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Mobile { get; set; }
        public string Address { get; set; }
        [Required]
        public Guid RoleId { get; set; }
        [Required]
        public Guid TenantId { get; set; }

        public string Profile { get; set; }
        public string UserName { get; set; }
        public bool IsUserApprovedBySystemAdmin { get; set; }
    }

    public class EditUserViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Mobile { get; set; }
        public string Address { get; set; }
        [Required]
        public Guid RoleId { get; set; }
        [Required]
        public Guid TenantId { get; set; }

        public string Profile { get; set; }
        public string UserName { get; set; }
        public bool IsUserApprovedBySystemAdmin { get; set; }
        public int Status { get; set; }
    }
    public class ResetPasswordViewModel
    {
        /// <summary>
        /// Email of user
        /// </summary>
        [Required]
        [Display(Name = "To EMail")]
        public string ToEMail { get; set; }
        /// <summary>
        /// Guid of Security Question
        /// </summary>
        //[Required]
        [Display(Name = "Security Question")]
        public Guid QuestionGuid { get; set; }
        /// <summary>
        /// Answer of Security question
        /// </summary>
        //[Required]
        [Display(Name = "Answer")]
        public string Answer { get; set; }

    }

    public class SmtpConfigurationViewModel
    {

        public string EmailFrom { get; set; }

        public string Password { get; set; }

        public string DisplayName { get; set; }

        public string SmtpHost { get; set; }

        public string Port { get; set; }
        public bool UserPassword { get; set; }
    }


    public class TestEnvironment_UserLoginViewModel
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; }
        public string Name { get; set; }
        public bool IsSystemRole { get; set; }
        public Guid TenantId { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public Int32 expires_in { get; set; }


    }

    public class GetAllUserLoginViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List <UserLoginViewModel>()
            {
                new UserLoginViewModel
                {
                            Id = 1,
                            FirstName = "Example First Name",
                            LastName = "Example Last Name",
                            Email = "Text Box",
                            Mobile = "123456789",
                            Address = "Text Box",
                            SecurityQuestionId = Guid.NewGuid(),
                            Answer = "Example Answer",
                            TenantId = Guid.NewGuid(),
                            AuthTypeId = 1,
                            CreatedBy = Guid.NewGuid(),
                            CreatedDate = DateTime.UtcNow,
                            ModifiedBy = Guid.NewGuid(),
                            ModifiedDate = DateTime.UtcNow,
                            DeactivatedBy = null,
                            DateDeactivated = DateTime.UtcNow,
                            Guid = Guid.NewGuid(),
                            RoleId = Guid.NewGuid(),
                            RoleName = "Example Name",
                            Status = 1,
                            Profile = "Example Profile",
                            TempGuid = Guid.NewGuid(),
                            UserTypeId = 1,
                            UserName = "Text Box",
                            IsApiAccessEnabled = true

                }
               
            };
        }
    }

   public class UserLoginViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new UserLoginViewModel()
            {
                Id = 1,
                FirstName = "Example First Name",
                LastName = "Example Last Name",
                Email = "Text Box",
                Mobile = "123456789",
                Address = "Text Box",
                SecurityQuestionId = Guid.NewGuid(),
                Answer = "Example Answer",
                TenantId = Guid.NewGuid(),
                AuthTypeId = 1,
                CreatedBy = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow,
                DeactivatedBy = null,
                DateDeactivated = DateTime.UtcNow,
                Guid = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                RoleName = "Example Name",
                Status = 1,
                Profile = "Example Profile",
                TempGuid = Guid.NewGuid(),
                UserTypeId = 1,
                UserName = "Text Box",
                IsApiAccessEnabled = true
            };
        }
    }

    public class ResetPasswordViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ResetPasswordViewModel()
            {
                ToEMail = "Example abc@gmail.com",
                QuestionGuid = Guid.NewGuid(),
                Answer = "Example Answer"
            };
        }

    }

    public class NewUserViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new NewUserViewModel
            {
                Address = "Example Address",
                Email = "Example Eamil",
                FirstName = "Example First name",
                LastName = "Example Last name",
                Mobile = "Example mobile",
                Profile = "Example profile",
                RoleId = Guid.NewGuid(),
                TenantId = Guid.NewGuid(),
                UserName = "Example User Name",
            };
        }
    }

    public class EditUserViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new EditUserViewModel
            {
                Address = "Example Address",
                Email = "Example Email",
                FirstName = "Example First Name",
                LastName = "Example Last Name",
                Mobile = "Example Mobile",
                Profile = "Example Profile",
                RoleId = Guid.NewGuid(),
                TenantId = Guid.NewGuid(),
                UserName = "Example User Name",
            };
        }
    }

    public class SystemAdminToolsUserViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string RoleName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string UserName { get; set; }
        public Guid Guid { get; set; }
        public int Id { get; set; }
        public Guid RoleId { get; set; }
        public int Status { get; set; }
        public string EntID { get; set; }

        public Guid FormId { get; set; }
        public Guid ProjectId { get; set; }
    }
}


