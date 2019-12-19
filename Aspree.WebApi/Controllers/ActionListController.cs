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
    /// action list
    /// </summary>
    public class ActionListController : BaseController
    {
        private readonly IActionListProvider _actionListProvider;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="actionListProvider"></param>
        public ActionListController(IActionListProvider actionListProvider)
        {
            this._actionListProvider = actionListProvider;
        }

        /// <summary>
        /// Get all action lists
        /// </summary>
        /// <remarks>
        /// Action list<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get all action list data.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="searchfiltermodel">search filter model</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided detail are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<ActionListViewModel>))]
        [SwaggerRequestExample(typeof(ActionListSearchParameters), typeof(ActionListSearchParametersExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllActionListViewModelExamples))]
        [HttpPost]
        [Route("api/v1/ActionList/AllActionListData")]
        public IEnumerable<ActionListViewModel> AllActionListData([FromBody]ActionListSearchParameters searchfiltermodel)
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
            searchfiltermodel.LoggedInUserGuid = LoggedInUserId;
            return _actionListProvider.GetAll(searchfiltermodel);
        }
        /// <summary>
        /// Count all action lists
        /// </summary>
        /// <remarks>
        /// Count all action list records<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to count all action list.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Project id that action list to be count</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Guid))]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        [Route("api/v1/ActionList/CountAllActionList/{guid}")]
        public int CountAllActionList(Guid? guid=null)
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
            if (guid == null)
            {
                GuidExceptionHandler();
            }
            Guid newguid = guid.Value;


            return _actionListProvider.CountAllActionListRecord(newguid, this.LoggedInUserId);
        }


        #region not-implemented

        ///////////// <summary>
        ///////////// Get Entity Type By Guid
        ///////////// </summary>
        ///////////// <remarks>
        ///////////// Get Entity Type By Guid
        ///////////// </remarks>
        ///////////// <param name="guid">Guid of an entity type that to be fetched</param>
        //////////[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        //////////[SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActionListViewModel))]
        //////////[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //////////[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        ////////////[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        ////////////[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]   
        //////////public ActionListViewModel Get(Guid guid)
        //////////{
        //////////    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented)
        //////////    {
        //////////        Content = new StringContent("API Not Implemented."),
        //////////        ReasonPhrase = "API Not Implemented."
        //////////    });
        //////////}

        /////////// <summary>
        /////////// Add New Entity Type
        /////////// </summary>
        /////////// <remarks>
        /////////// Add New Entity Type
        /////////// </remarks>
        /////////// <param name="newActionListViewModel">New ActionListViewModel Model</param>
        ////////[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        ////////[SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActionListViewModel))]
        ////////[SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        ////////[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //////////[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //////////[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //////////[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        ////////public HttpResponseMessage Post([FromBody]ActionListViewModel newActionListViewModel)
        ////////{
        ////////    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented)
        ////////    {
        ////////        Content = new StringContent("API Not Implemented."),
        ////////        ReasonPhrase = "API Not Implemented."
        ////////    });
        ////////}

        /////////// <summary>
        /////////// Edit Existing Entity Type
        /////////// </summary>
        /////////// <remarks>
        /////////// Edit Existing Entity Type
        /////////// </remarks>
        /////////// <param name="editEntityType">Edit EntityType Model</param>
        /////////// <param name="guid">guid of entity type that has to be edited</param>
        ////////[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        ////////[SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActionListViewModel))]
        ////////[SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        ////////[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        ////////[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //////////[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //////////[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        ////////public HttpResponseMessage Put(Guid guid, [FromBody]ActionListViewModel editEntityType)
        ////////{
        ////////    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented)
        ////////    {
        ////////        Content = new StringContent("API Not Implemented."),
        ////////        ReasonPhrase = "API Not Implemented."
        ////////    });
        ////////}

        /////////// <summary>
        /////////// Delete Existing Entity Type
        /////////// </summary>
        /////////// <remarks>
        /////////// Delete Existing Entity Type
        /////////// </remarks>
        /////////// <param name="guid">Guid of entity type that is to be deleted</param>
        ////////[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        ////////[SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActionListViewModel))]
        ////////[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        ////////[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //////////[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //////////[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        ////////public HttpResponseMessage Delete(Guid guid)
        ////////{
        ////////    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotImplemented)
        ////////    {
        ////////        Content = new StringContent("API Not Implemented."),
        ////////        ReasonPhrase = "API Not Implemented."
        ////////    });
        ////////}
        #endregion






        /// <summary>
        /// Get all activity/forms of action list
        /// </summary>
        /// <remarks>
        /// Get all activity/forms of action list<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get activity/forms of action list.
        /// - Activities of action list will be fetched from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="projectId">Project Id</param>
        /// <returns></returns>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<ActionListViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllActionListViewModelExamples))]
        [HttpGet]
        [Route("api/v1/ActionList/GetAllActionListActivities/{projectId}")]
        public IEnumerable<ActionListViewModel> GetAllActionListActivities(Guid? projectId=null)
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

            return _actionListProvider.GetAllActionListActivities(newprojectId, this.LoggedInUserId);
        }
    }
}