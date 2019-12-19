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
    public class StatusController : BaseController
    {
        // GET: api/Status
        private readonly IStatusProvider _statusProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusProvider"></param>
        public StatusController(IStatusProvider statusProvider)
        {
            _statusProvider = statusProvider;
        }


        /// <summary>
        /// Get All Status
        /// </summary>
        /// <remarks>
        /// Get All Status
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<StatusViewModel>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // GET: api/Status
        public IEnumerable<StatusViewModel> GetAll()
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
            return _statusProvider.GetAll();
        }


        /// <summary>
        /// Get Status By Guid
        /// </summary>
        /// <remarks>
        /// Get Status By Guid
        /// </remarks>
        /// <param name="guid">Guid of status that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(StatusViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]   
        // GET: api/Status/5
        public StatusViewModel Get(Guid guid)
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
            var checkList = _statusProvider.GetByGuid(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Status was not found."),
                    ReasonPhrase = "Status was not found."
                });
            }

            return checkList;
        }

        /// <summary>
        /// Add New Status
        /// </summary>
        /// <remarks>
        /// Add New Status
        /// </remarks>
        /// <param name="newStatus">New Status Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(StatusViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // POST: api/Status
        public HttpResponseMessage Post([FromBody]NewStatus newStatus)
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

           var added =  _statusProvider.Create(new StatusViewModel() {
                Status = newStatus.Status,
                IsActive = true
            });
            _statusProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, added);
        }


        /// <summary>
        /// Edit Existing Status
        /// </summary>
        /// <remarks>
        /// Edit Existing Status
        /// </remarks>
        /// <param name="editStatus">Edit Status Model</param>
        /// <param name="guid">guid of variable category that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(StatusViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // PUT: api/Status/5
        public HttpResponseMessage Put(Guid guid, [FromBody]EditStatus editStatus)
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

            var response = _statusProvider.Update(new StatusViewModel() {
                Status = editStatus.Status,
                Guid = guid
            });
            _statusProvider.SaveChanges();

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Status was not found.");
        }


        /// <summary>
        /// Delete Existing Status
        /// </summary>
        /// <remarks>
        /// Delete Existing Status
        /// </remarks>
        /// <param name="guid">Guid of status that is to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(StatusViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // DELETE: api/Status/5
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
            var response = _statusProvider.DeleteByGuid(guid, this.LoggedInUserId);
            _statusProvider.SaveChanges();

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Status was not found.");
        }
    }
}