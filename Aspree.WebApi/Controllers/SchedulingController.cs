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
using System.Web.Http;

namespace Aspree.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class SchedulingController : BaseController
    {
        private readonly ISchedulingProvider _schedulingProvider;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schedulingProvider"></param>
        public SchedulingController(ISchedulingProvider schedulingProvider)
        {
            this._schedulingProvider = schedulingProvider;
        }

        /// <summary>
        /// Get activities scheduling by guid
        /// </summary>
        /// <remarks>
        /// Get activities scheduling by guid<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get scheduling based on its guid which will be fetched from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of an activity that needs to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SchedulingViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(SchedulingViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        public SchedulingViewModel Get(Guid guid)
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
            var checkList = _schedulingProvider.GetByGuid(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Scheduling was not found."),
                });
            }
            return checkList;
        }

        /// <summary>
        /// Add new scheduling
        /// </summary>
        /// <remarks>
        /// Add new scheduling<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to create new scheduling into the system which will be fetched from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newScheduling">NewSchedulingViewModel</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "record already exist with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SchedulingViewModel))]
        [SwaggerRequestExample(typeof(NewSchedulingViewModel), typeof(NewSchedulingViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(SchedulingViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPost]
        public HttpResponseMessage Post([FromBody]NewSchedulingViewModel newScheduling)
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
            var addScheduling = _schedulingProvider.Create(new SchedulingViewModel()
            {
                Guid = Guid.NewGuid(),
                ActivityId = newScheduling.ActivityId,
                ScheduledToBeCompleted = newScheduling.ScheduledToBeCompleted,
                ActivityAvailableForCreation = newScheduling.ActivityAvailableForCreation,
                RolesToCreateActivity = newScheduling.RolesToCreateActivity,
                RoleToCreateActivityRegardlessScheduled = newScheduling.RoleToCreateActivityRegardlessScheduled,
                OtherActivity = newScheduling.OtherActivity,
                OffsetCount = newScheduling.OffsetCount,
                OffsetType = newScheduling.OffsetType,
                SpecifiedActivity = newScheduling.SpecifiedActivity,
                CreationWindowOpens = newScheduling.CreationWindowOpens,
                CreationWindowClose = newScheduling.CreationWindowClose,
                IsScheduled = true,
                ScheduleDate = DateTime.UtcNow,
                ProjectId = newScheduling.ProjectId,
                CanCreatedMultipleTime = newScheduling.CanCreatedMultipleTime,
            });

            _schedulingProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, addScheduling);
        }


        /// <summary>
        /// Edit existing scheduling
        /// </summary>
        /// <remarks>
        /// Edit existing scheduling<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to edit/update an existing scheduling by its guid.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editScheduling">EditSchedulingViewModel</param>
        /// <param name="guid">Guid of activities whose scheduling needs to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "record already exist with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SchedulingViewModel))]
        [SwaggerRequestExample(typeof(EditSchedulingViewModel), typeof(EditSchedulingViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(SchedulingViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPut]
        public HttpResponseMessage Put(Guid guid, [FromBody]EditSchedulingViewModel editScheduling)
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

            var updatedScheduling = _schedulingProvider.Update(new SchedulingViewModel()
            {
                Guid = guid,
                ActivityId = editScheduling.ActivityId,
                ScheduledToBeCompleted = editScheduling.ScheduledToBeCompleted,
                ActivityAvailableForCreation = editScheduling.ActivityAvailableForCreation,
                RolesToCreateActivity = editScheduling.RolesToCreateActivity,
                RoleToCreateActivityRegardlessScheduled = editScheduling.RoleToCreateActivityRegardlessScheduled,
                OtherActivity = editScheduling.OtherActivity,
                OffsetCount = editScheduling.OffsetCount,
                OffsetType = editScheduling.OffsetType,
                SpecifiedActivity = editScheduling.SpecifiedActivity,
                CreationWindowOpens = editScheduling.CreationWindowOpens,
                CreationWindowClose = editScheduling.CreationWindowClose,
                IsScheduled = true,
                ScheduleDate = DateTime.UtcNow,
                CanCreatedMultipleTime=editScheduling.CanCreatedMultipleTime,
            });

            if (updatedScheduling == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Scheduling was not found.");
            }

            _schedulingProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, updatedScheduling);

        }

        /// <summary>
        /// Get activities scheduling by guid
        /// </summary>
        /// <remarks>
        /// Get activities scheduling by guid<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get activity scheduling based on its guid which will be fetched from SQL database.
        /// - This api returns SchedulingViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of an activities that needs to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SchedulingViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(SchedulingViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        [Route("api/v1/Scheduling/GetSchedulingByActivityId/{guid}")]
        public SchedulingViewModel GetSchedulingByActivityId(Guid guid)
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
            var checkList = _schedulingProvider.GetByActivityGuid(guid);
            return checkList;
        }
        /// <summary>
        /// Get all scheduled activities by project id
        /// </summary>
        /// <remarks>
        /// Get all scheduled activities by project id<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get all scheduled activities based on its project id which will be fetched from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of project for get all its scheduled activity</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<SchedulingViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllSchedulingViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        [Route("api/v1/Scheduling/GetAllScheduledActivityByProjectId/{guid}")]
        public IEnumerable<SchedulingViewModel> GetAllScheduledActivityByProjectId(Guid guid)
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
            return _schedulingProvider.GetAllScheduledActivityByProjectId(guid);
        }

        /// <summary>
        /// Currently not in use
        /// </summary>
        /// <remarks>
        /// Currently not in use
        /// </remarks>
        /// <param name="deployType">type</param>
        /// <param name="activitiesList">Guid list of Scheduled activities</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<Guid>))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPost]
        [Route("api/v1/Scheduling/PushScheduledActivities/{deployType}")]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage PushScheduledActivities(int deployType, [FromBody]List<Guid> activitiesList)
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
            if ((int)Core.Enum.ActivityDeploymentStatus.Pushed == deployType)
            {
                bool result = _schedulingProvider.PushScheduledActivities(activitiesList, (int)Core.Enum.ActivityDeploymentStatus.Pushed, this.LoggedInUserId);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            else if ((int)Core.Enum.ActivityDeploymentStatus.Deployed == deployType)
            {
                bool result = _schedulingProvider.PushScheduledActivities(activitiesList, (int)Core.Enum.ActivityDeploymentStatus.Deployed, this.LoggedInUserId);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
            else
            {
                bool result = _schedulingProvider.PushScheduledActivities(activitiesList, (int)Core.Enum.ActivityDeploymentStatus.Scheduled, this.LoggedInUserId);
                return Request.CreateResponse(HttpStatusCode.OK, result);
            }
        }

        /// <summary>
        /// Inactivate the activity
        /// </summary>
        /// <remarks>
        ///Inactivate the activity<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to inactivate the activity scheduling based on its guid.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        ///<param name="activityIdList">List of activity id to inactive</param>
        ///<param name="projectId">Guid of project</param>
        /////<param name="activityDeploymentStatus"></param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPost]
        [Route("api/v1/Scheduling/InactivateActivity/{projectId}")]
        public HttpResponseMessage InactivateActivity([FromBody]List<Guid> activityIdList, Guid projectId)
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
            bool result = _schedulingProvider.InactivateActivity(activityIdList, (int)Core.Enum.ActivityDeploymentStatus.Scheduled, this.LoggedInUserId, projectId);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}