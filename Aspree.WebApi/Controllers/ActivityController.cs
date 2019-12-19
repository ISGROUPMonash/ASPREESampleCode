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
    public class ActivityController : BaseController
    {
        private readonly IActivityProvider _activityProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityProvider"></param>
        public ActivityController(IActivityProvider activityProvider)
        {
            _activityProvider = activityProvider;
        }
    /// <summary>
        /// Get all activities
        /// </summary>
        /// <remarks>
        /// Get all activities<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get all activities along with its associated forms.
        /// - This api fetches activities from SQL database.<br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <returns></returns>
        //[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the request")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<ActivityViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllActivityViewModelExamples))]
        [HttpGet]
        public IEnumerable<ActivityViewModel> GetAll()
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
            return _activityProvider.GetAll(this.LoggedInUserTenantId);
        }

        /// <summary>
        /// Get activity by guid
        /// </summary>
        /// <remarks>
        /// Get activity<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get an activity based on its guid from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of an Activities that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No record matching the provided guid is found")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ActivityViewModelExamples))]
        [HttpGet]
        public ActivityViewModel Get(Guid guid)
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

            var checkList = _activityProvider.GetByGuid(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Activity was not found."),
                });
            }

            return checkList;
        }


        /// <summary>
        /// Add new activity
        /// </summary>
        /// <remarks>
        /// Add new activity<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to create new activity into the system.
        /// - Activity will be saved with its associated project id.
        /// - The created activity will be saved in SQL database.<br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newActivity">New Activities Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityViewModel))]

        [SwaggerRequestExample(typeof(NewActivityViewModel), typeof(NewActivityViewModelExamples))] 
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ActivityViewModelExamples))]
        [HttpPost]
        public HttpResponseMessage Post([FromBody]NewActivityViewModel newActivity)
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
            newActivity.RepeatationType = (int)Core.Enum.RepeatationTypes.Daily;

            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.AllErrors());
            }

            var addedVariable = _activityProvider.Create(new ActivityViewModel()
            {
                CreatedBy = this.LoggedInUserId,
                CreatedDate = DateTime.UtcNow,
                ActivityCategoryId = newActivity.ActivityCategoryId,
                ActivityName = newActivity.ActivityName,
                ActivityStatusId = newActivity.ActivityStatusId,
                EndDate = newActivity.EndDate,
                DependentActivityId = newActivity.DependentActivityId,
                RepeatationCount = newActivity.RepeatationCount,
                RepeatationOffset = newActivity.RepeatationOffset,
                RepeatationType = newActivity.RepeatationType,
                ScheduleType = newActivity.ScheduleType,
                StartDate = newActivity.StartDate,
                TenantId = this.LoggedInUserTenantId,
                Forms = newActivity.Forms,
                ActivityRoles = newActivity.ActivityRoles,
                EntityTypes = newActivity.EntityTypes,
                ProjectId = newActivity.ProjectId,
                IsActivityRequireAnEntity = newActivity.IsActivityRequireAnEntity,
            });

            _activityProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
        }


        /// <summary>
        /// Edit existing activities
        /// </summary>
        /// <remarks>
        /// Edit existing activities<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to edit an existing activity by its guid.<br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editActivity">Edit Activities Model</param>
        /// <param name="guid">guid of Activities that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided GUID are found")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityViewModel))]
        [SwaggerRequestExample(typeof(EditActivityViewModel), typeof(EditActivityViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ActivityViewModelExamples))]
        //[SwaggerResponseExample(HttpStatusCode.OK, typeof(EditActivityViewModelExamples))]
        [HttpPut]
        public HttpResponseMessage Put(Guid guid, [FromBody]EditActivityViewModel editActivity)
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
            editActivity.RepeatationType = (int)Core.Enum.RepeatationTypes.Daily;
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.AllErrors());
            }

            var updatedVariable = _activityProvider.Update(new ActivityViewModel()
            {
                ModifiedBy = LoggedInUserId,
                ModifiedDate = DateTime.UtcNow,
                ActivityCategoryId = editActivity.ActivityCategoryId,
                ActivityName = editActivity.ActivityName,
                ActivityStatusId = editActivity.ActivityStatusId,
                EndDate = editActivity.EndDate,
                DependentActivityId = editActivity.DependentActivityId,
                RepeatationCount = editActivity.RepeatationCount,
                RepeatationOffset = editActivity.RepeatationOffset,
                RepeatationType = editActivity.RepeatationType,
                ScheduleType = editActivity.ScheduleType,
                StartDate = editActivity.StartDate,
                TenantId = this.LoggedInUserTenantId,
                Guid = guid,
                Forms = editActivity.Forms,
                ActivityRoles = editActivity.ActivityRoles,
                EntityTypes = editActivity.EntityTypes,
                ProjectId = editActivity.ProjectId,

                IsActivityRequireAnEntity = editActivity.IsActivityRequireAnEntity,
            });

            if (updatedVariable == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Activity was not found.");
            }

            _activityProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, updatedVariable);

        }


        /// <summary>
        /// Delete existing activity
        /// </summary>
        /// <remarks>
        /// Delete existing activity<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to delete an existing activity by its guid.
        /// - The api data will be soft deleted form SQL table(Activities).
        /// - When activity is soft deleted, the column "DateDeactivated" is updated in SQL database.
        /// - Link for forms and variables for the associated activity is deleted from the SQL database table(ActivityForm)
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of Activities that is to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided GUID are found")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ActivityViewModelExamples))]
        [HttpDelete]
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
            var deletedCategory = _activityProvider.DeleteByGuid(guid, LoggedInUserId);
            _activityProvider.SaveChanges();

            if (deletedCategory != null)
                return Request.CreateResponse(HttpStatusCode.OK, deletedCategory);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Activity was not found.");
        }


        /// <summary>
        /// Get project builder activities
        /// </summary>
        /// <remarks>
        /// 
        ///Get project builder activities<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get all existing activities, forms, roles and entity types.
        /// - Details of activity page will be fetched from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="projectId">Guid of a project that activities to be fetched</param>
        //[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectBuilderActivityViewModel))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectBuilderActivityViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ProjectBuilderActivityViewModelExamples))]

        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]

        [Route("api/v1/Activity/ProjectBuilderActivities/{projectId}")]
        [HttpGet]
        public ProjectBuilderActivityViewModel ProjectBuilderActivities(Guid projectId)
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
            return _activityProvider.GetProjectBuilderActivities(this.LoggedInUserTenantId, this.LoggedInUserId, projectId);
        }

 
        #region Currently not in used api's


        /// <summary>
        /// Currently not in use
        /// </summary>
        /// <remarks>
        /// Currently not in use
        /// </remarks>
        /// <param name="scheduleActivity">Schedule Activities Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPost]
        [Route("api/v1/Activity/ScheduleActivityList")]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage ScheduleActivityList([FromBody]NewScheduleActivityViewModel scheduleActivity)
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
            //editActivity.RepeatationType = (int)Core.Enum.RepeatationTypes.Daily;
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.AllErrors());
            }

            var updatedVariable = _activityProvider.UpdateActivityScheduling(new ScheduleActivityViewModel()
            {
                ModifiedBy = LoggedInUserId,
                ModifiedDate = DateTime.UtcNow,
                ScheduleActivityList = scheduleActivity.ScheduleActivityList,
                TenantId = this.LoggedInUserTenantId,
                //Guid = guid,
            });

            if (updatedVariable == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Activity was not found.");
            }

            _activityProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, updatedVariable);
        }




        /// <summary>
        /// Currently not in use
        /// </summary>
        /// <remarks>
        /// Currently not in use
        /// </remarks>
        /// <param name="guid">Guid of Activities that is to be Remove Scheduled Activity</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/v1/Activity/RemoveScheduledActivity/{guid}")]
        public HttpResponseMessage RemoveScheduledActivity(Guid guid)
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
            var deletedCategory = _activityProvider.RemoveScheduledActivity(guid, LoggedInUserId);
            _activityProvider.SaveChanges();

            if (deletedCategory != null)
                return Request.CreateResponse(HttpStatusCode.OK, deletedCategory);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Activity was not found.");
        }





        /// <summary>
        /// Currently not in use
        /// </summary>
        /// <remarks>
        /// Currently not in use
        /// </remarks>
        /// <param name="guid">Guid of Activities that is to be Save Preview Scheduled Activity</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        [Route("api/v1/Activity/SavePreviewScheduledActivity/{guid}")]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage SavePreviewScheduledActivity(string guid)
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
            var saveedPreview = _activityProvider.SavePreviewScheduledActivity(guid, LoggedInUserId);
            _activityProvider.SaveChanges();

            if (saveedPreview != null)
                return Request.CreateResponse(HttpStatusCode.OK, saveedPreview);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Activity was not found.");
        }


        /// <summary>
        /// Schedule an New Activities
        /// </summary>
        /// <remarks>
        /// Schedule an New Activities
        /// </remarks>
        /// <param name="scheduleActivity">Schedule Activities Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPost]
        [Route("api/v1/Activity/ScheduleActivityList_New")]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage ScheduleActivityList_New([FromBody]NewScheduleActivityViewModel scheduleActivity)
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
            //editActivity.RepeatationType = (int)Core.Enum.RepeatationTypes.Daily;
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.AllErrors());
            }

            var updatedVariable = _activityProvider.ScheduleActivity_New(new NewScheduleActivityViewModel()
            {
                ModifiedBy = LoggedInUserId,
                ModifiedDate = DateTime.UtcNow,
                ActivitySchedulingViewModel = scheduleActivity.ActivitySchedulingViewModel,
                TenantId = this.LoggedInUserTenantId,
                //Guid = guid,
            });

            if (updatedVariable == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Activity was not found.");
            }

            _activityProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, updatedVariable);
        }


        /// <summary>
        /// Get all activity scheduling details
        /// </summary>
        /// <remarks>
        /// Get all activity scheduling details
        /// </remarks>
        /// <param name="activityId">Guid of an Activity that scheduling details to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<SchedulingViewModel>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [Route("api/v1/Activity/GetAllActivityScheduling")]
        [HttpGet]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public IEnumerable<SchedulingViewModel> GetAllActivityScheduling(Guid activityId)
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
            return _activityProvider.GetAllActivityScheduling(activityId);
        }

        #endregion
        /// <summary>
        /// Add activities on summary page
        /// </summary>
        ///  <remarks>
        ///Add activities on summary page<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to add an activity on summary page.
        /// - New activity will be saved in SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newActivity">AddActivities Model</param>
        /// 
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(NewAddSummaryPageActivityViewModel))]
        [SwaggerRequestExample(typeof(NewAddSummaryPageActivityViewModel), typeof(NewAddSummaryPageActivityViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(AddSummaryPageActivityViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        //[SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPost]
        [Route("api/v1/Activity/AddSummaryPageActivity")]
        public HttpResponseMessage AddSummaryPageActivity([FromBody]NewAddSummaryPageActivityViewModel newActivity)
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

            WriteLog("API.ActivityController URL=" + Request.RequestUri);
            var absolutePath = "";
            absolutePath = (Request.RequestUri.Segments.Count() > 3 ? Request.RequestUri.Segments[3].ToLower() : string.Empty);
            if (absolutePath == "test/")
            {
                WriteLog("API.ActivityController URL=" + Request.RequestUri);
                var addedVariable = _activityProvider.TestEnvironment_AddSummaryPageActivity(new AddSummaryPageActivityViewModel()
                {
                    CreatedBy = this.LoggedInUserId,
                    CreatedDate = DateTime.UtcNow,
                    ActivityId = newActivity.ActivityId,
                    ActivityCompletedByUser = newActivity.ActivityCompletedByUser,
                    ActivityDate = newActivity.ActivityDate,
                    IsActivityAdded = newActivity.IsActivityAdded,
                    ProjectId = newActivity.ProjectId,
                    PersonEntityId = newActivity.PersonEntityId,
                });
                return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
            }
            else
            {
                WriteLog("API.ActivityController URL=" + Request.RequestUri);

                var addedVariable = _activityProvider.AddSummaryPageActivity(new AddSummaryPageActivityViewModel()
                {
                    CreatedBy = this.LoggedInUserId,
                    CreatedDate = DateTime.UtcNow,
                    ActivityId = newActivity.ActivityId,
                    ActivityCompletedByUser = newActivity.ActivityCompletedByUser,
                    ActivityDate = newActivity.ActivityDate,
                    IsActivityAdded = newActivity.IsActivityAdded,
                    ProjectId = newActivity.ProjectId,
                    PersonEntityId = newActivity.PersonEntityId,
                });
                return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
            }
        }

        /// <summary>
        /// Get all summary page activities of an entity
        /// </summary>
        /// <remarks>
        /// Get all summary page activities of an entity<br></br>
        /// <strong>Purpose.</strong>
        /// - The purpose of this api is to get the list of  all activities on summary page.
        /// - This api fetches all activities from SQL database on summary page based on entityId and projectId.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="entId">Entity id</param>
        /// <param name="projectId">Project id</param>
        //[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]        
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<AddSummaryPageActivityViewModel>))]
        [SwaggerRequestExample(typeof(AddSummaryPageActivityViewModel), typeof(AddSummaryPageActivityViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllAddSummaryPageActivityViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        [Route("api/v1/Activity/GetAllSummaryPageActivity/{entId}/{projectId}")]
        public IEnumerable<AddSummaryPageActivityViewModel> GetAllSummaryPageActivity(string entId, Guid projectId)
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
            var absolutePath = string.Empty;
            WriteLog("API.ActivityController URL=" + Request.RequestUri);
            absolutePath = (Request.RequestUri.Segments.Count() > 3 ? Request.RequestUri.Segments[3].ToLower() : string.Empty);

            if (absolutePath == "test/")
            {
                WriteLog("API.ActivityController URL=" + Request.RequestUri);
                return _activityProvider.TestEnvironment_GetAllSummaryPageActivity(entId, projectId);
            }
            else
            {
                WriteLog("API.ActivityController URL=" + Request.RequestUri);
                return _activityProvider.GetAllSummaryPageActivity(entId, projectId, this.LoggedInUserId);
            }
        }
        /// <summary>
        /// Edit activity on summary page
        /// </summary>
        /// <remarks>
        /// Edit activity on summary page<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to edit/update the existing activity on summary page.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editSummarypageActivity">edit summarypageActivity Model</param>
        /// 
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(NewAddSummaryPageActivityViewModel))]
        [SwaggerRequestExample(typeof(NewAddSummaryPageActivityViewModel), typeof(NewAddSummaryPageActivityViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(AddSummaryPageActivityViewModelExamples))]
        //[SwaggerResponseExample(HttpStatusCode.OK, typeof(IEnumerable<NewAddSummaryPageActivityViewModelExamples>))]
        //[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]       
        //[SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPost]
        [Route("api/v1/Activity/EditSummaryPageActivity")]
        public HttpResponseMessage EditSummaryPageActivity([FromBody]NewAddSummaryPageActivityViewModel editSummarypageActivity)
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

            WriteLog("API.ActivityController URL=" + Request.RequestUri);
            var absolutePath = "";
            absolutePath = (Request.RequestUri.Segments.Count() > 3 ? Request.RequestUri.Segments[3].ToLower() : string.Empty);
            if (absolutePath == "test/")
            {
                WriteLog("API.ActivityController URL=" + Request.RequestUri);
                var addedVariable = _activityProvider.TestEnvironment_AddSummaryPageActivity(new AddSummaryPageActivityViewModel()
                {
                    Id = editSummarypageActivity.Id,
                    CreatedBy = this.LoggedInUserId,
                    CreatedDate = DateTime.UtcNow,
                    ActivityId = editSummarypageActivity.ActivityId,
                    ActivityCompletedByUser = editSummarypageActivity.ActivityCompletedByUser,
                    ActivityDate = editSummarypageActivity.ActivityDate,
                    IsActivityAdded = editSummarypageActivity.IsActivityAdded,
                    ProjectId = editSummarypageActivity.ProjectId,
                    PersonEntityId = editSummarypageActivity.PersonEntityId,
                });
                return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
            }
            else
            {
                WriteLog("API.ActivityController URL=" + Request.RequestUri);

                var addedVariable = _activityProvider.EditSummaryPageActivity(new AddSummaryPageActivityViewModel()
                {
                    Id = editSummarypageActivity.Id,
                    CreatedBy = this.LoggedInUserId,
                    CreatedDate = DateTime.UtcNow,
                    ActivityId = editSummarypageActivity.ActivityId,
                    ActivityCompletedByUser = editSummarypageActivity.ActivityCompletedByUser,
                    ActivityDate = editSummarypageActivity.ActivityDate,
                    IsActivityAdded = editSummarypageActivity.IsActivityAdded,
                    ProjectId = editSummarypageActivity.ProjectId,
                    PersonEntityId = editSummarypageActivity.PersonEntityId,
                });
                return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
            }
        }

        /// <summary>
        /// Delete activities on summary page
        /// </summary>
        /// <remarks>
        /// Delete activities on summary page<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to delete activity added on summary page.
        /// - This api will delete data from SQL table.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
         /// </remarks>
        /// <param name="id">id of summary page activity</param>
        //[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]        
        //[SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(AddSummaryPageActivityViewModel))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(AddSummaryPageActivityViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(NewAddSummaryPageActivityViewModel))]
        //[SwaggerRequestExample(typeof(NewAddSummaryPageActivityViewModelExamples), typeof(NewAddSummaryPageActivityViewModelExamples))]
        //[SwaggerResponseExample(HttpStatusCode.OK, typeof(IEnumerable<NewAddSummaryPageActivityViewModelExamples>))]

        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpDelete]
        [Route("api/v1/Activity/DeleteSummaryPageActivity/{id}")]
        public HttpResponseMessage DeleteSummaryPageActivity(int id)
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
            WriteLog("API.ActivityController URL=" + Request.RequestUri);
            var absolutePath = "";
            absolutePath = (Request.RequestUri.Segments.Count() > 3 ? Request.RequestUri.Segments[3].ToLower() : string.Empty);
            if (absolutePath == "test/")
            {
                WriteLog("API.ActivityController URL=" + Request.RequestUri);
                var addedVariable = _activityProvider.TestEnvironment_DeleteSummaryPageActivity(id, this.LoggedInUserId);
                return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
            }
            else
            {
                WriteLog("API.ActivityController URL=" + Request.RequestUri);

                var addedVariable = _activityProvider.DeleteSummaryPageActivity(id, this.LoggedInUserId);
                return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
            }
        }

        /// <summary>
        /// Get activity by guid and projct id and userloginid
        /// </summary>
        /// <remarks>
        /// Get activity<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get an activity based on its guid from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of an Activities that to be fetched</param>
        /// <param name="projectid">project id of an Activities that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No record matching the provided guid is found")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ActivityViewModelExamples))]
        [HttpGet]
        [Route("api/v1/Activity/Getactivity/{guid}/{projectid}")]
        public ActivityViewModel Getactivity(Guid guid, Guid projectid)
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
            var checkList = _activityProvider.GetActivityByGuid(guid, this.LoggedInUserId, projectid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Activity was not found."),
                });
            }

            return checkList;
        }

    }
}
