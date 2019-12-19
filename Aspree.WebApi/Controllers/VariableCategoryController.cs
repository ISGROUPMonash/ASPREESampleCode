using System;
using System.Linq;
using System.Web.Http;
using Aspree.Provider.Interface;
using Aspree.Core.ViewModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using Swashbuckle.Swagger.Annotations;
using Swashbuckle.Examples;

namespace Aspree.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class VariableCategoryController : BaseController
    {
        private readonly IVariableCategoryProvider _VariableCategoryProvider;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="VariableCategoryProvider"></param>
        public VariableCategoryController(IVariableCategoryProvider VariableCategoryProvider)
        {
            _VariableCategoryProvider = VariableCategoryProvider;
        }
        /// <summary>
        /// Get all variable categories
        /// </summary>
        /// <remarks>
        /// Get all variable categories<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to get all variable categories.
        /// - This api returns list of VariableCategoryViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<VariableCategoryViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllVariableCategoryViewModelExamples))]
        // GET: api/VariableCategory
        public IEnumerable<VariableCategoryViewModel> GetAll()
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
            return _VariableCategoryProvider.GetAll(this.LoggedInUserTenantId);
        }

        /// <summary>
        /// Get variable category by guid
        /// </summary>
        /// <remarks>
        /// Get variable category by guid<br></br> 
        /// <strong>Purpose.</strong>
        /// - This api is used to get variable category by its guid.
        /// - This api returns VariableCategoryViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of a variable category that needs to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]        
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(VariableCategoryViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(VariableCategoryViewModelExamples))]
        // GET: api/VariableCategory/5
        public VariableCategoryViewModel Get(Guid guid)
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
            var category = _VariableCategoryProvider.GetByGuid(guid);
            if (category == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Variable category was not found."),
                    ReasonPhrase = "Variable category was not found."
                });
            }
            return category;
        }

        /// <summary>
        /// Add new variable category
        /// </summary>
        /// <remarks>
        /// Add new variable category<br></br>  
        /// <strong>Purpose.</strong>
        /// - This api is used to create new variable category.
        /// - This api takes NewCategory of a variable category as input request.
        /// - This api returns VariableCategoryViewModel of new created variable category in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newCategory">New Category Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(VariableCategoryViewModel))]
        [SwaggerRequestExample(typeof(NewCategory), typeof(NewCategoryExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(VariableCategoryViewModelExamples))]
        // POST: api/VariableCategory
        public HttpResponseMessage Post([FromBody]NewCategory newCategory)
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
            var added = _VariableCategoryProvider.Create(new VariableCategoryViewModel() {
                TenantId=this.LoggedInUserTenantId,
                CategoryName = newCategory.Category,
                CreatedBy = LoggedInUserId,
                CreatedDate = DateTime.UtcNow
            });
            _VariableCategoryProvider.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, added);
        }

        /// <summary>
        /// Edit existing variable category
        /// </summary>
        /// <remarks>
        /// Edit existing variable category<br></br>
        /// <strong>Purpose.</strong>        
        /// - This api is used to update an existing variable category.
        /// - This api takes EditCategory and guid of variable category as input request.
        /// - This api returns VariableCategoryViewModel of updated variable category in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editCategory">EditCategory</param>
        /// <param name="guid">Guid of variable category that needs to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "An records already exist with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(VariableCategoryViewModel))]
        [SwaggerRequestExample(typeof(EditCategory), typeof(EditCategoryExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(VariableCategoryViewModelExamples))]
        // PUT: api/VariableCategory/5
        public HttpResponseMessage Put(Guid guid, [FromBody]EditCategory editCategory)
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
            var response =  _VariableCategoryProvider.Update(new VariableCategoryViewModel() {
                CategoryName = editCategory.Category,
                Guid = guid,
                ModifiedBy = LoggedInUserId,
                ModifiedDate=DateTime.UtcNow
              
            });
            _VariableCategoryProvider.SaveChanges();
            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Variable Category was not found.");
        }

        /// <summary>
        /// Delete existing variable category
        /// </summary>
        /// <remarks>
        /// Delete existing variable category<br></br> 
        /// <strong>Purpose.</strong>
        /// - This api is used to delete an existing variable category by its guid.
        /// - This api returns VariableCategoryViewModel of deleted variable category in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of variable category that needs to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(VariableCategoryViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(VariableCategoryViewModelExamples))]
        // DELETE: api/VariableCategory/5
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
            var response = _VariableCategoryProvider.DeleteByGuid(guid, this.LoggedInUserId);
            _VariableCategoryProvider.SaveChanges();

            if (response != null)
                
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Variable Category was not found.");
        }
    }
}