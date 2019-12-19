using Aspree.Core.ViewModels;
using Aspree.Provider.Interface;
using Swashbuckle.Examples;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Aspree.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class SecurityQuestionController : BaseController
    {
        // GET: api/SecurityQuestion
        private readonly ISecurityQuestionProvider _securityQuestionProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="securityQuestionProvider"></param>
        public SecurityQuestionController(ISecurityQuestionProvider securityQuestionProvider)
        {
            _securityQuestionProvider = securityQuestionProvider;
        }

        /// <summary>
        /// Get all security questions
        /// </summary>
        /// <remarks>
        /// Get all security questions<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to get all security questions.
        /// - This api returns list of SecurityQuestionViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<SecurityQuestionViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllSecurityQuestionViewModelExamples))]
        // GET: api/SecurityQuestion
        //[AllowAnonymousAttribute]
        [AllowAnonymous]
        public HttpResponseMessage GetAll()
        {
            IEnumerable<SecurityQuestionViewModel> questions = _securityQuestionProvider.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, questions);
        }


        /// <summary>
        /// Get security question by guid
        /// </summary>
        /// <remarks>
        /// Get security question by guid<br></br>   
        /// <strong>Purpose.</strong>
        /// - This api is used to get security question by its Guid.
        /// - This api takes guid of a security question that needs to be fetched.
        /// - This api returns SecurityQuestionViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of a security question that needs to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]   
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SecurityQuestionViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(SecurityQuestionViewModelExamples))]
        [AllowAnonymous]
        // GET: api/SecurityQuestion/5
        public SecurityQuestionViewModel Get(Guid? guid)
        {
            if (guid == null)
            {
                GuidExceptionHandler();
            }
            var newguid = guid.Value;

            var checkList = _securityQuestionProvider.GetByGuid(newguid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Security question was not found."),
                    ReasonPhrase = "Security question was not found."
                });
            }

            return checkList;
        }


        /// <summary>
        /// Add new security question
        /// </summary>
        /// <remarks>
        /// Add new security question<br></br>  
        /// <strong>Purpose.</strong>
        /// - This api is used to create new security question.
        /// - This api takes NewSecurityQuestionViewModel of a security question as input request.
        /// - This api returns SecurityQuestionViewModel of new created security question in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newSecurityQuestion">NewSecurityQuestionViewModel</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SecurityQuestionViewModel))]
        [SwaggerRequestExample(typeof(NewSecurityQuestionViewModel), typeof(NewSecurityQuestionViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(SecurityQuestionViewModelExamples))]
        // POST: api/SecurityQuestion
        public HttpResponseMessage Post([FromBody]NewSecurityQuestionViewModel newSecurityQuestion)
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
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var added = _securityQuestionProvider.Create(new SecurityQuestionViewModel()
            {
                Question = newSecurityQuestion.Question,
                Guid = Guid.NewGuid()
            });
            _securityQuestionProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, added);
        }


        /// <summary>
        /// Edit existing security question
        /// </summary>
        /// <remarks>
        /// Edit existing security question<br></br> 
        /// <strong>Purpose.</strong>        
        /// - This api is used to update an existing security question.
        /// - This api takes EditSecurityQuestionViewModel and guid of security question as input request.
        /// - This api returns SecurityQuestionViewModel of updated security question in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editSecurityQuestion">EditSecurityQuestionViewModel</param>
        /// <param name="guid">Guid of security question that needs to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SecurityQuestionViewModel))]
        [SwaggerRequestExample(typeof(EditSecurityQuestionViewModel), typeof(EditSecurityQuestionViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(SecurityQuestionViewModelExamples))]
        // PUT: api/SecurityQuestion/5
        public HttpResponseMessage Put(Guid guid, [FromBody]EditSecurityQuestionViewModel editSecurityQuestion)
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
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var response = _securityQuestionProvider.Update(new SecurityQuestionViewModel()
            {
                Guid = guid,
                Question = editSecurityQuestion.Question
            });
            _securityQuestionProvider.SaveChanges();

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Security question was not found.");
        }

        /// <summary>
        /// Delete existing security question
        /// </summary>
        /// <remarks>
        /// Delete existing security question<br></br> 
        /// <strong>Purpose.</strong>
        /// - This api is used to delete an existing security question.
        /// - This api takes guid of a security question that needs to be deleted as input request.
        /// - This api returns SecurityQuestionViewModel of deleted security question in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of security question that needs to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]        
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SecurityQuestionViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(SecurityQuestionViewModelExamples))]
        // DELETE: api/SecurityQuestion/5
        public HttpResponseMessage Delete(Guid guid)
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
            var response = _securityQuestionProvider.DeleteByGuid(guid, this.LoggedInUserId);
            _securityQuestionProvider.SaveChanges();

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Security question was not found.");
        }
    }
}
