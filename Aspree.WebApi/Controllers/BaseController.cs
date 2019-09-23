using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Aspree.WebApi.Controllers
{
    public class BaseController : ApiController
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid LoggedInUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid LoggedInUserTenantId { get; set; }

        
        /// <summary>
        /// Exception handler for GUID type parameter
        /// </summary>
        [NonAction]
        public void GuidExceptionHandler()
        {
            throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.MethodNotAllowed)
            {
                Content = new StringContent("Type of requested parameter is invalid."),
            });
        }
        
    }
}
