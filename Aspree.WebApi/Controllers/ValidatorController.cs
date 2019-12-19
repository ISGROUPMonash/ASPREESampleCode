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
    public class ValidatorController : BaseController
    {
        private readonly IValidatorProvider _validatorProvider;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="validatorProvider"></param>
        public ValidatorController(IValidatorProvider validatorProvider)
        {
            _validatorProvider = validatorProvider;
        }

        /// <summary>
        /// Validate Fields for forms
        /// </summary>
        /// <remarks>
        /// Validate Fields for forms
        /// </remarks>
        /// <param name="validator">Validator Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // POST: api/Validator
        public HttpResponseMessage Post([FromBody]ValidatorViewModal validator)
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
            var exists = _validatorProvider.CheckExist(validator);
            
            return Request.CreateResponse(HttpStatusCode.OK, exists);
        }
    }
}
