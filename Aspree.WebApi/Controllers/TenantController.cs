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
    public class TenantController : BaseController
    {
        private readonly ITenantProvider _tenantProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantProvider"></param>
        public TenantController(ITenantProvider tenantProvider)
        {
            this._tenantProvider = tenantProvider;
        }


        /// <summary>
        /// Get All Tenants
        /// </summary>
        /// <remarks>
        /// Get All Tenants
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<TenantViewModel>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        public IEnumerable<TenantViewModel> GetAll()
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
            return _tenantProvider.GetAll();
        }

    }
}
