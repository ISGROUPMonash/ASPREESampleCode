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
    /// <summary>
    /// 
    /// </summary>
    public class CityController : BaseController
    {
        private readonly ICityProvider _cityProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cityProvider"></param>
        public CityController(ICityProvider cityProvider)
        {
            _cityProvider = cityProvider;
        }

        /// <summary>
        /// Get all cities
        /// </summary>
        /// <remarks>
        /// Get all cities<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to get all cities.
        /// - This api returns list of CityViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<CityViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllCityViewModelExamples))]
        public IEnumerable<CityViewModel> GetAll()
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
            return _cityProvider.GetAll();
        }

        /// <summary>
        /// Get city by guid
        /// </summary>
        /// <remarks>
        /// Get city by guid<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to get city by its guid.
        /// - This api returns CityViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of a city that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]  
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CityViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(CityViewModelExamples))]
        public CityViewModel Get(Guid guid)
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
            var stateList = _cityProvider.GetByGuid(guid);
            if (stateList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("City was not found."),
                    ReasonPhrase = "City was not found."
                });
            }

            return stateList;
        }

        /// <summary>
        /// Add new city
        /// </summary>
        /// <remarks>
        /// Add new city<br></br>    
        /// <strong>Purpose.</strong>
        /// - This api is used to create new city.
        /// - This api takes NewCityViewModel of a city as input request.
        /// - This api returns CityViewModel of created city in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="cityViewModel">NewCityViewModel Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CityViewModel))]
        [SwaggerRequestExample(typeof(NewCityViewModel), typeof(NewCityViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(CityViewModelExamples))]
        public HttpResponseMessage Post([FromBody]NewCityViewModel cityViewModel)
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
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var IsValidCountry = _cityProvider.CheckStateById(cityViewModel.StateId);

            if (IsValidCountry != null)
            {
                var added = _cityProvider.Create(new CityViewModel()
                {
                    Abbr = cityViewModel.Abbr,
                    Name = cityViewModel.Name,
                    StatedId = cityViewModel.StateId
                });

                _cityProvider.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, added);
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Provide valid state for city.");
        }

        /// <summary>
        /// Edit existing city
        /// </summary>
        /// <remarks>
        /// Edit existing city<br></br>    
        /// <strong>Purpose.</strong>
        /// - This api is used to update an existing city.
        /// - This api takes EditCityViewModel and guid of city as input request.
        /// - This api returns CityViewModel of updated city in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="cityViewModel">EditCityViewModel Model</param>
        /// <param name="guid">guid of city that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CityViewModel))]
        [SwaggerRequestExample(typeof(EditCityViewModel), typeof(EditCityViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(CityViewModelExamples))]
        public HttpResponseMessage Put(Guid guid,[FromBody]EditCityViewModel cityViewModel)
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
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var IsValidCountry = _cityProvider.CheckStateById(cityViewModel.StateId);

            if (IsValidCountry != null)
            {
               var updated = _cityProvider.Update(new CityViewModel()
                {
                    Abbr = cityViewModel.Abbr,
                    StatedId = cityViewModel.StateId,
                    Guid = guid,
                    Name = cityViewModel.Name
                });

                _cityProvider.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, updated);
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Provide valid State for city.");

        }

        /// <summary>
        /// Delete existing city
        /// </summary>
        /// <remarks>
        /// Delete existing city<br></br>      
        /// <strong>Purpose.</strong>
        /// - This api is used to delete an existing city.
        /// - This api takes guid of a city that has to be deleted as input request.
        /// - This api returns CityViewModel of deleted city in response model.
        /// - City will be soft deleted from SQL table
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of city that is to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CityViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(CityViewModelExamples))]
        public HttpResponseMessage Delete(Guid guid)
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
            var deleted = _cityProvider.DeleteByGuid(guid, this.LoggedInUserId);
            _cityProvider.SaveChanges();

            if (deleted != null)
                return Request.CreateResponse(HttpStatusCode.OK, deleted);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "City was not found.");
        }
    }
}
