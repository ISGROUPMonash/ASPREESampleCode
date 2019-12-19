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
    public class ProjectController : BaseController
    {
        private readonly IProjectProvider _projectProvider;
        private readonly Provider.Interface.MongoProvider.IProjectDeployProvider _projectDeployProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectProvider"></param>
        public ProjectController(IProjectProvider projectProvider, Provider.Interface.MongoProvider.IProjectDeployProvider projectDeployProvider)
        {
            _projectProvider = projectProvider;
            _projectDeployProvider = projectDeployProvider;
        }

        /// <summary>
        /// Get All Projects
        /// </summary>
        /// <remarks>
        /// Get All Projects
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<ProjectViewModel>))]
       
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public IEnumerable<ProjectViewModel> GetAll()
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
            return _projectProvider.GetAll(this.LoggedInUserTenantId);
        }

        /// <summary>
        /// Get project by guid
        /// </summary>
        /// <remarks>
        /// Get all projects by guid<br></br>
        /// <strong>Purpose.</strong>
        /// - The api provides the functionality to get project based on guid from SQL database.
        /// - This api returns FormDataEntryProjectsViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of a project that needs to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormDataEntryProjectsViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormDataEntryProjectsViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        public FormDataEntryProjectsViewModel Get(Guid guid)
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
            var checkList = _projectProvider.GetProjectByGuid_New(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Project was not found."),
                });
            }

            return checkList;
        }


        /// <summary>
        /// Add New Project
        /// </summary>
        /// <remarks>
        /// Add New Project
        /// </remarks>
        /// <param name="newProject">New Project Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectViewModel))]
        //[SwaggerRequestExample(typeof(NewProjectViewModel), typeof(NewProjectViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPost]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage Post([FromBody]NewProjectViewModel newProject)
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

            var addedProject = _projectProvider.Create(new ProjectViewModel()
            {
                ProjectName = newProject.ProjectName,
                CreatedBy = LoggedInUserId,
                CheckListID = newProject.CheckListID,
                EndDate = newProject.EndDate,
                PreviousProjectId = newProject.PreviousProjectId,
                ProjectStatusId = (int)Core.Enum.ProjectStatusTypes.Draft,
                State = (int)Core.Enum.ProjectStatusTypes.Draft,
                StartDate = newProject.StartDate,
                Version = newProject.Version,
                ProjectUrl = newProject.ProjectUrl,
                TenantId = newProject.TenantId,
                RoleId = newProject.RoleId,
                RoleGuid = newProject.RoleGuid,
                ProjectUserId = newProject.ProjectUserId,
                ProjectStaffMembersRoles = newProject.ProjectStaffMembersRoles,
            });

            _projectProvider.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, addedProject);
        }


        /// <summary>
        /// Edit Existing Project
        /// </summary>
        /// <remarks>
        /// Edit Existing Project
        /// </remarks>
        /// <param name="editProject">Edit Project Model</param>
        /// <param name="guid">guid of Project that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectViewModel))]
        //[SwaggerRequestExample(typeof(EditProjectViewModel), typeof(EditProjectViewModelExamples))]
        
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPut]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage Put(Guid guid, [FromBody]EditProjectViewModel editProject)
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

            var updatedProject = _projectProvider.Update(new ProjectViewModel()
            {
                ModifiedBy = LoggedInUserId,
                ModifiedDate = DateTime.UtcNow,
                Guid = guid,
                ProjectName = editProject.ProjectName,
                CheckListID = editProject.CheckListID,
                EndDate = editProject.EndDate,
                PreviousProjectId = editProject.PreviousProjectId,
                StartDate = editProject.StartDate,
                Version = editProject.Version,
                ProjectUrl = editProject.ProjectUrl,
                ProjectUserId = editProject.ProjectUserId,
                ProjectStaffMembersRoles = editProject.ProjectStaffMembersRoles,
            });

            if (updatedProject == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Project was not found.");
            }

            _projectProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, updatedProject);

        }


        /// <summary>
        /// Delete Existing Project
        /// </summary>
        /// <remarks>
        /// Delete Existing Project
        /// </remarks>
        /// <param name="guid">Guid of Project that is to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectViewModel))]
        
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpDelete]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
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
            var deletedCategory = _projectProvider.DeleteByGuid(guid, LoggedInUserId);
            _projectProvider.SaveChanges();

            if (deletedCategory != null)
                return Request.CreateResponse(HttpStatusCode.OK, deletedCategory);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Project was not found.");
        }



        /// <summary>
        /// Currently not in use
        /// </summary>
        /// <remarks>
        /// Currently not in use 
        /// </remarks>
        /// <param name="projectViewModel">Project Model</param>
        /// <param name="guid">guid of Project that has to be publish</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [Route("api/v1/Project/PublishProject/{guid}")]
        [HttpPut]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage PublishProject(Guid guid, [FromBody]EditProjectViewModel projectViewModel)
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
            }

            var updatedProject = _projectProvider.PublishProject(new ProjectViewModel()
            {
                ModifiedBy = LoggedInUserId,
                ModifiedDate = DateTime.UtcNow,
                Guid = guid,
                ProjectUrl = projectViewModel.ProjectUrl,
                ProjectStatusId = (int)Core.Enum.ProjectStatusTypes.Published,
                State = (int)Core.Enum.ProjectStatusTypes.Published,
            });

            if (updatedProject == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Project was not found.");
            }

            _projectProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, updatedProject);
        }
        
        /// <summary>
        /// Get all projects by userid
        /// </summary>
        /// <remarks>
        /// Get all projects by userid<br></br>
        /// <strong>Purpose.</strong>
        /// - The api provides the functionality to get list of all projects by userid from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<ProjectViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllProjectViewModelExamples))]
        //[SwaggerResponseExample(HttpStatusCode.OK, typeof(IEnumerable<ProjectViewModelExamples>))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        [Route("api/v1/Project/GetAllProjectByUserId/")]
        public IEnumerable<ProjectViewModel> GetAllProjectByUserId()
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
            return _projectProvider.GetAllProjectByUserId(this.LoggedInUserId);
        }


        /// <summary>
        /// Get project by guid
        /// </summary>
        /// <remarks>
        /// Get project by guid<br></br>
        /// <strong>Purpose.</strong>
        /// - The api provides the functionality to get project's basic details based on its guid from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of a project that needs to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectBasicDetailsViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ProjectBasicDetailsViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        [Route("api/v1/Project/GetProjectBasicDetails/{guid}")]
        public ProjectBasicDetailsViewModel GetProjectBasicDetails(Guid guid)
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
            var checkList = _projectProvider.ProjectBasicDetail(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Project was not found."),
                });
            }

            return checkList;
        }
        /// <summary>
        /// To check project linked by entityId
        /// </summary>
        /// <remarks>
        /// To check project linked by entityId<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to check whether project is already linked by entityId or not.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="projectId">Guid of a project</param>
        /// <param name="entityId">entity id</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectStaffMemberRoleViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ProjectStaffMemberRoleViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/v1/Project/CheckEntityLinkedProject/{projectId}/{entityId}")]
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
                var checkList = _projectDeployProvider.SQL_CheckEntityLinkedProject(newprojectId, entityId);
                if (checkList == null)
                {
                    return null;
                }
                return checkList;
            }
        }
    }
}
