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
    public class CountryController : BaseController
    {
        private readonly ICountryProvider _countryProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="countryProvider"></param>
        public CountryController(ICountryProvider countryProvider)
        {
            _countryProvider = countryProvider;
        }

        /// <summary>
        /// Get all countries
        /// </summary>
        /// <remarks>
        /// Get all countries<br></br>
        /// <strong>Purpose.</strong>
        /// - The purpose of this api is to get the list of all countries.
        /// - The api fetches country list from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
         /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<CountryViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllCountryViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        public IEnumerable<CountryViewModel> GetAll()
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
            return _countryProvider.GetAll();
        }

        /// <summary>
        /// Get country by guid
        /// </summary>
        /// <remarks>
        /// Get country by guid<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get country based on its guid.
        /// - The api fetches country from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of a country that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CountryViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(CountryViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")] 
        public CountryViewModel Get(Guid guid)
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
            var countryList = _countryProvider.GetByGuid(guid);
            if (countryList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Country was not found."),
                    ReasonPhrase = "Country was not found."
                });
            }

            return countryList;
        }

        /// <summary>
        /// Add new country
        /// </summary>
        /// <remarks>
        /// Add new country<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to create new country into the system.
        /// - Country name will be saved with its abbreviation into SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newCountry">NewCountry Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CountryViewModel))]
        [SwaggerRequestExample(typeof(NewCountryViewModel), typeof(NewCountryViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(CountryViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        public HttpResponseMessage Post([FromBody]NewCountryViewModel newCountry)
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

           var added  = _countryProvider.Create(new CountryViewModel()
            {
                Name = newCountry.Name,
                Abbr = newCountry.Abbr
            });

            _countryProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, added);

        }

        /// <summary>
        /// Edit existing country
        /// </summary>
        /// <remarks>
        /// Edit existing country<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to edit/update country using its guid.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editCountry">Edit EditCountry Model</param>
        /// <param name="guid">guid of country that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CountryViewModel))]
        [SwaggerRequestExample(typeof(EditCountryViewModel), typeof(EditCountryViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(CountryViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        public HttpResponseMessage Put(Guid guid, [FromBody]EditCountryViewModel editCountry)
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

            var updated =  _countryProvider.Update(new CountryViewModel()
            {
                Abbr = editCountry.Abbr,
                Name = editCountry.Name,
                Guid = guid
            });

            _countryProvider.SaveChanges();

            if (updated != null)
                return Request.CreateResponse(HttpStatusCode.OK, updated);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Country was not found.");
        }

        /// <summary>
        /// Delete existing country
        /// </summary>
        /// <remarks>
        ///Delete existing country<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to delete an existing country using its guid.
        /// - The api will soft delete data from SQL table
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of country that is to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CountryViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(CountryViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
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
            var response = _countryProvider.DeleteByGuid(guid, this.LoggedInUserId);
            _countryProvider.SaveChanges();

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Country was not found.");
        }
    }
}
