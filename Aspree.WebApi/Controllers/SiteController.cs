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
    public class SiteController : BaseController
    {
        private readonly ISiteProvider _siteProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteProvider"></param>
        public SiteController(ISiteProvider siteProvider)
        {
            _siteProvider = siteProvider;
        }

        /// <summary>
        /// Get All Sites
        /// </summary>
        /// <remarks>
        /// Get All Sites
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<SiteViewModel>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // GET: api/Site
        public IEnumerable<SiteViewModel> GetAll()
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
            return _siteProvider.GetAll();
        }


        /// <summary>
        /// Get Site By Guid
        /// </summary>
        /// <remarks>
        /// Get Site By Guid
        /// </remarks>
        /// <param name="guid">Guid of a site that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SiteViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]    
        // GET: api/Site/5
        public SiteViewModel Get(Guid guid)
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
            var checkList = _siteProvider.GetByGuid(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Site was not found."),
                    ReasonPhrase = "Site was not found."
                });
            }

            return checkList;
        }


        /// <summary>
        /// Add New Site
        /// </summary>
        /// <remarks>
        /// Add New Site
        /// </remarks>
        /// <param name="newSite">New Project Site Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SiteViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // POST: api/Site
        public HttpResponseMessage Post([FromBody]NewSiteViewModel newSite)
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

           var added =  _siteProvider.Create(new SiteViewModel()
            {
                AddressLine1 = newSite.AddressLine1,
                AddressLine2 = newSite.AddressLine2,
                CityId = newSite.CityId,
                CountyId = newSite.CountyId,
                CreatedBy = LoggedInUserId,
                CreatedDate = DateTime.UtcNow,
                GPSLocations = newSite.GPSLocations,
                Name = newSite.Name,
                PostCode = newSite.PostCode,
                StateId = newSite.StateId,
                Suburb = newSite.Suburb
            });
            _siteProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, added);
        }


        /// <summary>
        /// Edit Existing Site
        /// </summary>
        /// <remarks>
        /// Edit Existing Site
        /// </remarks>
        /// <param name="editSite">Edit Project Site Model</param>
        /// <param name="guid">guid of site that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SiteViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // PUT: api/Site/5
        public HttpResponseMessage Put(Guid guid, [FromBody]EditSiteViewModel editSite)
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

            var response = _siteProvider.Update(new SiteViewModel
            {
                AddressLine1 = editSite.AddressLine1,
                AddressLine2 = editSite.AddressLine2,
                CityId = editSite.CityId,
                CountyId = editSite.CountyId,
                ModifiedBy = LoggedInUserId,
                ModifiedDate = DateTime.UtcNow,
                GPSLocations = editSite.GPSLocations,
                Name = editSite.Name,
                PostCode = editSite.PostCode,
                StateId = editSite.StateId,
                Suburb = editSite.Suburb,
                Guid = guid
            });

            _siteProvider.SaveChanges();

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Site was not found.");
        }


        /// <summary>
        /// Delete Existing Site
        /// </summary>
        /// <remarks>
        /// Delete Existing Site
        /// </remarks>
        /// <param name="guid">Guid of site that is to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SiteViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // DELETE: api/Site/5
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
            var response = _siteProvider.DeleteByGuid(guid, this.LoggedInUserId);
            _siteProvider.SaveChanges();

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Site was not found.");
        }
    }
}
