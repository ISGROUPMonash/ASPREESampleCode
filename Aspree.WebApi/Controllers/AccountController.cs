using Aspree.Core.ViewModels;
using Aspree.Provider.Interface;
using Aspree.WebApi.Utilities;
using Swashbuckle.Examples;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace Aspree.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AccountController : BaseController
    {
        private readonly IUserLoginProvider _userLoginProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userLoginProvider"></param>
        public AccountController(IUserLoginProvider userLoginProvider)
        {
            _userLoginProvider = userLoginProvider;
        }



        /// <summary>
        /// Change user password
        /// </summary>
        /// <remarks>
        /// Change user password<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to change password.
        /// - Changed password will be saved in SQL database.
        /// <br></br> 
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="changePassword">ChangePassword Model</param>
        // PUT: api/ActivityCategory/5
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]        
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]        
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerRequestExample(typeof(ChangePassword), typeof(ChangePasswordExamples))]
        [HttpPut]
        [Route("api/v1/Account/ChangePassword")]
        //[Authorize(Roles = "Admin")]
        public HttpResponseMessage ChangePassword([FromBody]ChangePassword changePassword)
        {
            if (Request.Headers.Authorization == null)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login.")
                };
                throw new HttpResponseException(msg);
            }

            #region check login

            if (this.LoggedInUserId == Guid.Empty)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login."),
                });
            }
            #endregion

            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.AllErrors());
            }

            var result = _userLoginProvider.ChangePassword(this.LoggedInUserId, changePassword);

            if (!result)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Password was not updated successfully.");
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);

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
        [Route("api/v1/Account/ResetForgetPassword/{guid}")]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        [AllowAnonymous]
        public HttpResponseMessage ResetForgetPassword(Guid guid, [FromBody]ResetPassword resetPassword)
        {
            resetPassword.Guid = guid;

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var result = _userLoginProvider.ResetNewPassword(resetPassword);

            if (!result)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Password was not reset successfully.");
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);

        }


        /// <summary>
        /// Update security question for user.
        /// </summary>
        /// <remarks>
        /// Update security question for user.<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to update security question of an user for password recovery.
        /// - Security question can be changed from manage profile page.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="securityQuestion">UpdateUserSecurityQuestion Model</param>
        // PUT: api/ActivityCategory/5
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]        
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerRequestExample(typeof(UpdateUserSecurityQuestion), typeof(UpdateUserSecurityQuestionExample))]
        //[Swashbuckle.Examples.SwaggerRequestExample(typeof(UpdateUserSecurityQuestion), typeof(UpdateUserSecurityQuestionExample))]
        [HttpPut]
        //[Authorize(Roles = "Admin")]
        [Route("api/v1/Account/SecurityQuestion")]
        public HttpResponseMessage SecurityQuestion([FromBody]UpdateUserSecurityQuestion securityQuestion)
        {
            if (Request.Headers.Authorization == null)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login.")
                };
                throw new HttpResponseException(msg);
            }

            #region check login

            if (this.LoggedInUserId == Guid.Empty)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login."),
                });
            }
            #endregion
            securityQuestion.UserGuid = this.LoggedInUserId;

            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.AllErrors());
            }

            var result = _userLoginProvider.EditSecurityQuestion(securityQuestion);

            if (!result)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Security question was not reset successfully.");
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);

        }

        /// <summary>
        /// Check username availability
        /// </summary>
        /// <remarks>
        /// Check username availability<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to check availability of username based on username and its authType.
        /// - This api takes userName, authType, userId as input request.
        /// - Returns true if username and authentication type already exists into database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>        
        /// <param name="userName">UserName that to be check</param>
        /// <param name="authType">Auth type of user for login</param>
        /// <param name="userid">User id(optional)</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]                
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [Route("api/v1/Account/CheckUsernameAvailability/{userName}/{authType}/{userid}")]
        [HttpGet]
        public bool CheckUsernameAvailability(string userName, Guid? authType=null, Guid? userid = null)
        {
            if (Request.Headers.Authorization == null)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login.")
                };
                throw new HttpResponseException(msg);
            }

            #region check login

            if (this.LoggedInUserId == Guid.Empty)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login."),
                });
            }
            #endregion
            if (authType == null)
            {
                GuidExceptionHandler();
            }
            var newauthType = authType.Value;
            var isExist = _userLoginProvider.checkUsernameExist(userName, newauthType, userid);
            return isExist;
        }




        /// <summary>
        /// Check username availability
        /// </summary>
        /// <remarks>
        /// Check username availability<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to check availability of username based on username and its authType.
        /// - This api takes userName, authType, userId as input request.
        /// - Returns true if username and authentication type already exists into database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>        
        /// <param name="password">UserName that to be check</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]                
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/v1/Account/CheckUserpassword")]
        [HttpPost]
        public bool CheckUserpassword([FromBody]string password)
        {
            var isExist = _userLoginProvider.IsPassExist(password);
            return isExist;
        }


    }
}
