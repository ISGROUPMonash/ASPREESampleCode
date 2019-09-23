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
   public class VariableController : BaseController
    {
        private readonly IVariableProvider _variableProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variableProvider"></param>
        public VariableController(IVariableProvider variableProvider)
        {
            _variableProvider = variableProvider;
        }

        /// <summary>
        /// Get project builder variables
        /// </summary>
        /// <remarks>
        /// Get project builder variables<br></br>   
        /// <strong>Purpose.</strong>
        /// - This api is used for return all details of variable page in project builder.
        /// - This api return all existing Forms, VariableType, ValidationRule, VariableCategory and roles.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="projectId">Guid of a project that variables to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectBuilderVariablesViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ProjectBuilderVariablesViewModelExamples))]
        [AuthorizeAttribute(Roles = "Project Admin, System Admin")]
        [Route("api/v1/Variable/ProjectBuilderVariables/{projectId}")]
        [HttpGet]
        public ProjectBuilderVariablesViewModel ProjectBuilderVariables(Guid projectId)
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
            return _variableProvider.GetProjectBuilderVariables(this.LoggedInUserTenantId);
        }

        
    }
}
