using Aspree.Core.ViewModels;
using Aspree.Provider.Interface;
using Swashbuckle.Examples;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Aspree.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class VariableTypeController : BaseController
    {
        private readonly IVariableTypeProvider _variableTypeProvider;

        /// <summary>
        /// 
        /// </summary>
        public VariableTypeController(IVariableTypeProvider variableTypeProvider)
        {
            this._variableTypeProvider = variableTypeProvider;
        }

        /// <summary>
        /// Get all variable types
        /// </summary>
        /// <remarks>
        /// Get all variable types<br></br>
        /// <strong>Purpose.</strong>
        /// - The purpose of this api is to get all variable types which will be fetched from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<VariableTypeViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllVariableTypeViewModelExample))]
        //[SwaggerResponseExample(HttpStatusCode.OK, typeof(IEnumerable<VariableTypeViewModelExample>))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // GET: api/VariableType
        public IEnumerable<VariableTypeViewModel> GetAll()
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
            return _variableTypeProvider.GetAll();
        }

        /// <summary>
        /// Get variable type by guid
        /// </summary>
        /// <remarks>
        /// Get variable type by guid<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get variable type based on its guid which will be fetched from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of a variable type that needs to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(VariableTypeViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(VariableTypeViewModelExample))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // GET: api/VariableType/5
        public VariableTypeViewModel Get(Guid guid)
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
            var checkList = _variableTypeProvider.GetByGuid(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Variable type was not found."),
                });
            }
            return checkList;
        }

        /// <summary>
        /// Add new variable type
        /// </summary>
        /// <remarks>
        /// Add new variable type<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to create new variable type into the system.
        /// - Variable type will be saved with its associated variable type id into SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newVariableType">NewVariableType Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "record  already exist with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(VariableTypeViewModel))]
        [SwaggerRequestExample(typeof(NewVariableType), typeof(NewVariableTypeExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(VariableTypeViewModelExample))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // POST: api/VariableType
        public HttpResponseMessage Post([FromBody]NewVariableType newVariableType)
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
            var addedVariableType = _variableTypeProvider.Create(new VariableTypeViewModel()
            {
                Status = 1,
                Type = newVariableType.Type
            });
            _variableTypeProvider.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, addedVariableType);
        }

        /// <summary>
        /// Edit existing variable type
        /// </summary>
        /// <remarks>
        /// Edit existing variable type<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to edit/update an existing variable type by its guid.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editVariableType">EditVariableType model</param>
        /// <param name="guid">Guid of variable type that needs to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "record  already exist with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(VariableTypeViewModel))]
        [SwaggerRequestExample(typeof(EditVariableType), typeof(EditVariableTypeExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(VariableTypeViewModelExample))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // PUT: api/VariableType/5
        public HttpResponseMessage Put(Guid guid, [FromBody]EditVariableType editVariableType)
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
            var updatedType = _variableTypeProvider.Update(new VariableTypeViewModel()
            {
                Type = editVariableType.Type,
                Guid = guid
            });
            if (updatedType == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Variable type was not found.");
            }
            _variableTypeProvider.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, updatedType);
        }

        /// <summary>
        /// Delete existing variable type
        /// </summary>
        /// <remarks>
        /// Delete existing variable type<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to delete an existing variable type by its guid.
        /// - The data will be deleted from SQL table.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of variable type that needs to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(VariableTypeViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(VariableTypeViewModelExample))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        // DELETE: api/VariableType/5
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
            var deletedCategory = _variableTypeProvider.DeleteByGuid(guid, this.LoggedInUserId);
            _variableTypeProvider.SaveChanges();

            if (deletedCategory != null)
                return Request.CreateResponse(HttpStatusCode.OK, deletedCategory);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Variable type was not found.");
        }
    }
}
