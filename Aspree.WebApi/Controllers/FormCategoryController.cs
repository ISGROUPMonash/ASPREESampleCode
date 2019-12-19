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
    public class FormCategoryController : BaseController
    {
        private readonly IFormCategoryProvider _formCategoryProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formCategoryProvider"></param>
        public FormCategoryController(IFormCategoryProvider formCategoryProvider)
        {
            _formCategoryProvider = formCategoryProvider;
        }

        /// <summary>
        /// Get all form categories
        /// </summary>
        /// <remarks>
        /// Get all form categories<br></br>
        /// <strong>Purpose.</strong>
        /// - The purpose of this api is to get all form categories.
        /// - The api fetches categories of form from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        //[SwaggerResponseExample(HttpStatusCode.OK, typeof(FormCategoryViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<FormCategoryViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllFormCategoryViewModelExamples))]
        public IEnumerable<FormCategoryViewModel> GetAll()
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
            return _formCategoryProvider.GetAll(this.LoggedInUserTenantId);
        }

        /// <summary>
        /// Get form category by guid
        /// </summary>
        /// <remarks>
        /// Get form category by guid<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to get form category by its guid.
        /// - This api returns FormCategoryViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of a form category that needs to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]                
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]   
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormCategoryViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormCategoryViewModelExamples))]
        public FormCategoryViewModel Get(Guid guid)
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
            var checkList = _formCategoryProvider.GetByGuid(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Form category was not found."),
                    ReasonPhrase = "Form category was not found."
                });
            }

            return checkList;
        }

        /// <summary>
        /// Add new form category
        /// </summary>
        /// <remarks>
        /// Add new form category<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to create new form category.
        /// - This api takes NewCategory of a form category as input request.
        /// - This api returns FormCategoryViewModel of created form category in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newFormCategory">New FormCategory Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormCategoryViewModel))]
        [SwaggerRequestExample(typeof(NewCategory), typeof(NewCategoryExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormCategoryViewModelExamples))]
        public HttpResponseMessage Post([FromBody]NewCategory newFormCategory)
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

            var added = _formCategoryProvider.Create(new FormCategoryViewModel()
            {
                CategoryName = newFormCategory.Category,
                CreatedBy = LoggedInUserId,
                TenantId = this.LoggedInUserTenantId,
            });
            _formCategoryProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, added);
        }

        /// <summary>
        /// Edit existing form category
        /// </summary>
        /// <remarks>
        /// Edit existing form category<br></br>
        /// - This api is used to update an existing form category.
        /// - This api takes EditCategory and guid of form category as input request.
        /// - This api returns FormCategoryViewModel of updated form category in response model.
        /// </remarks>
        /// <param name="editFormCategory">Edit FormCategory Model</param>
        /// <param name="guid">Guid of form category that needs to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]                
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormCategoryViewModel))]
        [SwaggerRequestExample(typeof(EditCategory), typeof(EditCategoryExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormCategoryViewModelExamples))]
        public HttpResponseMessage Put(Guid guid, [FromBody]EditCategory editFormCategory)
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

           var updated = _formCategoryProvider.Update(new FormCategoryViewModel()
            {
                CategoryName = editFormCategory.Category,
                Guid = guid,
                ModifiedBy=LoggedInUserId,
                ModifiedDate=DateTime.UtcNow
                   
            });
            _formCategoryProvider.SaveChanges();

            if (updated != null)
                return Request.CreateResponse(HttpStatusCode.OK, updated);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Form category was not found.");
        }

        /// <summary>
        /// Delete existing form category
        /// </summary>
        /// <remarks>
        /// Delete existing form category<br></br> 
        /// <strong>Purpose.</strong>
        /// - This api is used to delete an existing form category.
        /// - This api takes guid of a form category that needs to be deleted as input request.
        /// - This api returns FormCategoryViewModel of deleted form category in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of form category that needs to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]        
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormCategoryViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormCategoryViewModelExamples))]
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
            var response = _formCategoryProvider.DeleteByGuid(guid, this.LoggedInUserId);
            _formCategoryProvider.SaveChanges();

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Form category was not found.");
        }
    }
}
