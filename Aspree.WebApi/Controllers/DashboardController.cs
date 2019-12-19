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
    public class DashboardController : ApiController
    {
        private readonly IDashboardProvider _dashboardProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dashboardProvider"></param>
        public DashboardController(IDashboardProvider dashboardProvider)
        {
            this._dashboardProvider = dashboardProvider;
        }

        /// <summary>
        /// Currently not in use
        /// </summary>
        /// <remarks>
        /// Currently not in use
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(DashboardStatus))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // GET: api/Dashboard
        public IEnumerable<string> Get()
        {
            #region check login
            //if (this.LoggedInUserId == Guid.Empty)
            //{
            //    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
            //    {
            //        Content = new StringContent("Unauthorized access. Please login."),
            //    });
            //}
            #endregion
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Currently not in use
        /// </summary>
        /// <remarks>
        /// Currently not in use
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(DashboardStatus))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // POST: api/Dashboard
        public string Get(Guid guid)
        {
            #region check login
            //if (this.LoggedInUserId == Guid.Empty)
            //{
            //    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
            //    {
            //        Content = new StringContent("Unauthorized access. Please login."),
            //    });
            //}
            #endregion
            return "value";
        }


        /// <summary>
        /// Return Dashboard User and Role data
        /// </summary>
        /// <remarks>
        /// Return Dashboard User and Role data
        /// </remarks>
        /// <param name="filter">DashboardFilter Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(DashboardStatus))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // POST: api/Dashboard
        public HttpResponseMessage Post([FromBody]DashboardFilter filter)
        {
            #region check login
            //if (this.LoggedInUserId == Guid.Empty)
            //{
            //    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
            //    {
            //        Content = new StringContent("Unauthorized access. Please login."),
            //    });
            //}
            #endregion
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var dashboardStatus = this._dashboardProvider.GetDashboardStatus(filter);
            return Request.CreateResponse(HttpStatusCode.OK, dashboardStatus);
        }

        /// <summary>
        /// Currently not in use
        /// </summary>
        /// <remarks>
        /// Currently not in use
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(DashboardStatus))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // PUT: api/Dashboard/5
        public void Put(Guid guid, [FromBody]string value)
        {
            #region check login
            //if (this.LoggedInUserId == Guid.Empty)
            //{
            //    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
            //    {
            //        Content = new StringContent("Unauthorized access. Please login."),
            //    });
            //}
            #endregion
        }

        /// <summary>
        /// Currently not in use
        /// </summary>
        /// <remarks>
        /// Currently not in use
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(DashboardStatus))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // DELETE: api/Dashboard/5
        public void Delete(Guid guid)
        {
            #region check login
            //if (this.LoggedInUserId == Guid.Empty)
            //{
            //    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
            //    {
            //        Content = new StringContent("Unauthorized access. Please login."),
            //    });
            //}
            #endregion
        }
    }
}
