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
    public class ValidationRuleController : BaseController
    {
        private readonly IValidationRuleProvider _validationRuleProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="validationRuleProvider"></param>
        public ValidationRuleController(IValidationRuleProvider validationRuleProvider)
        {
            _validationRuleProvider = validationRuleProvider;
        }

        /// <summary>
        /// Get All Validation Rules
        /// </summary>
        /// <remarks>
        /// Get All Validation Rules
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<ValidationRuleViewModel>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // GET: api/ValidationRule
        public IEnumerable<ValidationRuleViewModel> GetAll()
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
            return _validationRuleProvider.GetAll();
        }

        /// <summary>
        /// Get Validation Rule By Guid
        /// </summary>
        /// <remarks>
        /// Get Validation Rule By Guid
        /// </remarks>
        /// <param name="guid">Guid of a validation rule that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ValidationRuleViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]        
        // GET: api/ValidationRule/5
        public ValidationRuleViewModel Get(Guid guid)
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
            var checkList = _validationRuleProvider.GetByGuid(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Validation rule was not found."),
                    ReasonPhrase = "Validation rule was not found."
                });
            }

            return checkList;
        }

        /// <summary>
        /// Add New Validation Rule
        /// </summary>
        /// <remarks>
        /// Add New Validation Rule
        /// </remarks>
        /// <param name="newValidationRule">New Validation Rule Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ValidationRuleViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // POST: api/ValidationRule
        public HttpResponseMessage Post([FromBody]NewValidationRuleViewModel newValidationRule)
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
            var added = _validationRuleProvider.Create(new ValidationRuleViewModel()
            {
                ErrorMessage = newValidationRule.ErrorMessage,
                MaxRange = newValidationRule.MaxRange,
                MinRange = newValidationRule.MinRange,
                RegExId = newValidationRule.RegExId,
                RuleType = newValidationRule.RuleType
            });
            _validationRuleProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, added);
        }


        /// <summary>
        /// Edit Existing Validation Rule
        /// </summary>
        /// <remarks>
        /// Edit Existing Validation Rule
        /// </remarks>
        /// <param name="editValidationRule">Edit Validation Rule Model</param>
        /// <param name="guid">guid of validation rule that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ValidationRuleViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // PUT: api/ValidationRule/5
        public HttpResponseMessage Put(Guid guid, [FromBody]EditValidationRuleViewModel editValidationRule)
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
            var response = _validationRuleProvider.Update(new ValidationRuleViewModel()
            {
                ErrorMessage = editValidationRule.ErrorMessage,
                Guid = guid,
                MaxRange = editValidationRule.MaxRange,
                RuleType = editValidationRule.RuleType,
                RegExId = editValidationRule.RegExId,
                MinRange = editValidationRule.MinRange
            });
            _validationRuleProvider.SaveChanges();
            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Validation rule was not found.");
        }

        /// <summary>
        /// Delete Existing Validation Rule
        /// </summary>
        /// <remarks>
        /// Delete Existing Validation Rule
        /// </remarks>
        /// <param name="guid">Guid of validation rule that is to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ValidationRuleViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // DELETE: api/ValidationRule/5
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
            var response = _validationRuleProvider.DeleteByGuid(guid, this.LoggedInUserId);
            _validationRuleProvider.SaveChanges();
            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Validation rule was not found.");
        }
    }
}