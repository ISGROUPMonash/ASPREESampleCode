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
    public class EntitySubTypeController : BaseController
    {
        private readonly IEntitySubTypeProvider _entitySubTypeProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entitySubTypeProvider"></param>
        public EntitySubTypeController(IEntitySubTypeProvider entitySubTypeProvider)
        {
            _entitySubTypeProvider = entitySubTypeProvider;
        }

        /// <summary>
        /// Get all entity sub types
        /// </summary>
        /// <remarks>
        /// Get all entity sub types<br></br>
        /// <strong>Purpose.</strong>
        /// - The purpose of this api is to get the list of all entity subtypes.
        /// - The api fetches entity subtypes from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<EntitySubTypeViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllEntitySubTypeViewModelExamples))]
        public IEnumerable<EntitySubTypeViewModel> GetAll()
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
            return _entitySubTypeProvider.GetAll();

        }

        /// <summary>
        /// Get entity subtype by guid
        /// </summary>
        /// <remarks>
        /// Get entity subtype by guid<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get an entity subtype based on its guid.
        /// - Entity subtype will be fetched from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of an entity sub type that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]   
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EntitySubTypeViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(EntitySubTypeViewModelExamples))]
        public EntitySubTypeViewModel Get(Guid guid)
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
            var checkList = _entitySubTypeProvider.GetByGuid(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Entity sub type was not found."),
                    ReasonPhrase = "Entity sub type was not found."
                });
            }

            return checkList;
        }

        /// <summary>
        /// Add new entity subtype
        /// </summary>
        /// <remarks>
        ///  Add new entity subtype<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to create new entity subtype into the system.
        /// - Entity subtype will be saved in SQL database with its associated entity type id.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newEntitySubTypeViewModel">New EntitySubTypeViewModel Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EntitySubTypeViewModel))]
        [SwaggerRequestExample(typeof(NewEntitySubTypeViewModel), typeof(NewEntitySubTypeViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(EntitySubTypeViewModelExamples))]
        public HttpResponseMessage Post([FromBody]NewEntitySubTypeViewModel newEntitySubTypeViewModel)
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

           var added =  _entitySubTypeProvider.Create(new EntitySubTypeViewModel()
            {
                EntityTypeId = newEntitySubTypeViewModel.EntityTypeId,
                Name = newEntitySubTypeViewModel.Name
            });

            _entitySubTypeProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, added);
        }

        /// <summary>
        /// Edit existing entity subtype
        /// </summary>
        /// <remarks>
        /// Edit existing entity subtype<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to edit/update an existing entity subtype using its guid.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editEntitySubType">Edit EntitySubType Model</param>
        /// <param name="guid">guid of entity sub type that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EntitySubTypeViewModel))]
        [SwaggerRequestExample(typeof(EditEntitySubTypeViewModel), typeof(EditEntitySubTypeViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(EntitySubTypeViewModelExamples))]
        public HttpResponseMessage Put(Guid guid,[FromBody]EditEntitySubTypeViewModel editEntitySubType)
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

            var updated = _entitySubTypeProvider.Update(new EntitySubTypeViewModel()
            {
                EntityTypeId = editEntitySubType.EntityTypeId,
                Guid = guid,
                Name = editEntitySubType.Name
            });

            _entitySubTypeProvider.SaveChanges();

            if(updated!=null)
                return Request.CreateResponse(HttpStatusCode.OK, updated);

            return Request.CreateResponse(HttpStatusCode.NotFound, "Entity sub type was not found.");
        }

        /// <summary>
        /// Delete existing entity subtype
        /// </summary>
        /// <remarks>
        /// Delete existing entity subtype<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to delete an existing entity subtype by its guid.
        /// - The api will delete data from SQL table(EntitySubType).
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of entity sub type that is to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EntitySubTypeViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(EntitySubTypeViewModelExamples))]
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
            var response = _entitySubTypeProvider.DeleteByGuid(guid, this.LoggedInUserId);
            _entitySubTypeProvider.SaveChanges();

            if (response != null)
                return Request.CreateResponse(HttpStatusCode.OK, response);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Entity sub type was not found.");
        }
    }
}
