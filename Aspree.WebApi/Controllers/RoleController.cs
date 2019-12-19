using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Aspree.Core.ViewModels;
using Aspree.Provider.Interface;
using Swashbuckle.Swagger.Annotations;
using System.Web.Http.Description;
using System.Linq;
using Aspree.WebApi.Utilities;
using Swashbuckle.Examples;

namespace Aspree.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    //[Authorize(Roles = "Definition Admin")]
    public class RoleController : BaseController
    {
        private readonly IRoleProvider _roleProvider;

        ///<summary>
        /// 
        ///</summary>
        ///<param name="roleProvider"></param>
        public RoleController(IRoleProvider roleProvider)
        {
            _roleProvider = roleProvider;
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        /// <remarks>
        /// Get all roles<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to get all roles.
        /// - This api returns list of RoleModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <returns>all roles</returns>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<RoleModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllRoleModelExamples))]
        public IEnumerable<RoleModel> GetAll()
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
            return _roleProvider.GetAll(this.LoggedInUserTenantId);
        }


        /// <summary>
        /// Get role by guid
        /// </summary>
        /// <remarks>
        /// Get role by guid<br></br>   
        /// <strong>Purpose.</strong>
        /// - This api is used to get role by its guid.
        /// - This api returns RoleModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of Role</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(RoleModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(RoleModelExamples))]
        public RoleModel Get(Guid guid)
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
            var role = _roleProvider.GetByGuid(guid);
            if (role == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Role was not found."),
                });
            }

            return role;
        }


        /// <summary>
        /// Add new role
        /// </summary>
        /// <remarks>
        /// Add new role<br></br>  
        /// <strong>Purpose.</strong>
        /// - This api is used to create new role.
        /// - This api takes NewRoleModel for role as input request.
        /// - This api returns RoleModel of newly created role in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newRoleModel">RoleModel</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "An records already exist with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(RoleModel))]
        [SwaggerRequestExample(typeof(NewRoleModel), typeof(NewRoleModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(RoleModelExamples))]
        public HttpResponseMessage Post([FromBody]NewRoleModel newRoleModel)
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
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.AllErrors());
            }

            var newRole = _roleProvider.Create(new RoleModel()
            {
                CreatedBy = LoggedInUserId,
                CreatedDate = DateTime.UtcNow,
                Guid = Guid.NewGuid(),
                IsSystemRole = false,
                Name = newRoleModel.Name,
                Privileges = newRoleModel.Privileges,

                TenantId=newRoleModel.TenantId,
            });

            return Request.CreateResponse(HttpStatusCode.OK, newRole);
        }

        /// <summary>
        /// Edit existing role
        /// </summary>
        /// <remarks>
        /// Edit existing role<br></br>
        /// <strong>Purpose.</strong>        
        /// - This api is used to update an existing role.
        /// - This api takes EditRoleModel and guid of role as input request.
        /// - This api returns RoleModel of updated role in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editRoleModel">EditRoleModel</param>
        /// <param name="guid">Guid of role to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "An records already exist with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(RoleModel))]
        [SwaggerRequestExample(typeof(EditRoleModel), typeof(EditRoleModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(RoleModelExamples))]
        public HttpResponseMessage Put(Guid guid, [FromBody]EditRoleModel editRoleModel)
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
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.AllErrors());
            }

            var editedRole = _roleProvider.Update(new RoleModel()
            {
                Guid = guid,
                Name = editRoleModel.Name,
                ModifiedBy = LoggedInUserId,
                ModifiedDate = DateTime.UtcNow,
                Privileges = editRoleModel.Privileges
            });

            if (editedRole == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Role was not found.");
            }

            _roleProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, editedRole);
        }

        /// <summary>
        /// Delete existing role
        /// </summary>
        /// <remarks>
        /// Delete existing role<br></br> 
        /// <strong>Purpose.</strong>
        /// - This api is used to delete an existing role.
        /// - This api takes guid of a role that needs to be deleted as input request.
        /// - This api returns RoleModel of deleted role in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of the role to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(RoleModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(RoleModelExamples))]
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
            var deletedRole = _roleProvider.DeleteByGuid(guid, this.LoggedInUserId);
            _roleProvider.SaveChanges();

            if (deletedRole != null)
                return Request.CreateResponse(HttpStatusCode.OK, deletedRole);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Role was not found.");
        }
    }
}