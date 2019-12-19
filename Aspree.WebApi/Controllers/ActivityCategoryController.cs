using Aspree.Core.ViewModels;
using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Swashbuckle.Swagger.Annotations;
using Swashbuckle.Examples;

namespace Aspree.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ActivityCategoryController : BaseController
    {
        private readonly IActivityCategoryProvider _activityCategoryProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activityCategoryProvider"></param>
        public ActivityCategoryController(IActivityCategoryProvider activityCategoryProvider)
        {
            _activityCategoryProvider = activityCategoryProvider;
        }

        /// <summary>
        /// Get all activity categories
        /// </summary>
        /// <remarks>
        /// Get all activity categories<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to get all activity categories.
        /// - This api returns list of ActivityCategoryViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        // GET: api/v1/ActivityCategory
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<ActivityCategoryViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllActivityCategoryViewModelExamples))]
        [HttpGet]
        public IEnumerable<ActivityCategoryViewModel> GetAll()
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
            return _activityCategoryProvider.GetAll(this.LoggedInUserTenantId);
        }

        /// <summary>
        /// Get activity category by guid
        /// </summary>
        /// <remarks>
        /// Get activity category by guid<br></br>   
        /// <strong>Purpose.</strong>
        /// - This api is used to get an activity category by its guid.
        /// - This api returns ActivityCategoryViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of an activity category that has to be fetched</param>
        // GET: api/ActivityCategory/5
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]        
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityCategoryViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ActivityCategoryViewModelExamples))]
        [HttpGet]
        public ActivityCategoryViewModel Get(Guid guid)
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
            var checkList = _activityCategoryProvider.GetByGuid(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Activity category was not found."),
                });
            }

            return checkList;
        }


        /// <summary>
        /// Add new activity category
        /// </summary>
        /// <remarks>
        /// Add new activity category<br></br>  
        /// <strong>Purpose.</strong>
        /// - This api is used to create a new activity category.
        /// - This api returns the newly created ActivityCategoryViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newCategory">NewCategory Model</param>
        // POST: api/ActivityCategory
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]                
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityCategoryViewModel))]
        [SwaggerRequestExample(typeof(NewCategory), typeof(NewCategoryExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ActivityCategoryViewModelExamples))]
        [HttpPost]
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

            var addedCategory = _activityCategoryProvider.Create(new ActivityCategoryViewModel()
            {
                CategoryName = newCategory.Category,
                CreatedBy = LoggedInUserId,
                TenantId = this.LoggedInUserTenantId,
            });

            _activityCategoryProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, addedCategory);
        }


        /// <summary>
        /// Edit existing activity category
        /// </summary>
        /// <remarks>
        /// Edit existing activity category<br></br> 
        /// <strong>Purpose.</strong>        
        /// - This api is used to edit an existing activity category.
        /// - This api takes guid of an activity category and EditCategory model as input request.
        /// - This api returns updated ActivityCategoryViewModel model in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editCategory">EditCategory model</param>
        /// <param name="guid">Guid of activity category that needs to be edited</param>
        // PUT: api/ActivityCategory/5
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]        
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityCategoryViewModel))]
        [SwaggerRequestExample(typeof(EditCategory), typeof(EditCategoryExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ActivityCategoryViewModelExamples))]
        [HttpPut]
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

            var updatedCategory = _activityCategoryProvider.Update(new ActivityCategoryViewModel()
            { ModifiedBy = LoggedInUserId,
                ModifiedDate = DateTime.UtcNow,
                CategoryName = editCategory.Category,
                Guid = guid
            });

            if (updatedCategory == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Activity category was not found.");
            }

            _activityCategoryProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, updatedCategory);

        }


        /// <summary>
        /// Delete existing activity category
        /// </summary>
        /// <remarks>
        /// Delete existing activity category<br></br> 
        /// <strong>Purpose.</strong>
        /// - This api is used to delete an existing activity category.
        /// - This api takes guid of activity category to be deleted as an input parameter.
        /// - This api returns ActivityCategoryViewModel of deleted activity category in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of activity category that is to be deleted</param>
        // DELETE: api/ActivityCategory/5
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]                
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ActivityCategoryViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ActivityCategoryViewModelExamples))]
        [HttpDelete]
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
            var deletedCategory = _activityCategoryProvider.DeleteByGuid(guid, LoggedInUserId);
            _activityCategoryProvider.SaveChanges();

            if (deletedCategory != null)
                return Request.CreateResponse(HttpStatusCode.OK, deletedCategory);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Activity category was not found.");
        }
    }
}
