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
    [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
    public class EntityFormDataVariableController : BaseController
    {

        private readonly IEntityFormDataVariableProvider _entityProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityProvider"></param>
        public EntityFormDataVariableController(IEntityFormDataVariableProvider entityProvider)
        {
            _entityProvider = entityProvider;
        }

        /// <summary>
        /// Get All Entities
        /// </summary>
        /// <remarks>
        /// Get All Entities
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<EntityFormDataVariableViewModel>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        public IEnumerable<EntityFormDataVariableViewModel> GetAll()
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
            return _entityProvider.GetAll();
        }

        /// <summary>
        /// Get Entity By Guid
        /// </summary>
        /// <remarks>
        /// Get Entity By Guid
        /// </remarks>
        /// <param name="guid">Guid of an Entity that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EntityFormDataVariableViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        public EntityFormDataVariableViewModel Get(Guid guid)
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
            var checkList = _entityProvider.GetByGuid(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Entity Form Data Variable was not found."),
                });
            }

            return checkList;
        }


        /// <summary>
        /// Add New Entity
        /// </summary>
        /// <remarks>
        /// Add New Entity
        /// </remarks>
        /// <param name="newEntity">New Entity Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EntityFormDataVariableViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPost]
        public HttpResponseMessage Post([FromBody]EntityFormDataVariableViewModel newEntity)
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

            var addedEntity = _entityProvider.Create(new EntityFormDataVariableViewModel()
            {
                CreatedBy = LoggedInUserId,
                EntitySubTypeId = newEntity.EntitySubTypeId,
                EntityTypeId = newEntity.EntityTypeId,
                EntityId = newEntity.EntityId,
                VariableId = 2,                
                SelectedValues = "",
                Json = newEntity.Json,
                VariableGuid = newEntity.VariableGuid,
                EntityTypeGuid=newEntity.EntityTypeGuid,
                EntitySubTypeGuid = newEntity.EntitySubTypeGuid,
                EntityGuid=newEntity.EntityGuid,
                EntityName=newEntity .EntityName,
            });
            _entityProvider.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, addedEntity);
        }


        /// <summary>
        /// Edit Existing Entity
        /// </summary>
        /// <remarks>
        /// Edit Existing Entity
        /// </remarks>
        /// <param name="editEntity">Edit Entity Model</param>
        /// <param name="guid">guid of Entity that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EntityFormDataVariableViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPut]
        public HttpResponseMessage Put(Guid guid, [FromBody]EntityFormDataVariableViewModel editEntity)
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
            var updatedEntity = _entityProvider.Update(new EntityFormDataVariableViewModel()
            {
                ModifiedBy = LoggedInUserId,
                ModifiedDate = DateTime.UtcNow,
                Guid = guid,
                CreatedBy = LoggedInUserId,
                EntitySubTypeId = editEntity.EntitySubTypeId,
                EntityTypeId = editEntity.EntityTypeId,
                EntityId = editEntity.EntityId,
                VariableId = 2,
                SelectedValues = "",
                Json = editEntity.Json,
                VariableGuid = editEntity.VariableGuid,
                EntityTypeGuid = editEntity.EntityTypeGuid,
                EntitySubTypeGuid = editEntity.EntitySubTypeGuid,
                EntityGuid = editEntity.EntityGuid,
                EntityName = editEntity.EntityName,
            });
            if (updatedEntity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Entity was not found.");
            }
            _entityProvider.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, updatedEntity);
        }


        /// <summary>
        /// Delete Existing Entity
        /// </summary>
        /// <remarks>
        /// Delete Existing Entity
        /// </remarks>
        /// <param name="guid">Guid of Entity that is to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EntityFormDataVariableViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpDelete]
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
            var deletedCategory = _entityProvider.DeleteByGuid(guid, LoggedInUserId);
            _entityProvider.SaveChanges();

            if (deletedCategory != null)
                return Request.CreateResponse(HttpStatusCode.OK, deletedCategory);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Entity was not found.");
        }
    }
}