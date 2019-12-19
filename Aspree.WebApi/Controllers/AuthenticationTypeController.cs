using Aspree.Core.ViewModels;
using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using Swashbuckle.Examples;

namespace Aspree.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
    public class AuthenticationTypeController : BaseController
    {
        private readonly IAuthenticationTypeProvider _authenticationTypeProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authenticationTypeProvider"></param>
        public AuthenticationTypeController(IAuthenticationTypeProvider authenticationTypeProvider)
        {
            this._authenticationTypeProvider = authenticationTypeProvider;
        }
        /// <summary>
        /// Get all authentication types
        /// </summary>
        /// <remarks>
        /// Get all authentication types<br></br>
        /// <strong>Purpose.</strong>
        /// - The purpose of this api is to get the list of all authentication types.
        /// - The api fetches all authentication types from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        // GET: api/v1/AuthenticationType
        //[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]        
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<AuthenticationTypeViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllAuthenticationTypeViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        public HttpResponseMessage GetAll()
        {
            if (Request.Headers.Authorization == null)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login.")
                };
                throw new HttpResponseException(msg);
            }
            IEnumerable<AuthenticationTypeViewModel> authList = _authenticationTypeProvider.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, authList);
        }

        /// <summary>
        /// Get authentication type by guid
        /// </summary>
        /// <remarks>
        ///  Get authentication type by guid<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get authentication type based on its guid.
        /// - Authentication type will be fetched from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of an authentication type that is to be fetched</param>
        // GET: api/AuthenticationType/5
        //[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]       
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]

        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "This error is returned by the server when we pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided filters are found")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(AuthenticationTypeViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(AuthenticationTypeViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        public AuthenticationTypeViewModel Get(Guid? guid)
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
            //if (this.LoggedInUserId == Guid.Empty)
            //{
            //    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
            //    {
            //        Content = new StringContent("Unauthorized access. Please login."),
            //    });
            //}
            #endregion



            if (guid == null)
            {
                GuidExceptionHandler();
            }
            Guid newguid = guid.Value;

            var checkList = _authenticationTypeProvider.GetByGuid(newguid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("authentication type was not found."),
                });
            }

            return checkList;
        }


        /// <summary>
        /// Add new authentication type
        /// </summary>
        /// <remarks>
        /// Add new authentication type<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to create new authentication type into the system.
        /// - The new authentication type will be saved into SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newAuthenticationTypeViewModel">new authentication type model</param>
        // POST: api/AuthenticationType
        //[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        //[SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(AuthenticationTypeViewModel))]
        [SwaggerRequestExample(typeof(NewEntitySubTypeViewModel), typeof(NewAuthenticationTypeViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(AuthenticationTypeViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPost]
        public HttpResponseMessage Post([FromBody]NewAuthenticationTypeViewModel newAuthenticationTypeViewModel)
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

            var addedCategory = _authenticationTypeProvider.Create(new AuthenticationTypeViewModel()
            {
                UserName = newAuthenticationTypeViewModel.UserName,
                AuthTypeName = newAuthenticationTypeViewModel.AuthTypeName,
                AuthType = newAuthenticationTypeViewModel.AuthType,
                CreatedBy = LoggedInUserId,
                ClientId = newAuthenticationTypeViewModel.ClientId,
                ClientSecret = newAuthenticationTypeViewModel.ClientSecret,
                Domain = newAuthenticationTypeViewModel.Domain,
                Scope = newAuthenticationTypeViewModel.Scope,
                State = newAuthenticationTypeViewModel.State,
                AuthenticationProviderClaim = newAuthenticationTypeViewModel.AuthenticationProviderClaim,
                AuthorizeEndpoint = newAuthenticationTypeViewModel.AuthorizeEndpoint,
                TokenEndpoint = newAuthenticationTypeViewModel.TokenEndpoint,
                IntrospectEndpoint = newAuthenticationTypeViewModel.IntrospectEndpoint,
                RevokeEndpoint = newAuthenticationTypeViewModel.RevokeEndpoint,
                LogoutEndpoint = newAuthenticationTypeViewModel.LogoutEndpoint,
                KeysEndpoint = newAuthenticationTypeViewModel.KeysEndpoint,
                UserinfoEndpoint = newAuthenticationTypeViewModel.UserinfoEndpoint,
            });

            _authenticationTypeProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, addedCategory);
        }

        /// <summary>
        /// Edit authentication type
        /// </summary>
        /// <remarks>
        /// Edit authentication type<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to edit/update an existing authentication type using its guid.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editAuthenticationTypeViewModel">Edit Authentication Type Model</param>
        /// <param name="guid">guid of Authentication Type that has to be edited</param>
        // PUT: api/AuthenticationType/5
        //[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]        
        //[SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(AuthenticationTypeViewModel))]
        [SwaggerRequestExample(typeof(EditAuthenticationTypeViewModel), typeof(EditAuthenticationTypeViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(AuthenticationTypeViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPut]
        public HttpResponseMessage Put(Guid guid, [FromBody]EditAuthenticationTypeViewModel editAuthenticationTypeViewModel)
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

            var updatedCategory = _authenticationTypeProvider.Update(new AuthenticationTypeViewModel()
            {
                ModifiedBy = LoggedInUserId,
                ModifiedDate = DateTime.UtcNow,
                Guid = guid,
                UserName = editAuthenticationTypeViewModel.UserName,
                AuthTypeName = editAuthenticationTypeViewModel.AuthTypeName,
                AuthType = editAuthenticationTypeViewModel.AuthType,
                ClientId = editAuthenticationTypeViewModel.ClientId,
                ClientSecret = editAuthenticationTypeViewModel.ClientSecret,
                Domain = editAuthenticationTypeViewModel.Domain,
                Scope = editAuthenticationTypeViewModel.Scope,
                State = editAuthenticationTypeViewModel.State,

                AuthenticationProviderClaim = editAuthenticationTypeViewModel.AuthenticationProviderClaim,
                AuthorizeEndpoint = editAuthenticationTypeViewModel.AuthorizeEndpoint,
                TokenEndpoint = editAuthenticationTypeViewModel.TokenEndpoint,
                IntrospectEndpoint = editAuthenticationTypeViewModel.IntrospectEndpoint,
                RevokeEndpoint = editAuthenticationTypeViewModel.RevokeEndpoint,
                LogoutEndpoint = editAuthenticationTypeViewModel.LogoutEndpoint,
                KeysEndpoint = editAuthenticationTypeViewModel.KeysEndpoint,
                UserinfoEndpoint = editAuthenticationTypeViewModel.UserinfoEndpoint,
            });

            if (updatedCategory == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "authentication type was not found.");
            }
            _authenticationTypeProvider.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, updatedCategory);
        }


        /// <summary>
        /// Delete existing authentication type
        /// </summary>
        /// <remarks>
        /// Delete existing authentication type<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to delete an existing authentication type.
        /// - The data will be soft deleted from SQL table.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of Authentication Type that is to be deleted</param>
        // DELETE: api/AuthenticationType/5
        //[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]        
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided GUID are found")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(AuthenticationTypeViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(AuthenticationTypeViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpDelete]
        public HttpResponseMessage Delete(Guid guid)
        {
            
            #region check login
            if (this.LoggedInUserId == Guid.Empty)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login."),
                });
            }
            #endregion
           
            var deletedAuthenticationType = _authenticationTypeProvider.DeleteByGuid(guid, LoggedInUserId);
            _authenticationTypeProvider.SaveChanges();

            if (deletedAuthenticationType != null)
                return Request.CreateResponse(HttpStatusCode.OK, deletedAuthenticationType);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Authentication Type was not found.");
        }

        /// <summary>
        /// Get authentication type by guid
        /// </summary>
        /// <remarks>
        /// Get authentication type by guid<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get an authentication type by state.
        /// - Authentication type will be fetched from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="state">State of an Authentication Type that needs to be fetched</param>
        // GET: api/AuthenticationType/5
        //[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]        
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]

        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(AuthenticationTypeViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(AuthenticationTypeViewModelExamples))]
         //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [Route("api/v1/AuthenticationType/GetAuthenticationTypeByState/{state}")]
        [HttpGet]
        [AllowAnonymous]
        public AuthenticationTypeViewModel GetAuthenticationTypeByState(string state)
        {
            
            var checkList = _authenticationTypeProvider.GetAuthenticationTypeByState(state);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("authentication type was not found."),
                });
            }

            return checkList;
        }

        /// <summary>
        /// Get authentication type by guid
        /// </summary>
        /// <remarks>
        /// Get authentication type by guid<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get an authentication type by state.
        /// - Authentication type will be fetched from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">State of an Authentication Type that needs to be fetched</param>
        // GET: api/AuthenticationType/5
        //[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]        
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]

        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(AuthenticationTypeViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(AuthenticationTypeViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [Route("api/v1/AuthenticationType/InActiveByGuid/{guid}")]
        [HttpGet]
        [AllowAnonymous]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public AuthenticationTypeViewModel InActiveByGuid(Guid guid)
        {

            var checkList = _authenticationTypeProvider.InActiveByGuid(guid, LoggedInUserId);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("authentication type was not found."),
                });
            }

            return checkList;
        }


    }
}