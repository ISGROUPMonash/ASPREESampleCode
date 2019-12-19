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
    public class PostCodeController : BaseController
    {
        private readonly IPostCodeProvider _postCodeProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postCodeProvider"></param>
        public PostCodeController(IPostCodeProvider postCodeProvider)
        {
            _postCodeProvider = postCodeProvider;
        }

        /// <summary>
        /// Get all postcode.
        /// </summary>
        /// <remarks>
        /// Get all postcode<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to get all postcodes.
        /// - This api returns list of PostCodeViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<PostCodeViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllPostCodeViewModelExamples))]
        public IEnumerable<PostCodeViewModel> GetAll()
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
            return _postCodeProvider.GetAll();
        }
        /// <summary>
        /// Get postcode by guid
        /// </summary>
        /// <remarks>
        /// Get postcode by guid<br></br> 
        /// <strong>Purpose.</strong>
        /// - This api is used to get postcode by its guid.
        /// - This api returns PostCodeViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of a postcode that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]  
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(PostCodeViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(PostCodeViewModelExamples))]
        public PostCodeViewModel Get(Guid guid)
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
            var stateList = _postCodeProvider.GetByGuid(guid);
            if (stateList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("PostCode was not found."),
                    ReasonPhrase = "PostCode was not found."
                });
            }

            return stateList;
        }

        /// <summary>
        /// Add new postcode
        /// </summary>
        /// <remarks>
        /// Add new postcode<br></br> 
        /// <strong>Purpose.</strong>
        /// - This api is used to create new postcode.
        /// - This api takes NewPostCodeViewModel as input request.
        /// - This api returns PostCodeViewModel of new created postcode in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="PostCodeViewModel">New PostCodeViewModel Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "record  already exist with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(PostCodeViewModel))]
        [SwaggerRequestExample(typeof(NewPostCodeViewModel), typeof(NewPostCodeViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(PostCodeViewModelExamples))]
        public HttpResponseMessage Post([FromBody]NewPostCodeViewModel PostCodeViewModel)
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
                var added = _postCodeProvider.Create(new PostCodeViewModel()
                {
                    PostalCode = PostCodeViewModel.PostalCode,
                    StateId = PostCodeViewModel.StateId,
                    CityId = PostCodeViewModel.CityId,
                    SuburbId=PostCodeViewModel.SuburbId,
                });

                _postCodeProvider.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, added);
        }

        /// <summary>
        /// Edit existing postcode
        /// </summary>
        /// <remarks>
        /// Edit existing postcode<br></br> 
        /// <strong>Purpose.</strong>        
        /// - This api is used to update an existing postcode.
        /// - This api takes EditPostCodeViewModel and guid of postcode as input request.
        /// - This api returns PostCodeViewModel of updated postcode in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="PostCodeViewModel">Edit PostCodeViewModel Model</param>
        /// <param name="guid">guid of city that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "record  already exist with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]        
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]                
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(PostCodeViewModel))]
        [SwaggerRequestExample(typeof(EditPostCodeViewModel), typeof(EditPostCodeViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(PostCodeViewModelExamples))]
        public HttpResponseMessage Put(Guid guid, [FromBody]EditPostCodeViewModel PostCodeViewModel)
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

                var updated = _postCodeProvider.Update(new PostCodeViewModel()
                {
                    PostalCode = PostCodeViewModel.PostalCode,
                    StateId = PostCodeViewModel.StateId,
                    CityId = PostCodeViewModel.CityId,
                    SuburbId = PostCodeViewModel.SuburbId,
                });

                _postCodeProvider.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, updated);

        }

        /// <summary>
        /// Delete existing postcode
        /// </summary>
        /// <remarks>
        /// Delete existing postcode<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to delete an existing postcode.
        /// - This api takes guid of a postcode that needs to be deleted as input request.
        /// - This api returns PostCodeViewModel of deleted postcode in response model.
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
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(PostCodeViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(PostCodeViewModelExamples))]
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
            var deleted = _postCodeProvider.DeleteByGuid(guid, this.LoggedInUserId);
            _postCodeProvider.SaveChanges();

            if (deleted != null)
                return Request.CreateResponse(HttpStatusCode.OK, deleted);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "PostCode was not found.");
        }
    }
}
