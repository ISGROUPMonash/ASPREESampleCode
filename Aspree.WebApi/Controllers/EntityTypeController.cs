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
    public class EntityTypeController : BaseController
    {
        private readonly IEntityTypeProvider _entityTypeProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityTypeProvider"></param>
        public EntityTypeController(IEntityTypeProvider entityTypeProvider)
        {
            _entityTypeProvider = entityTypeProvider;
        }

        /// <summary>
        /// Get all entity types
        /// </summary>
        /// <remarks>
        /// Get all entity types<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get list of all entity types.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<EntityTypeViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllEntityTypeViewModelExamples))]
        public IEnumerable<EntityTypeViewModel> GetAll()
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
            return _entityTypeProvider.GetAll(this.LoggedInUserTenantId);
        }

        /// <summary>
        /// Get entity type by guid
        /// </summary>
        /// <remarks>
        /// Get entity type by guid<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get entity type details based on its guid.
        /// - Entity type will be fetched from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of an entity type that to be fetched</param>
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EntityTypeViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(EntityTypeViewModelExamples))]
        public EntityTypeViewModel Get(Guid guid)
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
            var checkList = _entityTypeProvider.GetByGuid(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Entity Type was not found."),
                    ReasonPhrase = "Entity Type was not found."
                });
            }

            return checkList;
        }

        /// <summary>
        /// Add new entity type
        /// </summary>
        /// <remarks>
        /// Add new entity type<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to create new entity type for the system.
        /// - The new entity type will be saved into SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newEntityTypeViewModel">New EntityTypeViewModel Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        // [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EntityTypeViewModel))]
        [SwaggerRequestExample(typeof(NewEntityTypeViewModel), typeof(NewEntityTypeViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(EntityTypeViewModelExamples))]
        public HttpResponseMessage Post([FromBody]NewEntityTypeViewModel newEntityTypeViewModel)
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

            var added =  _entityTypeProvider.Create(new EntityTypeViewModel()
            {
                Name = newEntityTypeViewModel.Name,
                Guid = Guid.NewGuid(),
                TenantId = this.LoggedInUserTenantId,
            });

            _entityTypeProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, added);
        }

        /// <summary>
        /// Edit existing entity type
        /// </summary>
        /// <remarks>
        /// Edit existing entity type<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to update entity type based on its guid.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editEntityType">Edit EntityType Model</param>
        /// <param name="guid">guid of entity type that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EntityTypeViewModel))]
        [SwaggerRequestExample(typeof(EditEntityTypeViewModel), typeof(EditEntityTypeViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(EntityTypeViewModelExamples))]

        public HttpResponseMessage Put(Guid guid, [FromBody]EditEntityTypeViewModel editEntityType)
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

            var updated = _entityTypeProvider.Update(new EntityTypeViewModel()
            {
                Guid = guid,
                Name = editEntityType.Name
            });

            _entityTypeProvider.SaveChanges();

            if (updated != null)
                return Request.CreateResponse(HttpStatusCode.OK, updated);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Entity Type was not found.");

        }

        /// <summary>
        /// Delete existing entity type
        /// </summary>
        /// <remarks>
        /// Delete existing entity type<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to delete an existing entity type by its guid.
        /// - The api will delete data from SQL table(EntityType).
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of entity type that is to be deleted</param>
        //[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EntityTypeViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(EntityTypeViewModelExamples))]

        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
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
            var response = _entityTypeProvider.DeleteByGuid(guid, this.LoggedInUserId);
            _entityTypeProvider.SaveChanges();

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Entity Type was not found.");
        }
    }
}
