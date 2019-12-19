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
    public class FormController : BaseController
    {
        // GET: api/Form
        private readonly IFormProvider _formProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formProvider"></param>
        public FormController(IFormProvider formProvider)
        {
            _formProvider = formProvider;
        }

        /// <summary>
        /// Get all forms
        /// </summary>
        /// <remarks>
        /// Get all forms<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to get all forms.
        /// - This api returns list of FormViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<FormViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllFormViewModelExamples))]
        [HttpGet]
        public IEnumerable<FormViewModel> GetAll()
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
            return _formProvider.GetAll(this.LoggedInUserTenantId);
        }

        /// <summary>
        /// Get forms by guid
        /// </summary>
        /// <remarks>
        /// Get forms by guid<br></br>  
        /// <strong>Purpose.</strong>
        /// - This api is used to get form by its guid.
        /// - This api returns FormViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of a forms that needs to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormViewModelExamples))]
        [HttpGet]
        public FormViewModel Get(Guid guid)
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
            var checkList = _formProvider.GetByGuid(guid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Form was not found."),
                });
            }

            return checkList;
        }


        /// <summary>
        /// Add new form
        /// </summary>
        /// <remarks>
        /// Add new form<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to create new form.
        /// - This api takes NewFormViewModel of a form as input request.
        /// - This api returns FormViewModel of created form in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newForm">New Forms Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]                
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormViewModel))]
        [SwaggerRequestExample(typeof(NewFormViewModel), typeof(NewFormViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormViewModelExamples))]
        [HttpPost]
        public HttpResponseMessage Post([FromBody]NewFormViewModel newForm)
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

            var addedVariable = _formProvider.Create(new FormViewModel()
            {
                CreatedBy = this.LoggedInUserId,
                ApprovedBy = newForm.ApprovedBy,
                ApprovedDate = newForm.ApprovedDate,
                CreatedDate = DateTime.UtcNow,
                FormCategoryId = newForm.FormCategoryId,
                Variables = newForm.Variables,
                FormTitle = newForm.FormTitle,
                TenantId = this.LoggedInUserTenantId,
                EntityTypes = newForm.EntityTypes,
                ProjectId = newForm.ProjectId,
            });

            _formProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
        }


        /// <summary>
        /// Edit existing forms
        /// </summary>
        /// <remarks>
        /// Edit existing formss<br></br>
        /// <strong>Purpose.</strong>        
        /// - This api is used to update an existing form.
        /// - This api takes EditFormViewModel and guid of form as input request.
        /// - This api returns FormViewModel of updated form in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editForm">Edit Forms Model</param>
        /// <param name="guid">guid of Forms that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]                
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormViewModel))]
        [SwaggerRequestExample(typeof(EditFormViewModel), typeof(EditFormViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormViewModelExamples))]
        [HttpPut]
        public HttpResponseMessage Put(Guid guid, [FromBody]EditFormViewModel editForm)
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

            var updatedVariable = _formProvider.Update(new FormViewModel()
            {
                ModifiedBy = LoggedInUserId,
                ModifiedDate = DateTime.UtcNow,
                ApprovedBy = editForm.ApprovedBy,
                ApprovedDate = editForm.ApprovedDate,
                FormCategoryId = editForm.FormCategoryId,
                Variables = editForm.Variables,
                FormTitle = editForm.FormTitle,
                TenantId = this.LoggedInUserTenantId,
                Guid = guid,
                EntityTypes = editForm.EntityTypes,
                ProjectId = editForm.ProjectId,
                IsPublished = editForm.IsPublished,
            });

            if (updatedVariable == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Form was not found.");
            }

            _formProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, updatedVariable);
        }
        
        /// <summary>
        /// Delete existing forms
        /// </summary>
        /// <remarks>
        /// Delete existing formss<br></br> 
        /// <strong>Purpose.</strong>
        /// - This api is used to delete an existing form.
        /// - This api takes guid of a form that needs to be deleted as input request.
        /// - This api returns FormViewModel of deleted form in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of Forms that needs to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormViewModelExamples))]
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
            var deletedCategory = _formProvider.DeleteByGuid(guid, LoggedInUserId);
            _formProvider.SaveChanges();

            if (deletedCategory != null)
                return Request.CreateResponse(HttpStatusCode.OK, deletedCategory);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Form was not found.");
        }



        /// <summary>
        /// Get project builder forms
        /// </summary>
        /// <remarks>
        /// Get project builder forms<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to return all details of form page in project builder.
        /// - This api returns all existing forms, variables, entity types and roles in response.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="projectId">Guid of a project that forms to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectBuilderFormsViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ProjectBuilderFormsViewModelExamples))]
        [Route("api/v1/Form/ProjectBuilderForms/{projectId}")]
        [HttpGet]
        public ProjectBuilderFormsViewModel ProjectBuilderForms(Guid projectId)
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
            return _formProvider.GetProjectBuilderForms(this.LoggedInUserTenantId, this.LoggedInUserId, projectId);
        }
        
        
        /// <summary>
        /// Get All default Forms
        /// </summary>
        /// <remarks>
        /// Get All default Forms
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<FormViewModel>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/v1/Form/GetAllDefaultForms")]
        [HttpGet]
        public IEnumerable<FormViewModel> GetAllDefaultForms()
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
            return _formProvider.GetAllDefaultForms(this.LoggedInUserTenantId);
        }




        /// <summary>
        /// Get all project default forms
        /// </summary>
        /// <remarks>
        /// Get all project default forms<br></br> 
        /// <strong>Purpose.</strong>
        /// - This api is used to get all default forms of a project.
        /// - This api takes project guid as input request.
        /// - This api returns list of FormViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="projectId">Project Id to get Project default Forms</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<FormViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllFormViewModelExamples))]
        [Route("api/v1/Form/GetProjectDefaultForms/{projectId}")]
        [HttpGet]
        public IEnumerable<FormViewModel> GetProjectDefaultForms(Guid projectId)
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
            return _formProvider.GetProjectDefaultForms(this.LoggedInUserTenantId, projectId);
        }


        /// <summary>
        /// Currently not in use
        /// </summary>
        /// <remarks>
        /// Currently not in use
        /// </remarks>
        /// <param name="formActivityViewModel">Activity Form View Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<FormViewModel>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [Route("api/v1/Form/GetFormsByGuidList/")]
        [HttpPost]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public IEnumerable<FormViewModel> GetFormsByGuidList([FromBody]List<Guid> formActivityViewModel)
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
            return _formProvider.GetFormsByGuidList(formActivityViewModel);
        }


        /// <summary>
        /// Get activity form by searched entity
        /// </summary>
        /// <remarks>
        /// Get activity form by searched entityy<br></br>
        /// <strong>Purpose.</strong>
        /// - This api returns a dataentry form.
        /// - This api takes entityId, formId, activityId, summarypage activityId as input parameters.
        /// - This api returns dataentry form, FormViewModel in response.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="entId">EntId</param>
        /// <param name="formId">Form Id</param>
        /// <param name="activityId">Activity Id</param>
        /// <param name="summaryPageActivityId">summary page activity id</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]        
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormViewModelExamples))]
        [Route("api/v1/Form/GetActivityFormBySearchedEntity/{entId}/{formId}/{activityId}/{summaryPageActivityId}")]
        //[Route("api/v1/Test/Form/GetActivityFormBySearchedEntity/{entId}/{formId}/{activityId}/{summaryPageActivityId}")]
        [HttpGet]
        public FormViewModel GetActivityFormBySearchedEntity(int entId, Guid formId, Guid activityId, int summaryPageActivityId)
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
            string absolutePath = string.Empty;
            try
            {
                absolutePath = Request.RequestUri.Segments[3].ToLower();
                WriteLog("API.FormController URL=" + string.Join(",", Request.RequestUri.Segments));
            }
            catch (Exception exc) { }

            if (absolutePath == "test/")
            {
                return _formProvider.TestEnvironment_GetActivityFormBySearchedEntity(entId, formId, activityId);
            }
            else
            {
                return _formProvider.GetActivityFormBySearchedEntity(entId, formId, activityId, summaryPageActivityId);
            }
        }
        /// <summary>
        /// Get all forms of a project
        /// </summary>
        /// <remarks>
        /// Get all forms of a project<br></br>
        /// <strong>Purpose.</strong>
        /// - This api is used to get all forms of a project.
        /// - This api takes guid of a project as input request.
        /// - This api returns list of FormViewModel in response model .
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
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<FormViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllFormViewModelExamples))]
        [HttpGet]
        [Route("api/v1/Form/GetAllForms/{projectId}")]
        public IEnumerable<FormViewModel> GetAllForms(Guid projectId)
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
            return _formProvider.GetAllForms(this.LoggedInUserTenantId, projectId);
        }

        /// <summary>
        /// Get forms by guid
        /// </summary>
        /// <remarks>
        /// Get forms by guid<br></br>  
        /// <strong>Purpose.</strong>
        /// - This api is used to get form by its guid.
        /// - This api returns FormViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of a forms that needs to be fetched</param>
        /// <param name="projectid">project id of a forms that needs to be fetched</param>
        /// 
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormViewModelExamples))]
        [HttpGet]
        [Route("api/v1/Form/GetformbyGuid/{guid}/{projectid}")]
        public FormViewModel GetformbyGuid(Guid guid, Guid projectid)
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
            var checkList = _formProvider.GetUSerFormByGuid(guid, this.LoggedInUserId, projectid);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Form was not found."),
                });
            }

            return checkList;
        }


    }
}
