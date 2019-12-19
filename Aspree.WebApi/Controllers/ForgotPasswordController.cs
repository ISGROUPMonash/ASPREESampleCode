using Aspree.Core.ViewModels;
using Aspree.Provider.Interface;
using Swashbuckle.Examples;
using Aspree.WebApi.Utilities;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;


namespace Aspree.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ForgotPasswordController : BaseController
    {
        private readonly IForgotPasswordProvider __forgotPassword;
        private readonly IUserLoginProvider _userLoginProvider;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_forgotPassword"></param>
        public ForgotPasswordController(IForgotPasswordProvider _forgotPassword, IUserLoginProvider userLoginProvider)
        {
            this.__forgotPassword = _forgotPassword;
            this._userLoginProvider = userLoginProvider;
        }
        /// <summary>
        /// Forgot password
        /// </summary>
        /// <remarks>
        /// Forgot password<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to reset forgotten password.
        /// - This api sends a password reset link on e-mail id of the user.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="resetPass">ResetPassword Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerRequestExample(typeof(ResetPasswordViewModel), typeof(ResetPasswordViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [AllowAnonymous]
        public HttpResponseMessage Post([FromBody]ResetPasswordViewModel resetPass)
        {
            if (!ModelState.IsValid)
            {   
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.AllErrors());
            }
            var user = __forgotPassword.checkUser(resetPass);
            string added = string.Empty;
            if (user != null)
            {
                //if user is not approved by system admin then prevent to send password reset mail.
                if (!user.IsUserApprovedBySystemAdmin)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, false);
                }
                //if user is not approved by system admin then prevent to send password reset mail.
                if (user.Status != (int)Core.Enum.Status.Active)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, false);
                }
                var emailService = new Services.EmailService();
                bool isSent = emailService.SendForgotPasswordEmail(user.Email, user.FirstName + " " + user.LastName, Utilities.ConfigSettings.WebUrl + "/account/forgotpassword/" + user.TempGuid.ToString());
            }
            else
            {
            }
            return Request.CreateResponse(HttpStatusCode.OK, true);
        }


        /// <summary>
        /// Reset user password
        /// </summary>
        /// <remarks>
        /// Reset user password<br></br>   
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to create/reset password.
        /// - User can create/reset password from password creation link sent via email.
        /// - Reset password will be saved in SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="resetPassword">ResetPassword Model</param>
        /// <param name="guid">guid of the user whose password to be change</param>
        // PUT: api/ActivityCategory/5
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerRequestExample(typeof(ResetPassword), typeof(ResetPasswordExamples))]
        [HttpPut]
        [Route("api/v1/ForgotPassword/ResetForgetPassword/{guid}")]
        [AllowAnonymous]
        public HttpResponseMessage ResetForgetPassword(Guid guid, [FromBody]ResetPassword resetPassword)
        {
            resetPassword.Guid = guid;
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            var result = _userLoginProvider.ResetPassword(resetPassword);
            if (!result)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Password was not reset successfully.");
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
