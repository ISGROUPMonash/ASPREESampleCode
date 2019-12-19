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
    public class LookupDataOptionGroupController : BaseController
    {
        private readonly ILookupDataOptionGroupProvider _lookupDataOptionGroupProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookupDataOptionGroupProvider"></param>
        public LookupDataOptionGroupController(ILookupDataOptionGroupProvider lookupDataOptionGroupProvider)
        {
            _lookupDataOptionGroupProvider = lookupDataOptionGroupProvider;
        }

        /// <summary>
        /// Get All Lookup Data Option Groups
        /// </summary>
        /// <remarks>
        /// Get All Lookup Data Option Groups
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<LookupDataOptionGroupViewModel>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // GET: api/LookupDataOptionGroup
        public IEnumerable<LookupDataOptionGroupViewModel> GetAll()
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
            return _lookupDataOptionGroupProvider.GetAll();
        }

        /// <summary>
        /// Get Lookup Data Option Group By Guid
        /// </summary>
        /// <remarks>
        /// Get Lookup Data Option Group By Guid
        /// </remarks>
        /// <param name="guid">Guid of an lookup data option group that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LookupDataOptionGroupViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")] 
        // GET: api/LookupDataOptionGroup/5
        public LookupDataOptionGroupViewModel Get(Guid guid)
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
            var checkList = _lookupDataOptionGroupProvider.GetByGuid(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Lookup Data Option Group was not found."),
                    ReasonPhrase = "Lookup Data Option Group was not found."
                });
            }

            return checkList;
        }

        /// <summary>
        /// Add New Lookup Data Option Group
        /// </summary>
        /// <remarks>
        /// Add New Lookup Data Option Group
        /// </remarks>
        /// <param name="newLookupDataOptionGroup">New LookupDataOptionGroup Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LookupDataOptionGroupViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // POST: api/LookupDataOptionGroup
        public HttpResponseMessage Post([FromBody]NewLookupDataOptionGroupViewModel newLookupDataOptionGroup)
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

            var added = _lookupDataOptionGroupProvider.Create(new LookupDataOptionGroupViewModel
            {
                Label = newLookupDataOptionGroup.Label,
                LookupDataId = newLookupDataOptionGroup.LookupDataId,
                Value = newLookupDataOptionGroup.Value
            });

            _lookupDataOptionGroupProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, added);
        }

        /// <summary>
        /// Edit Existing Lookup Data Option Group
        /// </summary>
        /// <remarks>
        /// Edit Existing Lookup Data Option Group
        /// </remarks>
        /// <param name="editLookupDataOption">Edit LookupDataOption Model</param>
        /// <param name="guid">guid of lookup data option group that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LookupDataOptionGroupViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // PUT: api/LookupDataOptionGroup/5
        public HttpResponseMessage Put(Guid guid, [FromBody]EditLookupDataOptionGroupViewModel editLookupDataOption)
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

            var response = _lookupDataOptionGroupProvider.Update(new LookupDataOptionGroupViewModel
            {
                Label = editLookupDataOption.Label,
                LookupDataId = editLookupDataOption.LookupDataId,
                Value = editLookupDataOption.Value,
                Guid = guid
            });

            _lookupDataOptionGroupProvider.SaveChanges();

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Lookup Data Option Group was not found.");
        }

        /// <summary>
        /// Delete Existing Lookup Data Option Group
        /// </summary>
        /// <remarks>
        /// Delete Existing Lookup Data Option Group
        /// </remarks>
        /// <param name="guid">Guid of lookup data option group that is to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LookupDataOptionGroupViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // DELETE: api/LookupDataOptionGroup/5
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
            var response = _lookupDataOptionGroupProvider.DeleteByGuid(guid, LoggedInUserId);
            _lookupDataOptionGroupProvider.SaveChanges();

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Lookup Data Option Group was not found.");
        }
    }
}
