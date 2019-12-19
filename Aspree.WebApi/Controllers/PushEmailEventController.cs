using Aspree.Core.ViewModels;
using Aspree.Provider.Provider;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Aspree.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
    public class PushEmailEventController : BaseController
    {
        private readonly PushEmailEventProvider _pushEmailEventProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushEmailEventProvider"></param>
        public PushEmailEventController(
            PushEmailEventProvider pushEmailEventProvider)
        {
            _pushEmailEventProvider = pushEmailEventProvider;
        }


        /// <summary>
        /// currently not in use
        /// </summary>
        /// <remarks>
        /// currently not in use
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(PushEmailEventViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // GET: PushEmailEvent
        public IEnumerable<PushEmailEventViewModel> GetAll()
        {
            #region check login
            if (this.LoggedInUserId == Guid.Empty)
            {
                throw new System.Web.Http.HttpResponseException(new System.Net.Http.HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new System.Net.Http.StringContent("Unauthorized access. Please login."),
                });
            }
            #endregion
            return _pushEmailEventProvider.GetAll();
        }
    }
}