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
    public class LookupDataGroupOptionController : BaseController
    {
        private readonly ILookupDataGroupOptionProvider _lookupDataGroupOptionProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookupDataGroupOptionProvider"></param>
        public LookupDataGroupOptionController(ILookupDataGroupOptionProvider lookupDataGroupOptionProvider)
        {
            _lookupDataGroupOptionProvider = lookupDataGroupOptionProvider;
        }


        /// <summary>
        /// Get All Lookup Data Group Options
        /// </summary>
        /// <remarks>
        /// Get All Lookup Data Group Options
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<LookupDataGroupOptionViewModel>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // GET: api/LookupDataGroupOption
        public IEnumerable<LookupDataGroupOptionViewModel> GetAll()
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
            return _lookupDataGroupOptionProvider.GetAll();
        }

        /// <summary>
        /// Get Lookup Data Group Option By Guid
        /// </summary>
        /// <remarks>
        /// Get Lookup Data Group Option By Guid
        /// </remarks>
        /// <param name="guid">Guid of an lookup data group option that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LookupDataGroupOptionViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]       
        // GET: api/LookupDataGroupOption/5
        public LookupDataGroupOptionViewModel Get(Guid guid)
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
            var checkList = _lookupDataGroupOptionProvider.GetByGuid(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Lookup Data Group Option was not found."),
                    ReasonPhrase = "Lookup Data Group Option was not found."
                });
            }

            return checkList;
        }

        /// <summary>
        /// Add New Lookup Data Group Option
        /// </summary>
        /// <remarks>
        /// Add New Lookup Data Group Option
        /// </remarks>
        /// <param name="newLookupDataGroupOption">New LookupDataGroupOption Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LookupDataGroupOptionViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // POST: api/LookupDataGroupOption
        public HttpResponseMessage Post([FromBody]NewLookupDataGroupOptionViewModel newLookupDataGroupOption)
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

            var added = _lookupDataGroupOptionProvider.Create(new LookupDataGroupOptionViewModel
            {
                LookupDataOptionGroupId = newLookupDataGroupOption.LookupDataOptionGroupId,
                Text = newLookupDataGroupOption.Text,
                Value = newLookupDataGroupOption.Value
            });
            
            _lookupDataGroupOptionProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, added);
        }

        /// <summary>
        /// Edit Existing Lookup Data Group Option
        /// </summary>
        /// <remarks>
        /// Edit Existing Lookup Data Group Option
        /// </remarks>
        /// <param name="editLookupDataGroupOption">Edit LookupDataGroupOption Model</param>
        /// <param name="guid">guid of lookup data group option that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LookupDataGroupOptionViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // PUT: api/LookupDataGroupOption/5
        public HttpResponseMessage Put(Guid guid, [FromBody]EditLookupDataGroupOptionViewModel editLookupDataGroupOption)
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

            var response =_lookupDataGroupOptionProvider.Update(new LookupDataGroupOptionViewModel
            {
                LookupDataOptionGroupId = editLookupDataGroupOption.LookupDataOptionGroupId,
                Text = editLookupDataGroupOption.Text,
                Value = editLookupDataGroupOption.Value,
                Guid  = guid
            });

            _lookupDataGroupOptionProvider.SaveChanges();

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Lookup Data Group Option was not found.");
        }

        /// <summary>
        /// Delete Existing Lookup Data Group Option
        /// </summary>
        /// <remarks>
        /// Delete Existing Lookup Data Group Option
        /// </remarks>
        /// <param name="guid">Guid of lookup data group option that is to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(LookupDataGroupOptionViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // DELETE: api/LookupDataGroupOption/5
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
            var response = _lookupDataGroupOptionProvider.DeleteByGuid(guid, LoggedInUserId);
            _lookupDataGroupOptionProvider.SaveChanges();

            if (response!=null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Lookup Data Group Option was not found.");
        }
    }
}
