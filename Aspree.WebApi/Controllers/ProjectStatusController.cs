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
    public class ProjectStatusController : BaseController
    {
        private readonly IProjectStatusProvider _projectStatusProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectStatusProvider"></param>
        public ProjectStatusController(IProjectStatusProvider projectStatusProvider)
        {
            _projectStatusProvider = projectStatusProvider;
        }

        /// <summary>
        /// Get All Project Status
        /// </summary>
        /// <remarks>
        /// Get All Project Status
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<ProjectStatusViewModel>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // GET: api/ProjectStatus
        public IEnumerable<ProjectStatusViewModel> GetAll()
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
            return _projectStatusProvider.GetAll();
        }

        /// <summary>
        /// Get Project Status By Guid
        /// </summary>
        /// <remarks>
        /// Get Project Status By Guid
        /// </remarks>
        /// <param name="guid">Guid of a project status that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectStatusViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]  
        // GET: api/ProjectStatus/5
        public ProjectStatusViewModel Get(Guid guid)
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
            var checkList = _projectStatusProvider.GetByGuid(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Project status was not found."),
                    ReasonPhrase = "Project status was not found."
                });
            }

            return checkList;
        }


        /// <summary>
        /// Add New Project Status
        /// </summary>
        /// <remarks>
        /// Add New Project Status
        /// </remarks>
        /// <param name="newProjectStatus">New Project Status Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectStatusViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // POST: api/ProjectStatus
        public HttpResponseMessage Post([FromBody]NewStatus newProjectStatus)
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

            var added = _projectStatusProvider.Create(new ProjectStatusViewModel() {
                 IsActive = true,
                 Status = newProjectStatus.Status
            });
            _projectStatusProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, added);
        }


        /// <summary>
        /// Edit Existing Project Status
        /// </summary>
        /// <remarks>
        /// Edit Existing Project Status
        /// </remarks>
        /// <param name="editProjectStatus">Edit Project Status Model</param>
        /// <param name="guid">guid of project status that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectStatusViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // PUT: api/ProjectStatus/5
        public HttpResponseMessage Put(Guid guid, [FromBody]EditStatus editProjectStatus)
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

            var response = _projectStatusProvider.Update(new ProjectStatusViewModel() {
                 Status = editProjectStatus.Status,
                 Guid = guid
            });
            _projectStatusProvider.SaveChanges();

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Project status was not found.");
        }

        /// <summary>
        /// Delete Existing Project Status
        /// </summary>
        /// <remarks>
        /// Delete Existing Project Status
        /// </remarks>
        /// <param name="guid">Guid of project status that is to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectStatusViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // DELETE: api/ProjectStatus/5
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
            var response = _projectStatusProvider.DeleteByGuid(guid, this.LoggedInUserId);
            _projectStatusProvider.SaveChanges();

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Project status was not found.");
        }
    }
}
