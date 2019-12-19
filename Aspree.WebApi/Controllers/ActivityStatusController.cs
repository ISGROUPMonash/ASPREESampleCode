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
    public class ActivityStatusController : BaseController
    {
        private readonly IActivityStatusProvider _activityStatusProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityStatusProvider"></param>
        public ActivityStatusController(IActivityStatusProvider activityStatusProvider)
        {
            _activityStatusProvider = activityStatusProvider;
        }

        /// <summary>
        /// Get all activity status
        /// </summary>
        /// <remarks>
        ///Get all activity status
        /// <para /> This api is used to get all activity status.
        /// </remarks>
        /// <returns>All Activity Category</returns>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<ActivityStatusViewModel>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        public IEnumerable<ActivityStatusViewModel> GetAll()
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
            return _activityStatusProvider.GetAll();
        }

        /// <summary>
        /// Get activity status by guid
        /// </summary>
        /// <remarks>
        /// Get activity status by guid
        /// <para />This api is used to get activity status using guid.
        /// <para />This api returns ActivityStatusViewModel model as response.
        /// </remarks>
        /// <param name="guid">Guid of Activity Status</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityStatusViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
       
        public ActivityStatusViewModel Get(Guid guid)
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
            var activityStatus = _activityStatusProvider.GetByGuid(guid);
            if (activityStatus == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Activity status was not found."),
                    ReasonPhrase = "Activity status was not found."
                });
            }
            return activityStatus;
        }

        /// <summary>
        /// Add New Activity Status
        /// </summary>
        /// <remarks>
        /// Add New Activity Status
        /// <para />This api is used to create a new activity status.
        /// <para />This api takes NewStatus model as input request.
        /// <para />This api returns created ActivityStatusViewModel model as response model.
        /// </remarks>
        /// <param name="newStatus">Activity Status Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityStatusViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
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

           var added =  _activityStatusProvider.Create(new ActivityStatusViewModel() {
                IsActive = true,
                Status = newStatus.Status
            });
            _activityStatusProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, added);
        }

        /// <summary>
        /// Update an existing activity status
        /// </summary>
        /// <remarks>
        /// Update an existing activity status
        /// <para />This api is used to update an existing activity status.
        /// <para />This api takes guid of an activity status and  EditStatus model with updated values as input request.
        /// <para />This api returns updated ActivityStatusViewModel model as response model.
        /// </remarks>
        /// <param name="editStatus">Activity Status Model</param>
        /// <param name="guid">Guid of ativity status that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityStatusViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
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

            var updatedStatus = _activityStatusProvider.Update(new ActivityStatusViewModel() {
                Guid = guid,
                Status = editStatus.Status
            });
            _activityStatusProvider.SaveChanges();

            if (updatedStatus != null)
                return Request.CreateResponse(HttpStatusCode.OK, updatedStatus);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Activity status was not found.");
        }

        /// <summary>
        /// Delete an activity status by guid
        /// </summary>
        /// <remarks>
        ///  Delete an activity status by guid
        ///  <para />This api is used to delete an activity status.
        ///  <para />This api takes guid of an activity status as input parameter in input request.
        ///  <para />This api returns ActivityStatusViewModel of deleted activity status.
        /// </remarks>
        /// <param name="guid">Guid of activity status that has to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityStatusViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
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
            var deletedStatus = _activityStatusProvider.DeleteByGuid(guid, this.LoggedInUserId);
            _activityStatusProvider.SaveChanges();

            if (deletedStatus != null)
                return Request.CreateResponse(HttpStatusCode.OK, deletedStatus);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Activity status was not found.");
        }
    }
}
