using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class AuthenticationTypeViewModel
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Name of User
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Authentication type Name
        /// </summary>
        public string AuthTypeName { get; set; }
        /// <summary>
        /// Authentication type
        /// </summary>
        public int AuthType { get; set; }
        /// <summary>
        /// Domain
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        /// Client Id
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Client Secret
        /// </summary>
        public string ClientSecret { get; set; }
        /// <summary>
        /// Scope of Authentication Type
        /// </summary>
        public string Scope { get; set; }
        /// <summary>
        /// State of Authentication Type
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// Authentication Type Created By 
        /// </summary>
        public Guid CreatedBy { get; set; }
        /// <summary>
        /// CreatedDate of Authentication Type
        /// </summary>
        public System.DateTime CreatedDate { get; set; }
        /// <summary>
        ///  Authentication Type Modified By
        /// </summary>
        public Nullable<Guid> ModifiedBy { get; set; }
        /// <summary>
        /// ModifiedDate of Authentication Type
        /// </summary>
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        /// <summary>
        /// Authentication Type DeactivatedBy
        /// </summary>
        public Nullable<Guid> DeactivatedBy { get; set; }
        /// <summary>
        /// Deactivated Date of Authentication Type
        /// </summary>
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        public System.Guid Guid { get; set; }

        /// <summary>
        /// AuthenticationProviderClaim
        /// </summary>
        public string AuthenticationProviderClaim { get; set; }
        /// <summary>
        /// AuthorizeEndpoint
        /// </summary>
        public string AuthorizeEndpoint { get; set; }
        /// <summary>
        /// TokenEndpoint
        /// </summary>
        public string TokenEndpoint { get; set; }
        /// <summary>
        /// IntrospectEndpoint
        /// </summary>
        public string IntrospectEndpoint { get; set; }
        /// <summary>
        /// RevokeEndpoint
        /// </summary>
        public string RevokeEndpoint { get; set; }
        /// <summary>
        /// LogoutEndpoint
        /// </summary>
        public string LogoutEndpoint { get; set; }
        /// <summary>
        /// KeysEndpoint
        /// </summary>
        public string KeysEndpoint { get; set; }
        /// <summary>
        /// UserinfoEndpoint
        /// </summary>
        public string UserinfoEndpoint { get; set; }

        /// <summary>
        /// Status of Authentication Type
        /// </summary>
        public int Status { get; set; }
    }

    public class GetAllAuthenticationTypeViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {

           return new List<AuthenticationTypeViewModel>()
            {
               new AuthenticationTypeViewModel
               {
                AuthenticationProviderClaim = "Test",
                AuthorizeEndpoint = "End-point",
                AuthType = 1,
                AuthTypeName = "Test-Auth",
                ClientId = "Client",
                ClientSecret = "Client-Secret",
                CreatedBy = Guid.NewGuid(),
                Guid = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                DateDeactivated = DateTime.UtcNow,
                DeactivatedBy = Guid.NewGuid(),
                Domain = "Domain",
                Id = 1,
                IntrospectEndpoint = "end-point",
                KeysEndpoint = "key-point",
                LogoutEndpoint = "logout-end-point",
                ModifiedBy = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow,
                RevokeEndpoint = "Revoke-point",
                Scope = "scope",
                State = "Active",
                TokenEndpoint = "token-end-point",
                UserinfoEndpoint = "user-end-point",
                UserName = "Test",
                Status = (int)Enum.Status.Active,
               }
                

            };
        }
    }
    public class AuthenticationTypeViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new AuthenticationTypeViewModel
            {
               AuthenticationProviderClaim="Test",
               AuthorizeEndpoint="End-point",
               AuthType=1,
               AuthTypeName="Test-Auth",
               ClientId="Client",
               ClientSecret="Client-Secret",
               CreatedBy=Guid.NewGuid(),
               Guid=Guid.NewGuid(),
               CreatedDate=DateTime.UtcNow,
               DateDeactivated=DateTime.UtcNow,
               DeactivatedBy=Guid.NewGuid(),
               Domain="Domain",
               Id=1,
               IntrospectEndpoint="end-point",
               KeysEndpoint="key-point",
               LogoutEndpoint="logout-end-point",
               ModifiedBy=Guid.NewGuid(),
               ModifiedDate=DateTime.UtcNow,
               RevokeEndpoint="Revoke-point",
               Scope="scope",
               State="Active",
               TokenEndpoint="token-end-point",
               UserinfoEndpoint="user-end-point",
               UserName="Test",
                Status = (int)Enum.Status.Active,
            };
        }
    }


 

    public class NewAuthenticationTypeViewModel
    {
        [IgnoreDataMember]
        public int Id { get; set; }

        //[Required(ErrorMessage = "This field is required.")]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "AuthTypeName")]
        public string AuthTypeName { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "AuthType")]
        public int AuthType { get; set; }

        //[Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Domain")]
        public string Domain { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "ClientId")]
        public string ClientId { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "ClientSecret")]
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string State { get; set; }


        public string AuthenticationProviderClaim { get; set; }
        public string AuthorizeEndpoint { get; set; }
        public string TokenEndpoint { get; set; }
        public string IntrospectEndpoint { get; set; }
        public string RevokeEndpoint { get; set; }
        public string LogoutEndpoint { get; set; }
        public string KeysEndpoint { get; set; }
        public string UserinfoEndpoint { get; set; }
    }

    public class NewAuthenticationTypeViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new AuthenticationTypeViewModel
            {
                AuthenticationProviderClaim = "Test",
                AuthorizeEndpoint = "End-point",
                AuthType = 1,
                AuthTypeName = "Test-Auth",
                ClientId = "Client",
                ClientSecret = "Client-Secret",
                CreatedBy = Guid.NewGuid(),
                Guid = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                DateDeactivated = DateTime.UtcNow,
                DeactivatedBy = Guid.NewGuid(),
                Domain = "Domain",
                Id = 1,
                IntrospectEndpoint = "end-point",
                KeysEndpoint = "key-point",
                LogoutEndpoint = "logout-end-point",
                ModifiedBy = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow,
                RevokeEndpoint = "Revoke-point",
                Scope = "scope",
                State = "Active",
                TokenEndpoint = "token-end-point",
                UserinfoEndpoint = "user-end-point",
                UserName = "Test",
                

            };
        }
    }

    public class EditAuthenticationTypeViewModel
    {
        public int Id { get; set; }
        //[Required(ErrorMessage = "This field is required.")]
        [Display(Name = "UserName")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "AuthTypeName")]
        public string AuthTypeName { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "AuthType")]
        public int AuthType { get; set; }

        //[Required(ErrorMessage = "This field is required.")]
        [Display(Name = "Domain")]
        public string Domain { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "ClientId")]
        public string ClientId { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [Display(Name = "ClientSecret")]
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
        public string State { get; set; }
        public System.Guid Guid { get; set; }


        public string AuthenticationProviderClaim { get; set; }
        public string AuthorizeEndpoint { get; set; }
        public string TokenEndpoint { get; set; }
        public string IntrospectEndpoint { get; set; }
        public string RevokeEndpoint { get; set; }
        public string LogoutEndpoint { get; set; }
        public string KeysEndpoint { get; set; }
        public string UserinfoEndpoint { get; set; }
    }


    public class EditAuthenticationTypeViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new EditAuthenticationTypeViewModel
            {
                AuthenticationProviderClaim = "Test",
                AuthorizeEndpoint = "End-point",
                AuthType = 1,
                AuthTypeName = "Test-Auth",
                ClientId = "Client",
                Guid = Guid.NewGuid(),
                ClientSecret = "Client-Secret",
                Domain = "Domain",
                Id = 1,
                IntrospectEndpoint = "end-point",
                KeysEndpoint = "key-point",
                LogoutEndpoint = "logout-end-point",
                RevokeEndpoint = "Revoke-point",
                Scope = "scope",
                State = "Active",
                TokenEndpoint = "token-end-point",
                UserinfoEndpoint = "user-end-point",
                UserName = "Test",

             };
        }
    }

}
