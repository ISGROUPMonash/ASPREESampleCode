using Aspree.Core.ViewModels;
using Aspree.Provider.Interface;
using Aspree.WebApi.Utilities;
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
    public class VariableController : BaseController
    {
        private readonly IVariableProvider _variableProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="variableProvider"></param>
        public VariableController(IVariableProvider variableProvider)
        {
            _variableProvider = variableProvider;
        }

        /// <summary>
        /// Get all variables
        /// </summary>
        /// <remarks>
        /// Get all variables<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to get all variables.
        /// - This api returns list of VariableViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<VariableViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllVariableViewModelExamples))]
        [HttpGet]
        public IEnumerable<VariableViewModel> GetAll()
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
            return _variableProvider.GetAll(this.LoggedInUserTenantId);
        }

        /// <summary>
        /// Get variable by guid
        /// </summary>
        /// <remarks>
        /// Get variable by guid<br></br> 
        /// <strong>Purpose.</strong>
        /// - This api is used to get variable by its guid.
        /// - This api returns VariableViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of a variable that needs to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(VariableViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(VariableViewModelExamples))]
        [HttpGet]
        public VariableViewModel Get(Guid guid)
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
            var checkList = _variableProvider.GetByGuid(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Variable was not found."),
                });
            }
            return checkList;
        }

        /// <summary>
        /// Add new variable
        /// </summary>
        /// <remarks>
        /// Add new variable<br></br>  
        /// <strong>Purpose.</strong>
        /// - This api is used for create new Variable.
        /// - This api takes NewVariableViewModel of a variable as input request.
        /// - This api returns VariableViewModel of new created variable in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newVariable">New Variable Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(VariableViewModel))]
        [SwaggerRequestExample(typeof(NewVariableViewModel), typeof(NewVariableViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(VariableViewModelExamples))]
        [HttpPost]
        public HttpResponseMessage Post([FromBody]NewVariableViewModel newVariable)
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

            var addedVariable = _variableProvider.Create(new VariableViewModel()
            {
                CreatedBy = this.LoggedInUserId,
                VariableName = newVariable.VariableName,
                CanCollectMultiple = newVariable.CanCollectMultiple,
                DependentVariableId = newVariable.DependentVariableId,
                HelpText = newVariable.HelpText,
                IsApproved = newVariable.IsApproved,
                IsRequired = newVariable.IsRequired,
                IsSoftRange = newVariable.IsSoftRange,
                MaxRange = newVariable.MaxRange,
                MinRange = newVariable.MinRange,
                Question = newVariable.Question,
                RegEx = newVariable.RegEx,
                RequiredMessage = newVariable.RequiredMessage,
                ValidationMessage = newVariable.ValidationMessage,
                ValidationRuleId = newVariable.ValidationRuleId,
                ValueDescription = newVariable.ValueDescription,
                Values = newVariable.Values,
                VariableCategoryId = newVariable.VariableCategoryId,
                VariableLabel = newVariable.VariableLabel,
                VariableTypeId = newVariable.VariableTypeId,
                VariableRoles = newVariable.VariableRoles,
                Comment = newVariable.Comment,
                TenantId = this.LoggedInUserTenantId,
                ValidationRuleIds = newVariable.ValidationRuleIds,
                CustomRegEx = newVariable.CustomRegEx,
                VariableValueDescription = newVariable.VariableValueDescription,
                OutsideRangeValidation = newVariable.OutsideRangeValidation,
                MissingValidation = newVariable.MissingValidation,
                LookupEntityType = newVariable.LookupEntityType,
                LookupEntitySubtype = newVariable.LookupEntitySubtype,
                DateFormat = newVariable.DateFormat,
                CanFutureDate = newVariable.CanFutureDate,
            });
            _variableProvider.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
        }


        /// <summary>
        /// Edit existing variable
        /// </summary>
        /// <remarks>
        /// Edit existing variable<br></br>      
        /// <strong>Purpose.</strong>        
        /// - This api is used to update an existing variable.
        /// - This api takes EditVariableViewModel and guid of variable as input request.
        /// - This api returns VariableViewModel of updated variable in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editVariable">EditVariableViewModel</param>
        /// <param name="guid">Guid of variable that needs to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "records already exist with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(VariableViewModel))]
        [SwaggerRequestExample(typeof(EditVariableViewModel), typeof(EditVariableViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(VariableViewModelExamples))]
        [HttpPut]
        public HttpResponseMessage Put(Guid guid, [FromBody]EditVariableViewModel editVariable)
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

            var updatedVariable = _variableProvider.Update(new VariableViewModel()
            {
                ModifiedBy = LoggedInUserId,
                ModifiedDate = DateTime.UtcNow,
                Guid = guid,
                VariableName = editVariable.VariableName,
                CanCollectMultiple = editVariable.CanCollectMultiple,
                DependentVariableId = editVariable.DependentVariableId,
                HelpText = editVariable.HelpText,
                IsApproved = editVariable.IsApproved,
                IsRequired = editVariable.IsRequired,
                IsSoftRange = editVariable.IsSoftRange,
                MaxRange = editVariable.MaxRange,
                MinRange = editVariable.MinRange,
                Question = editVariable.Question,
                RegEx = editVariable.RegEx,
                RequiredMessage = editVariable.RequiredMessage,
                ValidationMessage = editVariable.ValidationMessage,
                ValidationRuleId = editVariable.ValidationRuleId,
                ValueDescription = editVariable.ValueDescription,
                Values = editVariable.Values,
                VariableCategoryId = editVariable.VariableCategoryId,
                VariableLabel = editVariable.VariableLabel,
                VariableTypeId = editVariable.VariableTypeId,
                VariableRoles = editVariable.VariableRoles,
                Comment = editVariable.Comment,
                ValidationRuleIds = editVariable.ValidationRuleIds,
                CustomRegEx = editVariable.CustomRegEx,
                VariableValueDescription = editVariable.VariableValueDescription,
                IsVariableLogTable = editVariable.IsVariableLogTable,
                OutsideRangeValidation = editVariable.OutsideRangeValidation,
                MissingValidation = editVariable.MissingValidation,
                LookupEntityType = editVariable.LookupEntityType,
                LookupEntitySubtype = editVariable.LookupEntitySubtype,
                DateFormat = editVariable.DateFormat,
                CanFutureDate = editVariable.CanFutureDate,
            });
            if (updatedVariable == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Variable was not found.");
            }
            _variableProvider.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, updatedVariable);
        }

        /// <summary>
        /// Delete existing variable
        /// </summary>
        /// <remarks>
        /// Delete existing variable<br></br> 
        /// <strong>Purpose.</strong>
        /// - The purpose of this api is to delete an existing variable by its guild.
        /// - Variable will be soft deleted from SQL table(variable)
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of variable that needs to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(VariableViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(VariableViewModelExamples))]
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
            var deletedCategory = _variableProvider.DeleteByGuid(guid, LoggedInUserId);
            _variableProvider.SaveChanges();

            if (deletedCategory != null)
                return Request.CreateResponse(HttpStatusCode.OK, deletedCategory);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Variable was not found.");
        }



        /// <summary>
        /// Get project builder variables
        /// </summary>
        /// <remarks>
        /// Get project builder variables<br></br>   
        /// <strong>Purpose.</strong>
        /// - This api is used for return all details of variable page in project builder.
        /// - This api return all existing Forms, VariableType, ValidationRule, VariableCategory and roles.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="projectId">Guid of a project that variables to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectBuilderVariablesViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ProjectBuilderVariablesViewModelExamples))]
        [Route("api/v1/Variable/ProjectBuilderVariables/{projectId}")]
        [HttpGet]
        public ProjectBuilderVariablesViewModel ProjectBuilderVariables(Guid projectId)
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
            return _variableProvider.GetProjectBuilderVariables(this.LoggedInUserTenantId, this.LoggedInUserId, projectId);
        }

        /// <summary>
        /// Get variables by project guid
        /// </summary>
        /// <remarks>
        /// Get variables by project guid<br></br> 
        /// <strong>Purpose.</strong>
        /// - This api is used for return all form variables by project id.
        /// - This api return list of ProjectBuilderFormViewModelViewModel as response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of a project that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]        
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<ProjectBuilderFormViewModelViewModel>))]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/v1/Variable/GetFormVariableByProjectId/{guid}")]
        [HttpGet]
        public IEnumerable<ProjectBuilderFormViewModelViewModel> GetFormVariableByProjectId(Guid guid)
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
            var checkList = _variableProvider.GetFormVariableByProjectId(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Variable was not found."),
                });
            }
            return checkList;
        }

        /// <summary>
        /// Get all variables for form page
        /// </summary>
        /// <remarks>
        /// Get all variables for form page<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to get all variable in project.
        /// - This api return list of VariableViewModel as response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="projectId">project id</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<VariableViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllVariableViewModelExamples))]
        [HttpGet]
        [Route("api/v1/Variable/GetAllVariables/{projectId}")]
        public IEnumerable<VariableViewModel> GetAllVariables(Guid projectId)
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
            return _variableProvider.GetAllVariables(this.LoggedInUserTenantId, projectId);
        }

        /// <summary>
        /// Get variable by guid and projct id and userloginid
        /// </summary>
        /// <remarks>
        /// Get variable by guid<br></br> 
        /// <strong>Purpose.</strong>
        /// - This api is used to get variable by its guid.
        /// - This api returns VariableViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of a variable that needs to be fetched</param>
        /// <param name="projectid">projectid of a variable that needs to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(VariableViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(VariableViewModelExamples))]
        [HttpGet]
        [Route("api/v1/Variable/Getactivity/{guid}/{projectid}")]
        public VariableViewModel GetVariables(Guid guid, Guid projectid)
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
            var checkList = _variableProvider.GetVariablesByGuid(guid, this.LoggedInUserId, projectid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Variable was not found."),
                });
            }
            return checkList;
        }
    }
}
