using Aspree.Core.ViewModels;
using Aspree.Core.ViewModels.MongoViewModels;
using Aspree.Provider.Interface;
using Aspree.Provider.Interface.MongoProvider;
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
    /// Summary of an entity
    /// </summary>
    public class MongoDB_SummaryController : BaseController
    {        
        private readonly ISummaryProvider _summaryProvider;
        private readonly IUserLoginProvider _userLoginProvider;
        /// <summary>
        /// controller
        /// </summary>
        /// <param name="summaryProvider"></param>
        /// <param name="userLoginProvider"></param>
        public MongoDB_SummaryController(ISummaryProvider summaryProvider, IUserLoginProvider userLoginProvider)
        {
            this._summaryProvider = summaryProvider;
            this._userLoginProvider = userLoginProvider;
        }

        /// <summary>
        /// Get summary details by entityid and projectid
        /// </summary>
        /// <remarks>
        /// Get summary details by entityid and projectid<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get summary page details based on entity id and project id.
        /// - This api fetches entity basic details and list of activities to display on summary page.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>        
        /// <param name="projectId">Guid of project</param>
        ///<param name="entityId">Entity Number</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "This error is returned by the server when we pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching for the provided detail are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SummaryViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(SummaryViewModelExamples))]
        [Route("api/v1/MongoDB_Summary/GetSummaryDetails/{projectId}/{entityId}")]
        //[Route("api/v1/Test/MongoDB_Summary/GetSummaryDetails/{projectId}/{entityId}")]
        [HttpGet]
        public SummaryViewModel GetSummaryDetails(Guid? projectId, Int64 entityId)
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

            if (projectId == null)
            {
                GuidExceptionHandler();
            }
            Guid newprojectId = projectId.Value;

            string absolutePath = string.Empty;
            try { absolutePath = Request.RequestUri.Segments[3].ToLower(); } catch (Exception exc) { }

            if (absolutePath == "test/")
            {
                var checkList = _summaryProvider.TestEnvironment_GetSummaryDetails(newprojectId, entityId, this.LoggedInUserId);
                if (checkList == null)
                {
                    throw new Core.NotFoundException("Data not found.");
                }
                return checkList;
            }
            else
            {
                var checkList = _summaryProvider.GetSummaryDetails(newprojectId, entityId, this.LoggedInUserId);
                if (checkList == null)
                {
                    throw new Core.NotFoundException("Data not found.");
                }
                return checkList;
            }
        }

        /// <summary>
        /// Open dataentry form on summary page
        /// </summary>
        /// <remarks>
        /// Open dataentry form on summary page<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get form for data entry along with its variables and their respective values.
        /// - This api is used on summary page to open form for data entry.
        /// - The details will be fetched from the database for the form on the basis of entity number, guid of form, guid of activity, guid of project etc.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="entId">Entity id</param>
        /// <param name="formId">Guid of form</param>
        /// <param name="activityId">Guid of activity</param>
        /// <param name="projectId">Guid of project</param>
        /// <param name="p_Version">version</param>
        /// <param name="currentProjectId">Guid of loggedin project</param>
        /// <param name="summaryPageActivityId">Activity id added on summary page</param>
        [SwaggerResponse(HttpStatusCode.BadRequest,Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided detail are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormsMongo))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormsMongoExamples))]
        [Route("api/v1/MongoDB_Summary/GetSummaryPageForm/{entId}/{formId}/{activityId}/{projectId}/{p_Version}/{summaryPageActivityId}/{currentProjectId}")]
        //[Route("api/v1/Test/MongoDB_Summary/GetSummaryPageForm/{entId}/{formId}/{activityId}/{projectId}/{p_Version}/{summaryPageActivityId}")]
        [HttpGet]
        public FormsMongo GetSummaryPageForm(int entId, Guid? formId, Guid? activityId, Guid? projectId, int p_Version, string summaryPageActivityId, Guid? currentProjectId)
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

            if (formId == null || activityId == null || projectId == null)
            {
                GuidExceptionHandler();
            }
            Guid newformId = formId.Value;
            Guid newactivityId = activityId.Value;
            Guid newprojectId = projectId.Value;
            Guid newcurrentProjectId = currentProjectId.Value;

            string absolutePath = string.Empty;
            try
            {
                absolutePath = Request.RequestUri.Segments[3].ToLower();
                WriteLog("API.FormController URL=" + string.Join(",", Request.RequestUri.Segments));
            }
            catch (Exception exc) { }

            if (absolutePath == "test/")
            {
                return _summaryProvider.TestEnvironment_GetSummaryPageForm(entId, newformId, newactivityId, newprojectId, p_Version, summaryPageActivityId, this.LoggedInUserId, newcurrentProjectId);
            }
            else
            {
                return _summaryProvider.GetSummaryPageForm(entId, newformId, newactivityId, newprojectId, p_Version, summaryPageActivityId, this.LoggedInUserId, newcurrentProjectId);
            }
        }

        /// <summary>
        /// Add activities on summary page
        /// </summary>
        /// <remarks>
        /// Add activities on summary page<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to add activities on summary page associated to an entity.
        /// - Activities added on summary page will be saved in Mongo db with its respective entity id.
        /// - These activities consists of forms for data entry with their respective entities.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newActivity">AddActivities Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "This error is returned by the server when we pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SummaryPageActivityViewModel))]
        [SwaggerRequestExample(typeof(SummaryPageActivityViewModel), typeof(SummaryPageActivityViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(SummaryPageActivityViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPost]
        [Route("api/v1/MongoDB_Summary/AddSummaryPageActivity")]
        public HttpResponseMessage AddSummaryPageActivity([FromBody]SummaryPageActivityViewModel newActivity)
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
                var addedVariable = _summaryProvider.TestEnvironment_AddSummaryPageActivity(new SummaryPageActivityViewModel()
                {
                    ActivityCompletedByGuid = newActivity.ActivityCompletedByGuid,
                    PersonEntityId = Convert.ToInt64(newActivity.PersonEntityId),
                    ProjectGuid = newActivity.ProjectGuid,
                    ActivityGuid = newActivity.ActivityGuid,
                    CreatedDate = DateTime.UtcNow,
                    ActivityDate = newActivity.ActivityDate,
                }, this.LoggedInUserId);
                return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
            }
            else
            {
                WriteLog("API.ActivityController URL=" + Request.RequestUri);

                var addedVariable = _summaryProvider.AddSummaryPageActivity(new SummaryPageActivityViewModel()
                {
                    ActivityCompletedByGuid = newActivity.ActivityCompletedByGuid,
                    PersonEntityId = Convert.ToInt64(newActivity.PersonEntityId),
                    ProjectGuid = newActivity.ProjectGuid,
                    ActivityGuid = newActivity.ActivityGuid,
                    CreatedDate = DateTime.UtcNow,
                    ActivityDate = newActivity.ActivityDate,
                }, this.LoggedInUserId);
                return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
            }
        }

        /// <summary>
        /// Edit activity on summary page
        /// </summary>
        /// <remarks>
        /// Edit activity on summary page<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to edit an existing activity on summary page.
        /// - Using this api, we can change "activity date" and "activity completed by" field.
        /// - The updated "summary page activity" will get stored in MongoDB for respective entity.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="id">id of summary page activity</param>
        /// <param name="editActivity">editActivities Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "This error is returned by the server when we pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided id are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SummaryPageActivityViewModel))]
        [SwaggerRequestExample(typeof(SummaryPageActivityViewModel), typeof(SummaryPageActivityViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(SummaryPageActivityViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPut]
        [Route("api/v1/MongoDB_Summary/EditSummaryPageActivity/{id}")]
        //[Route("api/v1/Test/MongoDB_Summary/EditSummaryPageActivity/{id}")]
        public HttpResponseMessage EditSummaryPageActivity(string id, [FromBody]SummaryPageActivityViewModel editActivity)
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
                var addedVariable = _summaryProvider.TestEnvironment_EditSummaryPageActivity(new SummaryPageActivityViewModel()
                {
                    Id = new MongoDB.Bson.ObjectId(id),
                    ActivityCompletedByGuid = editActivity.ActivityCompletedByGuid,
                    PersonEntityId = Convert.ToInt64(editActivity.PersonEntityId),
                    ProjectGuid = editActivity.ProjectGuid,
                    ActivityGuid = editActivity.ActivityGuid,
                    CreatedDate = DateTime.UtcNow,
                    ActivityDate = editActivity.ActivityDate,
                }, this.LoggedInUserId);
                return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
            }
            else
            {
                WriteLog("API.ActivityController URL=" + Request.RequestUri);

                var addedVariable = _summaryProvider.EditSummaryPageActivity(new SummaryPageActivityViewModel()
                {
                    Id = new MongoDB.Bson.ObjectId(id),
                    ActivityCompletedByGuid = editActivity.ActivityCompletedByGuid,
                    PersonEntityId = Convert.ToInt64(editActivity.PersonEntityId),
                    ProjectGuid = editActivity.ProjectGuid,
                    ActivityGuid = editActivity.ActivityGuid,
                    CreatedDate = DateTime.UtcNow,
                    ActivityDate = editActivity.ActivityDate,
                }, this.LoggedInUserId);
                return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
            }
        }
        /// <summary>
        /// Delete activities of summary page
        /// </summary>
        /// <remarks>
        /// Delete activities of summary page<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to delete summary page activity by its id.
        /// - The api data will be soft deleted form Mongo database and then we will update the "DateDeactivated", "DeactivatedBy" value.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="id">id of summary page activity</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "This error is returned by the server when we pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided id are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SummaryPageActivityViewModel))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SummaryPageActivityViewModelExamples))]
        [HttpDelete]
        [Route("api/v1/MongoDB_Summary/DeleteSummaryPageActivity/{id}")]
        public HttpResponseMessage DeleteSummaryPageActivity(string id)
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
                var addedVariable = _summaryProvider.TestEnvironment_DeleteSummaryPageActivity(id, this.LoggedInUserId);
                return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
            }
            else
            {
                WriteLog("API.ActivityController URL=" + Request.RequestUri);

                var addedVariable = _summaryProvider.DeleteSummaryPageActivity(id, this.LoggedInUserId);
                return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
            }
        }

        /// <summary>
        /// Get summary details by entityid and projectid
        /// </summary>
        /// <remarks>
        /// Get summary details by entityid and projectid<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get summary page details based on entity id and project id.
        /// - This api fetches entity basic details and list of activities to display on summary page.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>        
        /// <param name="projectId">Guid of project</param>
        ///<param name="entityId">Entity Number</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "This error is returned by the server when we pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching for the provided detail are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SummaryPageLeftPanelViewModel))]
        //[SwaggerResponseExample(HttpStatusCode.OK, typeof(SummaryViewModelExamples))]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/v1/MongoDB_Summary/UpdateLeftPanelSummaryPage/{projectId}/{entityId}")]
        [Route("api/v1/Test/MongoDB_Summary/UpdateLeftPanelSummaryPage/{projectId}/{entityId}")]
        [HttpGet]
        public SummaryPageLeftPanelViewModel UpdateLeftPanelSummaryPage(Guid? projectId, Int64 entityId)
        {
            #region check login
            if (Request.Headers.Authorization == null)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login.")
                };
                throw new HttpResponseException(msg);
            }

            if (this.LoggedInUserId == Guid.Empty)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login."),
                });
            }
            #endregion

            if (projectId == null)
            {
                GuidExceptionHandler();
            }
            Guid newprojectId = projectId.Value;

            string absolutePath = string.Empty;
            try { absolutePath = Request.RequestUri.Segments[3].ToLower(); } catch (Exception exc) { }

            if (absolutePath == "test/")
            {
                var checkList = _summaryProvider.UpdateLeftPanelSummaryPage(newprojectId, entityId, true);
                return checkList;
            }
            else
            {
                var checkList = _summaryProvider.UpdateLeftPanelSummaryPage(newprojectId, entityId);
                return checkList;
            }
        }

    }
}