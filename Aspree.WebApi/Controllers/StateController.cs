using Aspree.Core.ViewModels;
using Aspree.Provider.Interface;
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
    [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
    public class StateController : BaseController
    {
        private readonly IStateProvider _stateProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stateProvider"></param>
        public StateController(IStateProvider stateProvider)
        {
            _stateProvider = stateProvider;
        }


        /// <summary>
        /// Get All States
        /// </summary>
        /// <remarks>
        /// Get All States
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<StateViewModel>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // GET: api/State
        public IEnumerable<StateViewModel> GetAll()
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
            return _stateProvider.GetAll();
        }


        /// <summary>
        /// Get State By Guid
        /// </summary>
        /// <remarks>
        /// Get State By Guid
        /// </remarks>
        /// <param name="guid">Guid of a state that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(StateViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]        
        // GET: api/State/5
        public StateViewModel Get(Guid guid)
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
            var stateList = _stateProvider.GetByGuid(guid);
            if (stateList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("State was not found."),
                    ReasonPhrase = "State was not found."
                });
            }
            return stateList;
        }

        /// <summary>
        /// Add New State
        /// </summary>
        /// <remarks>
        /// Add New State
        /// </remarks>
        /// <param name="newState">New State Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(StateViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        /// <param name="model">model contains all the properties of NewStateViewModel</param>
        // POST: api/State
        public HttpResponseMessage Post([FromBody]NewStateViewModel newState)
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
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }
            var IsValidCountry = _stateProvider.CheckCountryById(newState.CountryId);
            if (IsValidCountry != null)
            {
               var added =  _stateProvider.Create(new StateViewModel()
                {
                    CountryId = newState.CountryId,
                    Name = newState.Name
                });
                _stateProvider.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, added);
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Provide valid country for state.");
        }


        /// <summary>
        /// Edit Existing State
        /// </summary>
        /// <remarks>
        /// Edit Existing State
        /// </remarks>
        /// <param name="editState">Edit State Model</param>
        /// <param name="guid">guid of variable category that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(StateViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // PUT: api/State/5
        public HttpResponseMessage Put(Guid guid, [FromBody]EditStateViewModel editState)
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
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var IsValidCountry = _stateProvider.CheckCountryById(editState.CountryId);

            if (IsValidCountry != null)
            {
               var updated =  _stateProvider.Update(new StateViewModel()
                {
                    CountryId = editState.CountryId,
                    Guid = guid,
                    Name = editState.Name
                });
                _stateProvider.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, updated);
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Provide valid country for state.");
        }

        /// <summary>
        /// Delete Existing State
        /// </summary>
        /// <remarks>
        /// Delete Existing State
        /// </remarks>
        /// <param name="guid">Guid of state that is to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(StateViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // DELETE: api/State/5
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
            var response = _stateProvider.DeleteByGuid(guid, this.LoggedInUserId);
            _stateProvider.SaveChanges();
            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "State was not found.");
        }
    }
}
