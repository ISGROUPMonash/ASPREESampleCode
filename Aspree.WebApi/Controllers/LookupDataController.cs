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
    public class LookupDataController : BaseController
    {
        private readonly IVariableTypeProvider _VariableTypeProvider;
        private readonly ILookupDataProvider _lookupDataProvider;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookupDataProvider"></param>
        public LookupDataController(ILookupDataProvider lookupDataProvider ,IVariableTypeProvider VariableTypeProvider)
        {
            _lookupDataProvider = lookupDataProvider;
       
        }
        /// <summary>
        /// Get All Lookup Data
        /// </summary>
        /// <remarks>
        /// Get All Lookup Data
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<LookupDataViewModel>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // GET: api/LookupData
        public IEnumerable<LookupDataViewModel> GetAll()
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
            return _lookupDataProvider.GetAll();
        }

        /// <summary>
        /// Get Lookup Data By Guid
        /// </summary>
        /// <remarks>
        /// Get Lookup Data By Guid
        /// </remarks>
        /// <param name="guid">Guid of an lookup data that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LookupDataViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]  
        // GET: api/LookupData/5
        public LookupDataViewModel Get(Guid guid)
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
            var checkList = _lookupDataProvider.GetByGuid(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Lookup Data was not found."),
                    ReasonPhrase = "Lookup Data was not found."
                });
            }

            return checkList;
        }

        /// <summary>
        /// Add New Lookup Data
        /// </summary>
        /// <remarks>
        /// Add New Lookup Data
        /// </remarks>
        /// <param name="newLookupData">New LookupData Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LookupDataViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // POST: api/LookupData
        public HttpResponseMessage Post([FromBody]NewLookupDataViewModel newLookupData)
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
            var added =  _lookupDataProvider.Create(new LookupDataViewModel
            {
                CreatedBy = LoggedInUserId,
                CreatedDate = DateTime.UtcNow,
                LookupDataType = newLookupData.LookupDataType,
                Title = newLookupData.Title,
                VariableTypeId = newLookupData.VariableTypeId
            });
            _lookupDataProvider.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, added);
        }

        /// <summary>
        /// Edit Existing Lookup Data
        /// </summary>
        /// <remarks>
        /// Edit Existing Lookup Data
        /// </remarks>
        /// <param name="editLookupData">Edit LookupData Model</param>
        /// <param name="guid">guid of lookup data that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LookupDataViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // PUT: api/LookupData/5
        public HttpResponseMessage Put(Guid guid, [FromBody]EditLookupDataViewModel editLookupData)
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
           var response = _lookupDataProvider.Update(new LookupDataViewModel
            {
                ModifiedBy = LoggedInUserId,
                ModifiedDate = DateTime.UtcNow,
                LookupDataType = editLookupData.LookupDataType,
                Title = editLookupData.Title,
                VariableTypeId = editLookupData.VariableTypeId,
                Guid = guid
           });
            _lookupDataProvider.SaveChanges();
            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Lookup Data was not found.");
        }

        /// <summary>
        /// Delete Existing Lookup Data
        /// </summary>
        /// <remarks>
        /// Delete Existing Lookup Data
        /// </remarks>
        /// <param name="guid">Guid of lookup data that is to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LookupDataViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // DELETE: api/LookupData/5
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
            var response = _lookupDataProvider.DeleteByGuid(guid, this.LoggedInUserId);
            _lookupDataProvider.SaveChanges();

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Lookup Data was not found.");
        }
    }
}
