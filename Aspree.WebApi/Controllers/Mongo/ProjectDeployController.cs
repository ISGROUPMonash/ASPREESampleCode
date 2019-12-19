using Aspree.Core.ViewModels;
using Aspree.Core.ViewModels.MongoViewModels;
using Aspree.Provider.Interface.MongoProvider;
using Swashbuckle.Examples;
using Aspree.WebApi.Utilities;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Aspree.WebApi.Controllers.Mongo
{
    /// <summary>
    /// deployment of project
    /// </summary>
    public class ProjectDeployController : BaseController
    {
        private readonly IProjectDeployProvider _projectDeployProvider;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectDeployProvider"></param>
        public ProjectDeployController(IProjectDeployProvider projectDeployProvider)
        {
            this._projectDeployProvider = projectDeployProvider;
        }

        /// <summary>
        /// Deploy active project by project id
        /// </summary>
        /// <remarks>
        /// Deploy active project by project id<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to deploy project activities by project id.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="projectId">Project id</param>
        /// <param name="activitiesList">List of Activities to deploy</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "record  already exist with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectDeployViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ProjectDeployViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/v1/ProjectDeploy/DeployProject/{projectId}")]
        public HttpResponseMessage DeployProject(Guid? projectId, [FromBody]List<Guid> activitiesList)
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

            if (!ModelState.IsValid)
            {
                //return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.AllErrors());
            }
            var addedVariable = _projectDeployProvider.Create(newprojectId, activitiesList, (int)Core.Enum.ActivityDeploymentStatus.Deployed);
            return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
        }


        /// <summary>
        /// Get project from mongo db by project guid
        /// </summary>
        /// <remarks>
        /// Get project from mongo db by project guid<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get project by project id from Mongo database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="projectId">Guid of a project that need to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectDeployViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ProjectDeployViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/v1/ProjectDeploy/GetProjectMongo/{projectId}")]
        //[System.Web.Http.Route("api/v1/Test/ProjectDeploy/GetProjectMongo/{projectId}")]
        public ProjectDeployViewModel GetProjectMongo(Guid? projectId)
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

            var absolutePath = "";
            try { absolutePath = Request.RequestUri.Segments[3].ToLower(); } catch (Exception exc) { }
            if (absolutePath == "test/")
            {
                var checkList = _projectDeployProvider.TestEnvironment_GetProjectByGuid(newprojectId);
                if (checkList == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("Project was not found."),
                    });
                }
                return checkList;
            }
            else
            {
                var checkList = _projectDeployProvider.GetProjectByGuid(newprojectId);
                if (checkList == null)
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        Content = new StringContent("Project was not found."),
                    });
                }
                return checkList;
            }
        }

        /// <summary>
        /// Get all deployed projects
        /// </summary>
        /// <remarks>
        /// Get all deployed projects<br></br>
        /// <strong>Purpose.</strong>
        /// - The purpose of this api is to get all deployed projects from Mongo Database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<Core.ViewModels.FormDataEntryProjectsViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllFormDataEntryProjectsViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [System.Web.Http.Route("api/v1/ProjectDeploy/GetAllDeployedProject/")]
        [System.Web.Http.HttpGet]
        public IEnumerable<Core.ViewModels.FormDataEntryProjectsViewModel> GetAllDeployedProject()
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

            var absolutePath = "";
            absolutePath = Request.RequestUri.Segments[3].ToLower();
            if (absolutePath == "test/")
                return _projectDeployProvider.TestEnvironment_GetAllDeployedProject(this.LoggedInUserId);
            else
                return _projectDeployProvider.GetAllDeployedProject(this.LoggedInUserId);
        }

        /// <summary>
        /// Push activities to test environment
        /// </summary>
        /// <remarks>
        /// Push activities to test environment<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to Push activities to test environment.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="projectId">Project id</param>
        /// <param name="activitiesList">Activities to push</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "record  already exist with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectDeployViewModel))]
        //[SwaggerRequestExample(typeof(List<Guid>), typeof(List<Guid>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ProjectDeployViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/v1/ProjectDeploy/PushTestProject/{projectId}")]
        public HttpResponseMessage PushTestProject(Guid? projectId, [FromBody]List<Guid> activitiesList)
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

            if (!ModelState.IsValid)
            {
                //return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.AllErrors());
            }
            var addedVariable = _projectDeployProvider.CreateTestProject(newprojectId, activitiesList, (int)Core.Enum.ActivityDeploymentStatus.Deployed);
            return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
        }

        /// <summary>
        /// Check project linked by entityId
        /// </summary>
        /// <remarks>
        /// Check project linked by entityId<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to check project linked with entity.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="projectId">Guid of a project</param>
        /// <param name="entityId">Entity id</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectStaffMemberRoleViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ProjectStaffMemberRoleViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/v1/ProjectDeploy/CheckEntityLinkedProject/{projectId}/{entityId}")]
        public ProjectStaffMemberRoleViewModel CheckEntityLinkedProject(Guid? projectId, int entityId)
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

            var absolutePath = "";
            try { absolutePath = Request.RequestUri.Segments[3].ToLower(); } catch (Exception exc) { }
            if (absolutePath == "test/")
            {
                var checkList = _projectDeployProvider.TestEnvironment_CheckEntityLinkedProject(newprojectId, entityId);
                if (checkList == null)
                {
                    return null;
                }
                return checkList;
            }
            else
            {
                var checkList = _projectDeployProvider.CheckEntityLinkedProject(newprojectId, entityId);
                if (checkList == null)
                {
                    return null;
                }
                return checkList;
            }
        }
    }
}