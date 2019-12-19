using Aspree.Core.ViewModels;
using Aspree.Provider.Interface;
using Swashbuckle.Swagger.Annotations;
using Swashbuckle.Examples;
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
    public class PrivilegeController : BaseController
    {
        private readonly IPrivilegeProvider _privilegeProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="privilegeProvider"></param>
        public PrivilegeController(IPrivilegeProvider privilegeProvider)
        {
            this._privilegeProvider = privilegeProvider;
        }


        /// <summary>
        /// Get all privileges
        /// </summary>
        /// <remarks>
        /// Get all privileges<br></br>
        /// <strong>Purpose.</strong>
        /// - The purpose of this api is to get all privileges from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<PrivilegeSmallViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllPrivilegeSmallViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        public HttpResponseMessage GetAll()
        {
            if (Request.Headers.Authorization == null)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login.")
                };
                throw new HttpResponseException(msg);
            }
            var privilege=  _privilegeProvider.GetAll();
            return Request.CreateResponse(HttpStatusCode.OK, privilege);
        }
    }
}
