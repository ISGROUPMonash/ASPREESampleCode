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
    public class EntityController : BaseController
    {
        private readonly IEntityProvider _entityProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityProvider"></param>
        public EntityController(IEntityProvider entityProvider)
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
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<EntityViewModel>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public IEnumerable<EntityViewModel> GetAll()
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
            return _entityProvider.GetAll(this.LoggedInUserTenantId);
        }

        /// <summary>
        /// Get Entity By Guid
        /// </summary>
        /// <remarks>
        /// Get Entity By Guid
        /// </remarks>
        /// <param name="guid">Guid of an Entity that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EntityViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public EntityViewModel Get(Guid guid)
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
            var checkList = _entityProvider.GetByGuid(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Entity was not found."),
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
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EntityViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPost]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage Post([FromBody]NewEntityViewModel newEntity)
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

            var addedEntity = _entityProvider.Create(new EntityViewModel()
            {
                CreatedBy = LoggedInUserId,
                EntitySubTypeId = newEntity.EntitySubTypeId,
                EntityTypeId = newEntity.EntityTypeId,
                Name = newEntity.Name,
                ParentEntityId = newEntity.ParentEntityId,
                Status = (int)Core.Enum.Status.Active,
                DroppedVariablesList = newEntity.DroppedVariablesList,
                TenantId = newEntity.TenantId,
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
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EntityViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPut]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage Put(Guid guid, [FromBody]EditEntityViewModel editEntity)
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

            var updatedEntity = _entityProvider.Update(new EntityViewModel()
            {
                ModifiedBy = LoggedInUserId,
                ModifiedDate = DateTime.UtcNow,
                Guid = guid,
                EntitySubTypeId = editEntity.EntitySubTypeId,
                EntityTypeId = editEntity.EntityTypeId,
                Name = editEntity.Name,
                ParentEntityId = editEntity.ParentEntityId,
                Status = (int)Core.Enum.Status.Active,

                DroppedVariablesList = editEntity.DroppedVariablesList,
                TenantId = editEntity.TenantId,
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
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EntityViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpDelete]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
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
            var deletedCategory = _entityProvider.DeleteByGuid(guid, LoggedInUserId);
            _entityProvider.SaveChanges();

            if (deletedCategory != null)
                return Request.CreateResponse(HttpStatusCode.OK, deletedCategory);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Entity was not found.");
        }




        /// <summary>
        /// Get Entity By Tenant Guid
        /// </summary>
        /// <remarks>
        /// Get Entity By Tenant Guid
        /// </remarks>
        /// <param name="guid">Guid of an Entity that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EntityViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/v1/Entity/GetByTenantId/{guid}")]
        [HttpGet]
        public EntityViewModel GetByTenantId(Guid guid)
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
            var checkList = _entityProvider.GetByTenantGuid(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Entity was not found."),
                });
            }

            return checkList;
        }




        /// <summary>
        /// Get Entity variable By Entity Type and Sub Type Guid
        /// </summary>
        /// <remarks>
        /// Get Entity variable By Entity Type and Sub Type Guid
        /// </remarks>
        /// <param name="tenantGuid">Guid of Tenant that to be fetched</param>
        /// <param name="typeGuid">Guid of an EntityType that to be fetched</param>
        /// <param name="subTypeGuid">Guid of an EntitySubtype that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EntityViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/v1/Entity/GetByEntityTypeAndSubTypeId/{tenantGuid}/{typeguid}/{subtypeguid}")]
        [HttpGet]
        public EntityViewModel GetByEntityTypeAndSubTypeId(Guid tenantGuid, Guid typeGuid, Guid? subTypeGuid = null)
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
            var checkList = _entityProvider.GetByEntityTypeAndSubTypeGuid(tenantGuid, typeGuid, subTypeGuid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Entity was not found."),
                });
            }

            return checkList;
        }
        /// <summary>
        /// Get all entities created by search page.
        /// </summary>
        /// <remarks>
        /// Get all entities created by search page.<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to display enities into system admin tools.
        /// - This api will get all person entity, participant entity, place/group entity, project entity created into the system from the search page.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "This error is returned by the server when we pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided filters are found")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<EntityViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllEntityViewModelExamples))]
        [HttpGet]
        [Route("api/v1/Entity/GetAllEntitiesCreatedBySearch")]
        public IEnumerable<EntityViewModel> GetAllEntitiesCreatedBySearch()
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
            return _entityProvider.GetAllEntitiesCreatedBySearch();
        }

    }
}
