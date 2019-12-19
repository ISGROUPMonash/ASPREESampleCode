using Aspree.Core.Enum;
using Aspree.Core.ViewModels;
using Aspree.Core.ViewModels.MongoViewModels;
using Aspree.Provider.Interface;
using Aspree.Provider.Interface.MongoProvider;
using Aspree.WebApi.Utilities;
using Swashbuckle.Examples;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;

namespace Aspree.WebApi.Controllers
{
    /// <summary>
    /// Test site using this controller
    /// </summary>
    public class ReviewController : BaseController
    {
        private readonly IProjectDeployProvider _projectDeployProvider;
        private readonly ISearchProvider _searchProvider;
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly IFormDataEntryProvider _formDataEntryProvider;
        private readonly IActivityProvider _activityProvider;
        private readonly IFormProvider _formProvider;
        private readonly ISummaryProvider _summaryProvider;
        private readonly IForgotPasswordProvider _forgotPassword;

        /// <summary>
        /// Constructor of ReviewController class
        /// </summary>
        /// <param name="projectDeployProvider"></param>
        /// <param name="searchProvider"></param>
        /// <param name="userLoginProvider"></param>
        /// <param name="formDataEntryProvider"></param>
        /// <param name="activityProvider"></param>
        /// <param name="formProvider"></param>
        /// <param name="summaryProvider"></param>
        /// <param name="forgotPassword"></param>
        public ReviewController(
            IProjectDeployProvider projectDeployProvider
            , ISearchProvider searchProvider
            , IUserLoginProvider userLoginProvider
            , IFormDataEntryProvider formDataEntryProvider
            , IActivityProvider activityProvider
            , IFormProvider formProvider
            , ISummaryProvider summaryProvider
            , IForgotPasswordProvider forgotPassword
            )
        {
            this._projectDeployProvider = projectDeployProvider;
            this._searchProvider = searchProvider;
            this._userLoginProvider = userLoginProvider;
            this._formDataEntryProvider = formDataEntryProvider;
            this._activityProvider = activityProvider;
            this._formProvider = formProvider;
            this._summaryProvider = summaryProvider;
            this._forgotPassword = forgotPassword;
        }

        #region ProjectDeployController Test API's

        /// <summary>
        /// Get project from mongo db by project guid
        /// </summary>
        /// <remarks>
        /// Get project from mongo db by project guid<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get project by its guid .
        /// - Project will be fetched from Mongo Database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="projectId">Guid of a project that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectDeployViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ProjectDeployViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        [Route("api/v1/Review/GetProjectMongo/{projectId}")]
        public ProjectDeployViewModel GetProjectMongo(Guid? projectId)
        {
            WriteLog("api called");
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

            if (projectId == null)
            {
                GuidExceptionHandler();
            }

            WriteLog("api projectId" + projectId);
            Guid newprojectId = projectId.Value;

            var checkList = _projectDeployProvider.TestEnvironment_GetProjectByGuid(newprojectId);
            WriteLog("api checkList" + checkList);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Project was not found."),
                });
            }
            WriteLog("api checkList" + checkList.ProjectName + "ver" + checkList.ProjectInternalVersion);
            return checkList;
        }
        /// <summary>
        /// Get all deployed projects
        /// </summary>
        /// <remarks>
        /// Get all deployed projects<br></br>
        /// <strong>Purpose.</strong>
        /// - The purpose of this api is to get all deployed projects from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<FormDataEntryProjectsViewModel>))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllFormDataEntryProjectsViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [Route("api/v1/Review/GetAllDeployedProject/")]
        [HttpGet]
        public IEnumerable<FormDataEntryProjectsViewModel> GetAllDeployedProject()
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
            return _projectDeployProvider.TestEnvironment_GetAllDeployedProject(this.LoggedInUserId);
        }



        /// <summary>
        /// Check project linked by entityId
        /// </summary>
        /// <remarks>
        /// Check project linked by entityId<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to check whether the project is already linked to entity or not.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="projectId">Guid of a project</param>
        /// <param name="entityId">Entity id</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ProjectStaffMemberRoleViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ProjectStaffMemberRoleViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        [Route("api/v1/Review/CheckEntityLinkedProject/{projectId}/{entityId}")]
        public ProjectStaffMemberRoleViewModel CheckEntityLinkedProject(Guid? projectId, int entityId)
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

            if (projectId == null)
            {
                GuidExceptionHandler();
            }
            var newprojectId = projectId.Value;

            var checkList = _projectDeployProvider.TestEnvironment_CheckEntityLinkedProject(newprojectId, entityId);
            if (checkList == null) { return null; }
            return checkList;

        }


        #endregion


        #region Mongo -> SearchController Test API's

        /// <summary>
        /// Search Entities
        /// </summary>
        /// <remarks>
        /// Search Entities<br></br>
        /// <strong>Purpose.</strong>
        /// - The purpose of this api is to search an entity from SQL database.
        /// - This api takes SearchPageVariableViewModel as request model.
        /// - This api return list of FormDataEntryVariableViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="searchPageVariables">SearchPageVariableViewModel</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SearchPageVariableViewModel))]
        [SwaggerRequestExample(typeof(SearchPageVariableViewModel), typeof(SearchPageVariableViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormDataEntryVariableViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [Route("api/v1/Review/SearchEntities_M/")]
        [HttpPost]
        public List<List<FormDataEntryVariableViewModel>> SearchEntities([FromBody]SearchPageVariableViewModel searchPageVariables)
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
            try
            {
                var searchResult = _searchProvider.TestEnvironment_SearchEntities(searchPageVariables);
                return searchResult;
            }
            catch (Exception exc)
            { Console.WriteLine(exc); return null; }
            //return new List<List<FormDataEntryVariableViewModel>>();
        }

        /// <summary>
        /// Save new entities into Mongo database
        /// </summary>
        /// <remarks>
        /// Save new entities into Mongo database<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to create new entity into the system.
        /// - The new entity will be saved into Mongo database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newFormDataEntry">Forms Data Entry Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormDataEntryViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormDataEntryViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPost]
        [Route("api/v1/Review/SaveEntities/")]
        public HttpResponseMessage Post([FromBody]FormDataEntryViewModel newFormDataEntry)
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
            WriteLog("API.FormDataEntryController URL=" + Request.RequestUri);
            var addedVariable = new FormDataEntryViewModel();
            newFormDataEntry.Status = newFormDataEntry.Status == 0 ? (int)Core.Enum.FormStatusTypes.Draft : newFormDataEntry.Status;
            try
            {
                string[] defaultFormNames = new string[]
{
                        EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration)
                        , EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration)
                        , EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration)
                        , EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration)
                        , EnumHelpers.GetEnumDescription(DefaultFormName.Project_Linkage)
};

                List<FormDataEntryVariableViewModel> formVariablesEntryList = new List<FormDataEntryVariableViewModel>();
                WriteLog("API.FormDataEntryController URL=" + Request.RequestUri);
                if (!defaultFormNames.Contains(newFormDataEntry.FormTitle))
                {
                    if (newFormDataEntry.FormDataEntryVariable != null)
                    {
                        newFormDataEntry.FormDataEntryVariable.ForEach(variable =>
                        {
                            if (variable.VariableTypeName == Core.Enum.VariableTypes.FileType.ToString())
                            {
                                if (variable.SelectedValues.StartsWith("data:"))
                                    variable.SelectedValues = SaveFileTypeVariableFile(variable, newFormDataEntry.FormId);
                            }
                            formVariablesEntryList.Add(variable);                            
                        });
                    }
                }
                else
                {
                    formVariablesEntryList = newFormDataEntry.FormDataEntryVariable;
                }

                addedVariable = _searchProvider.TestEnvironment_Create(new FormDataEntryViewModel()
                {
                    CreatedBy = this.LoggedInUserId,
                    ActivityId = newFormDataEntry.ActivityId,
                    FormDataEntryVariable = newFormDataEntry.FormDataEntryVariable,
                    ProjectId = newFormDataEntry.ProjectId,
                    FormId = newFormDataEntry.FormId,
                    Status = newFormDataEntry.Status,
                    TenantId = this.LoggedInUserTenantId,
                    ParentEntityNumber = newFormDataEntry.ParentEntityNumber,
                    ParticipantId = newFormDataEntry.ParticipantId,
                    SummaryPageActivityObjId = newFormDataEntry.SummaryPageActivityObjId,
                });
            }
            catch (Exception exc) { WriteLog(exc.Message); }

            _searchProvider.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
        }
        /// <summary>
        /// Update entities into mongo db
        /// </summary>
        /// <remarks>
        /// Update entities into mongo db<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to edit/update an existing entity by its guid.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editForm">Edit Forms Model</param>
        /// <param name="guid">guid of Forms that needs to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormDataEntryViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormDataEntryViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPut]
        [Route("api/v1/Review/EditEntities/{guid}")]
        public HttpResponseMessage Put(string guid, [FromBody]FormDataEntryViewModel editForm)
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
            editForm.Status = editForm.Status == 0 ? (int)Core.Enum.FormStatusTypes.Draft : editForm.Status;
            Guid? islocalpassword = _searchProvider.TestEnvironment_GetCurrentAuthType(guid);
            var updatedVariable = new FormDataEntryViewModel();
            try
            {
                string[] defaultFormNames = new string[]
{
                        EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration)
                        , EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration)
                        , EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration)
                        , EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration)
                        , EnumHelpers.GetEnumDescription(DefaultFormName.Project_Linkage)
};

                List<FormDataEntryVariableViewModel> formVariablesEntryList = new List<FormDataEntryVariableViewModel>();
                if (!defaultFormNames.Contains(editForm.FormTitle))
                {
                    if (editForm.FormDataEntryVariable != null)
                    {
                        editForm.FormDataEntryVariable.ForEach(variable =>
                        {
                            if (variable.VariableTypeName == Core.Enum.VariableTypes.FileType.ToString())
                            {
                                if (variable.SelectedValues.StartsWith("data:"))
                                    variable.SelectedValues = SaveFileTypeVariableFile(variable, editForm.FormId);
                            }
                            formVariablesEntryList.Add(variable);
                        });
                    }
                }
                else
                {
                    formVariablesEntryList = editForm.FormDataEntryVariable;
                }

                #region test site
                updatedVariable = _searchProvider.TestEnvironment_UpdateSearchForm(guid, new FormDataEntryViewModel()
                {
                    ModifiedDate = DateTime.UtcNow,
                    CreatedBy = this.LoggedInUserId,
                    ActivityId = editForm.ActivityId,
                    FormDataEntryVariable = formVariablesEntryList,
                    ProjectId = editForm.ProjectId,
                    FormId = editForm.FormId,
                    Status = editForm.Status, 
                    TenantId = this.LoggedInUserTenantId,
                    ParentEntityNumber = editForm.ParentEntityNumber,
                    ParticipantId = editForm.ParticipantId,
                });
                if (updatedVariable == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Form was not found.");
                }
                _searchProvider.SaveChanges();
                if (editForm.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                    || editForm.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                    || editForm.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration))
                {
                    var emailid = string.Empty;
                    var username = string.Empty;
                    bool isSendMail = false;

                    emailid = editForm.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38)
                        != null
                        ? editForm.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38).SelectedValues
                        : string.Empty;

                    username = editForm.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40)
                        != null
                        ? editForm.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40).SelectedValues
                        : string.Empty;

                    var authType = editForm.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51)
                        != null
                        ? editForm.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51).SelectedValues
                        : string.Empty;

                    bool isMailAlreadySent = _userLoginProvider.check_IsMailSend(true, username, updatedVariable.ThisUserId);

                    if (!string.IsNullOrEmpty(authType))
                    {
                        var localpw = _formDataEntryProvider.LocalPasswordGuid();
                        if (localpw != null)
                        {
                            if (localpw.ToString() == authType)
                            {
                                isSendMail = true;
                                if (islocalpassword.ToString() == authType)
                                {
                                    if (!string.IsNullOrEmpty(username))
                                    {
                                        if (isMailAlreadySent)
                                            isSendMail = false;
                                    }
                                    else
                                    {
                                        isSendMail = false;
                                    }
                                }
                                var ApprovedBySystemAdmin = editForm.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 41);
                                var ActiveUser = editForm.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 42);
                                var SystemRole = editForm.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 43);

                                string ApprovedBySystemAdminSelectedVal = ApprovedBySystemAdmin != null ? ApprovedBySystemAdmin.SelectedValues : string.Empty;
                                string ActiveUserSelectedVal = ActiveUser != null ? ActiveUser.SelectedValues : string.Empty;
                                string SystemRoleSelectedVal = SystemRole != null ? SystemRole.SelectedValues : string.Empty;

                                bool ApprovedBySystemAdminSelectedVal_Bool = ApprovedBySystemAdminSelectedVal == "1" ? true : false;
                                bool ActiveUserSelectedVal_Bool = ActiveUserSelectedVal == "1" ? true : false;
                                bool SystemRoleSelectedVal_Bool = !string.IsNullOrEmpty(SystemRoleSelectedVal) ? true : false;

                                if (editForm.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                                        || editForm.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration))

                                {
                                    if (ApprovedBySystemAdminSelectedVal_Bool && ActiveUserSelectedVal_Bool && SystemRoleSelectedVal_Bool)
                                    {
                                        //do stuff
                                        //system amdin fileds are field
                                    }
                                    else
                                    {
                                        isSendMail = false;
                                    }
                                }
                            }
                        }
                    }

                    string fname = string.Empty;
                    string lname = string.Empty;
                    if (editForm.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration))
                    {
                        var l = editForm.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                        var f = editForm.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                        lname = l != null ? l.SelectedValues : string.Empty;
                        fname = f != null ? f.SelectedValues : string.Empty;
                    }
                    else if (editForm.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration))
                    {
                        var l = editForm.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                        var f = editForm.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                        var m = editForm.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 17);

                        lname = l != null ? l.SelectedValues : string.Empty;
                        fname = f != null ? f.SelectedValues : string.Empty + " " + m != null ? m.SelectedValues : string.Empty;
                    }
                    else if (editForm.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration))
                    {
                        var l = editForm.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                        lname = l != null ? l.SelectedValues : string.Empty;
                    }
                    username = fname + " " + lname;
                    if (emailid != string.Empty && username != string.Empty && isSendMail == true)
                    {
                        Int32 thisUserId = updatedVariable.ThisUserId != null ? (int)updatedVariable.ThisUserId : 0;
                        Guid tempGuid = _userLoginProvider.UpdateTempGuid(thisUserId);
                        if (tempGuid != Guid.Empty)
                        {
                            var emailService = new Services.EmailService();
                            var isSent = emailService.SendWelcomeEmail(emailid, username, Utilities.ConfigSettings.WebUrlTestSite + "account/resetpassword/" + tempGuid.ToString());
                            var updateStatus = _userLoginProvider.UpdateIsMailSend(thisUserId);
                        }
                    }
                }
                #endregion
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, exc.Message);
            }
            return Request.CreateResponse(HttpStatusCode.OK, updatedVariable);
        }


        /// <summary>
        /// Delete activities on summary page
        /// </summary>
        /// <remarks>
        /// Delete activities on summary page<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to delete activity added on summary page.
        /// - This api will delete data from SQL table.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="id">id of summary page activity</param>
        //[SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]        
        //[SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormDataEntryMongo))]
        //[SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormDataEntryMongoExamples))]
        [HttpDelete]
        [Route("api/v1/Review/DeleteSummaryPageFormData/{id}")]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage DeleteSummaryPageFormData(string id)
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
            var addedVariable = _searchProvider.TestEnvironment_Delete(id, this.LoggedInUserId);
            return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
        }



        private string SaveFileTypeVariableFile(FormDataEntryVariableViewModel formDataEntryVariableViewModel, Guid formId)
        {
            try
            {
                string fileName = LoggedInUserId + "-" + DateTime.UtcNow.Ticks.ToString() + ".jpg";
                if (formDataEntryVariableViewModel.SelectedValues.Split(',')[0].Contains("application/pdf"))
                {
                    fileName = LoggedInUserId + "-" + DateTime.UtcNow.Ticks.ToString() + ".pdf";
                }
                else if (formDataEntryVariableViewModel.SelectedValues.Split(',')[0].Contains("image/jpeg"))
                {
                    fileName = LoggedInUserId + "-" + DateTime.UtcNow.Ticks.ToString() + ".jpg";
                }
                else
                {
                    string ext = formDataEntryVariableViewModel.SelectedValues.Split(',')[0];
                    ext = ext.Replace("data:", "");
                    fileName = LoggedInUserId + "-" + DateTime.UtcNow.Ticks.ToString() + "." + ext;
                }


                string fileDirectoryFullPath = System.Web.Hosting.HostingEnvironment.MapPath(ConfigSettings.UploadsDataEntryVariablePath);
                string fileDirectoryDBPath = "~/" + ConfigSettings.UploadsDataEntryVariablePath + fileName;

                // If directory does not exist, create it. 
                if (!Directory.Exists(fileDirectoryFullPath))
                {
                    Directory.CreateDirectory(fileDirectoryFullPath);
                }
                if (string.IsNullOrEmpty(formDataEntryVariableViewModel.SelectedValues))
                {
                    return string.Empty;
                }
                using (var fileStream = System.IO.File.OpenWrite(System.Web.Hosting.HostingEnvironment.MapPath(fileDirectoryDBPath)))
                {
                    byte[] file = Convert.FromBase64String(formDataEntryVariableViewModel.SelectedValues.Split(',')[1]);
                    fileStream.Write(file, 0, file.Length);
                }

                return fileDirectoryDBPath;
            }
            catch (Exception exp)
            {
                return string.Empty;
            }
        }

        #endregion


        #region ActivityController Test API's

        /// <summary>
        /// Get all summary page activities of an entity
        /// </summary>
        /// <remarks>
        /// Get all summary page activities of an entity<br></br>
        /// <strong>Purpose.</strong>
        /// - The purpose of this api is to get all activities of summary page, based on entityid and projectid.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="entId">entity id</param>
        /// <param name="projectId">project id</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<AddSummaryPageActivityViewModel>))]
        [SwaggerRequestExample(typeof(AddSummaryPageActivityViewModel), typeof(AddSummaryPageActivityViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(GetAllAddSummaryPageActivityViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        [Route("api/v1/Review/GetAllSummaryPageActivity/{entId}/{projectId}")]
        public IEnumerable<AddSummaryPageActivityViewModel> GetAllSummaryPageActivity(string entId, Guid? projectId)
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
            if (projectId == null)
            {
                GuidExceptionHandler();
            }
            var newprojectId = projectId.Value;

            return _activityProvider.TestEnvironment_GetAllSummaryPageActivity(entId, newprojectId);
        }

        /// <summary>
        /// Add Activities on Summary page
        /// </summary>
        /// <remarks>
        /// Add Activities on Summary page
        /// <para />1. This api is used for add an activity on summary page.
        /// <para />2. This api takes NewAddSummaryPageActivityViewModel model as request model.
        /// <para />3. This api return AddSummaryPageActivityViewModel model as response.
        /// </remarks>
        /// <param name="newActivity">AddActivities Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(NewAddSummaryPageActivityViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPost]
        [Route("api/v1/Review/AddSummaryPageActivity_SQL")]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage AddSummaryPageActivity_SQL([FromBody]NewAddSummaryPageActivityViewModel newActivity)
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
            var addedVariable = _activityProvider.TestEnvironment_AddSummaryPageActivity(new AddSummaryPageActivityViewModel()
            {
                CreatedBy = this.LoggedInUserId,
                CreatedDate = DateTime.UtcNow,
                ActivityId = newActivity.ActivityId,
                ActivityCompletedByUser = newActivity.ActivityCompletedByUser,
                ActivityDate = newActivity.ActivityDate,
                IsActivityAdded = newActivity.IsActivityAdded,
                ProjectId = newActivity.ProjectId,
                PersonEntityId = newActivity.PersonEntityId,
            });
            return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
        }

        /// <summary>
        /// Edit Activity of Summary page
        /// </summary>
        /// <remarks>
        /// Edit Activity of Summary page
        /// <para />1. This api is used to edit activity that is added on summary page.
        /// <para />2. This api is used to edit activity date, and completed by, of summary page activity
        /// <para />3. This api takes NewAddSummaryPageActivityViewModel as input request.
        /// <para />4. This api return updated response model.
        /// </remarks>
        /// <param name="editSummarypageActivity">edit summarypageActivity Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(NewAddSummaryPageActivityViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPost]
        [Route("api/v1/Review/EditSummaryPageActivity_SQL")]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage EditSummaryPageActivity_SQL([FromBody]NewAddSummaryPageActivityViewModel editSummarypageActivity)
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
            var addedVariable = _activityProvider.TestEnvironment_AddSummaryPageActivity(new AddSummaryPageActivityViewModel()
            {
                Id = editSummarypageActivity.Id,
                CreatedBy = this.LoggedInUserId,
                CreatedDate = DateTime.UtcNow,
                ActivityId = editSummarypageActivity.ActivityId,
                ActivityCompletedByUser = editSummarypageActivity.ActivityCompletedByUser,
                ActivityDate = editSummarypageActivity.ActivityDate,
                IsActivityAdded = editSummarypageActivity.IsActivityAdded,
                ProjectId = editSummarypageActivity.ProjectId,
                PersonEntityId = editSummarypageActivity.PersonEntityId,
            });
            return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
        }

        /// <summary>
        /// Delete activities on Summary page
        /// </summary>
        /// <remarks>
        /// Delete activities on Summary page
        /// <para />1. This api is used to delete an summary page added activity.
        /// <para />2. This api takes id of summary page added activity as input request.
        /// <para />3. This api return model of deleted summary page activity.
        /// </remarks>
        /// <param name="id">id of summary page activity</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(NewAddSummaryPageActivityViewModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpDelete]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/v1/Review/DeleteSummaryPageActivity_SQL/{id}")]
        public HttpResponseMessage DeleteSummaryPageActivity_SQL(int id)
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
            var addedVariable = _activityProvider.TestEnvironment_DeleteSummaryPageActivity(id, this.LoggedInUserId);
            return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
        }

        #endregion


        #region FormController Test API's

        /// <summary>
        /// Get activity form by searched entity
        /// </summary>
        /// <remarks>
        /// Get activity form by searched entity<br></br>
        /// <strong>Purpose.</strong>
        /// - The purpose of this api is to return a DataEntry form.
        /// - This api takes entityid, formid, activityid, summarypage activity id as input parameters.
        /// - This api returns DataEntry form, FormViewModel in response.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="entId">Entity Id</param>
        /// <param name="formId">Form Id</param>
        /// <param name="activityId">Activity Id</param>
        /// <param name="summaryPageActivityId">Summary page activity Id</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [Route("api/v1/Review/GetActivityFormBySearchedEntity/{entId}/{formId}/{activityId}/{summaryPageActivityId}")]
        [HttpGet]
        public FormViewModel GetActivityFormBySearchedEntity(int entId, Guid? formId, Guid? activityId, int summaryPageActivityId)
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

            if (formId == null || activityId == null)
            {
                GuidExceptionHandler();
            }
            Guid newformId = formId.Value;
            Guid newactivityId = activityId.Value;

            return _formProvider.TestEnvironment_GetActivityFormBySearchedEntity(entId, newformId, newactivityId);
        }
        #endregion

        #region FormDataEntryController Test API's

        /// <summary>
        /// Add new forms for DataEntry
        /// </summary>
        /// <remarks>
        /// Add new forms for DataEntry<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to create new form for Data Entry and save into SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newFormDataEntry">New Forms Data Entry Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormDataEntryViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormDataEntryViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPost]
        [Route("api/v1/Review/FormDataEntrySave/")]
        public HttpResponseMessage FormDataEntrySave([FromBody]FormDataEntryViewModel newFormDataEntry)
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

            var addedVariable = new FormDataEntryViewModel();
            try
            {
                newFormDataEntry.Status = newFormDataEntry.Status == 0 ? (int)Core.Enum.FormStatusTypes.Draft : newFormDataEntry.Status;
                addedVariable = _formDataEntryProvider.TestEnvironment_Create(new FormDataEntryViewModel()
                {
                    CreatedBy = this.LoggedInUserId,
                    ActivityId = newFormDataEntry.ActivityId,
                    FormDataEntryVariable = newFormDataEntry.FormDataEntryVariable,
                    ProjectId = newFormDataEntry.ProjectId,
                    FormId = newFormDataEntry.FormId,
                    Status = newFormDataEntry.Status,
                    TenantId = this.LoggedInUserTenantId,
                    ParentEntityNumber = newFormDataEntry.ParentEntityNumber,
                    ParticipantId = newFormDataEntry.ParticipantId,
                });
            }
            catch (Exception exc) { WriteLog(exc.Message); }
            _formDataEntryProvider.SaveChanges();
            return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
        }

        /// <summary>
        /// Edit existing forms for DataEntry
        /// </summary>
        /// <remarks>
        /// Edit existing forms for DataEntry<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to edit/update an existing form for DataEntry by its guid.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editFormDataEntry">Edit forms model of DataEntry</param>
        /// <param name="guid">Guid of forms of DataEntry that needs to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormDataEntryViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormDataEntryViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPut]
        [Route("api/v1/Review/FormDataEntryEdit/{guid}")]
        public HttpResponseMessage FormDataEntryEdit(Guid guid, [FromBody]FormDataEntryViewModel editFormDataEntry)
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

            editFormDataEntry.Status = editFormDataEntry.Status == 0 ? (int)Core.Enum.FormStatusTypes.Draft : editFormDataEntry.Status;

            var islocalpassword = _formDataEntryProvider.AuthTypeLocalMailsend(guid);
            var updatedVariable = new FormDataEntryViewModel();
            try
            {
                updatedVariable = _formDataEntryProvider.TestEnvironment_Update(new FormDataEntryViewModel()
                {
                    ModifiedDate = DateTime.UtcNow,
                    CreatedBy = this.LoggedInUserId,
                    ActivityId = editFormDataEntry.ActivityId,
                    FormDataEntryVariable = editFormDataEntry.FormDataEntryVariable,
                    ProjectId = editFormDataEntry.ProjectId,
                    FormId = editFormDataEntry.FormId,
                    Status = editFormDataEntry.Status,
                    Guid = guid,
                    TenantId = this.LoggedInUserTenantId,
                    ParentEntityNumber = editFormDataEntry.ParentEntityNumber,
                    ParticipantId = editFormDataEntry.ParticipantId,
                });
                if (updatedVariable == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Form was not found.");
                }

                _formDataEntryProvider.SaveChanges();

                if (editFormDataEntry.FormTitle == "Place/Group Registration" || editFormDataEntry.FormTitle == "Person Registration" || editFormDataEntry.FormTitle == "Participant Registration")
                {
                    var emailid = string.Empty;
                    var username = string.Empty;
                    bool isSendMail = false;
                    foreach (var item in editFormDataEntry.FormDataEntryVariable)
                    {
                        //variable name = "email"
                        if (item.VariableId == 38) { emailid = item.SelectedValues; }
                        //variable name = "Username"
                        if (item.VariableId == 40) { username = item.SelectedValues; }
                        //variable name = "AuthenticationMethod"
                        if (item.VariableId == 51)
                        {
                            var localpw = _formDataEntryProvider.LocalPasswordGuid();
                            if (localpw != null)
                            {
                                if (localpw.ToString() == item.SelectedValues)
                                {
                                    isSendMail = true;

                                    if (islocalpassword.ToString() == item.SelectedValues)
                                    {
                                        isSendMail = false;
                                    }
                                }

                            }
                        }
                    }
                    string fname = string.Empty;
                    string lname = string.Empty;
                    if (editFormDataEntry.FormTitle == "Person Registration")
                    {
                        var l = editFormDataEntry.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                        var f = editFormDataEntry.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                        lname = l != null ? l.SelectedValues : string.Empty;
                        fname = f != null ? f.SelectedValues : string.Empty;
                    }
                    else if (editFormDataEntry.FormTitle == "Participant Registration")
                    {
                        var l = editFormDataEntry.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                        var f = editFormDataEntry.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                        var m = editFormDataEntry.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 17);
                        lname = l != null ? l.SelectedValues : string.Empty;
                        fname = f != null ? f.SelectedValues : string.Empty + " " + m != null ? m.SelectedValues : string.Empty;
                    }
                    else if (editFormDataEntry.FormTitle == "Place/Group Registration")
                    {
                        var l = editFormDataEntry.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                        lname = l != null ? l.SelectedValues : string.Empty;
                    }
                    username = fname + " " + lname;
                    if (emailid != string.Empty && username != string.Empty && isSendMail == true)
                    {
                        Int32 thisUserId = updatedVariable.ThisUserId != null ? (Int32)updatedVariable.ThisUserId : 0;
                        var user = _userLoginProvider.GetById(thisUserId);
                        Guid tempGuid = user != null ? (Guid)user.TempGuid : Guid.Empty;
                        if (tempGuid != Guid.Empty)
                        {
                            var emailService = new Services.EmailService();
                            var isSent = emailService.SendWelcomeEmail(emailid, username, Utilities.ConfigSettings.WebUrlTestSite +"account/resetpassword/" + tempGuid.ToString());
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, exc.Message);
            }

            return Request.CreateResponse(HttpStatusCode.OK, updatedVariable);
        }


        /// <summary>
        /// Search entities
        /// </summary>
        /// <remarks>
        /// Search entities<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to search an entity from database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="searchPageVariables">SearchPageVariableViewModel</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SearchPageVariableViewModel))]
        [SwaggerRequestExample(typeof(SearchPageVariableViewModel), typeof(SearchPageVariableViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormDataEntryVariableViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [Route("api/v1/Review/SearchTestEntities/")]
        [HttpPost]
        public List<List<FormDataEntryVariableViewModel>> SearchTestEntities([FromBody]SearchPageVariableViewModel searchPageVariables)
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
            try
            {
                var searchResult = _formDataEntryProvider.TestEnvironment_SearchVariables(searchPageVariables);
                return searchResult != null ? (searchResult.Count != 0 ? searchResult : null) : null;
            }
            catch (Exception exc) { Console.WriteLine(exc); }
            return null;
        }

        #endregion


        #region MongoDB_Summary Test API's

        /// <summary>
        /// Get summary details by entityid and projectid
        /// </summary>
        /// <remarks>
        /// Get summary details by entityid and projectid<br></br>
        /// <strong>Purpose.</strong>
        /// - The purpose of this api is to get details of summary page.
        /// - Summary page details will be fetched based on entity id and project id.<br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>        
        /// <param name="projectId">guid of project</param>
        ///<param name="entityId">entity number</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "This error is returned by the server when we pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided detail are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SummaryViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(SummaryViewModelExamples))]
        [Route("api/v1/Review/GetSummaryDetails/{projectId}/{entityId}")]
        [HttpGet]
        public SummaryViewModel GetSummaryDetails(Guid? projectId, Int64 entityId)
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

            if (projectId == null)
            {
                GuidExceptionHandler();
            }
            Guid newprojectId = projectId.Value;

            var checkList = _summaryProvider.TestEnvironment_GetSummaryDetails(newprojectId, entityId, this.LoggedInUserId);
            if (checkList == null)
            {
                throw new Core.NotFoundException("Data not found.");
            }
            return checkList;
        }



        /// <summary>
        /// Open DataEntry form on summary page
        /// </summary>
        /// <remarks>
        /// Open DataEntry form on summary page<br></br>
        /// <strong>Purpose.</strong>
        /// - The purpose of this api is to get details of data entry form, based on entity id, guid of form, guid of activity, guid of project etc.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="entId">Entity id</param>
        /// <param name="formId">Guid of form</param>
        /// <param name="activityId">Guid of activity</param>
        /// <param name="projectId">Guid of project</param>
        /// <param name="p_Version">version</param>
        /// <param name="currentProjectId">Guid of loggedin project</param>
        /// <param name="summaryPageActivityId">activity id added on summary page</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided detail are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormsMongo))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormsMongoExamples))]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [Route("api/v1/Review/GetSummaryPageForm/{entId}/{formId}/{activityId}/{projectId}/{p_Version}/{summaryPageActivityId}/{currentProjectId}")]
        [HttpGet]
        public FormsMongo GetSummaryPageForm(int entId, Guid formId, Guid activityId, Guid projectId, int p_Version, string summaryPageActivityId, Guid currentProjectId)
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

            return _summaryProvider.TestEnvironment_GetSummaryPageForm(entId, formId, activityId, projectId, p_Version, summaryPageActivityId, this.LoggedInUserId, currentProjectId);
        }

        /// <summary>
        /// Add activities on summary page
        /// </summary>
        /// <remarks>
        /// Add activities on summary page<br></br>
        /// <strong>Purpose.</strong>
        /// - The purpose of this api is to add an activity on summary page.
        /// - Activity added on summary page contains form for data entry.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newActivity">SummaryPageActivityViewModel</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "This error is returned by the server when we pass wrong datatype value in request parameter")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SummaryPageActivityViewModel))]
        [SwaggerRequestExample(typeof(SummaryPageActivityViewModel), typeof(SummaryPageActivityViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPost]
        [Route("api/v1/Review/AddSummaryPageActivityMongo")]
        public HttpResponseMessage AddSummaryPageActivityMongo([FromBody]SummaryPageActivityViewModel newActivity)
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

            var addedVariable = _summaryProvider.TestEnvironment_AddSummaryPageActivity(new Core.ViewModels.MongoViewModels.SummaryPageActivityViewModel()
            {
                ActivityCompletedByGuid = newActivity.ActivityCompletedByGuid,
                PersonEntityId = Convert.ToInt64(newActivity.PersonEntityId),
                ProjectGuid = newActivity.ProjectGuid,
                ActivityGuid = newActivity.ActivityGuid,
                CreatedDate = DateTime.UtcNow,
                ActivityDate = newActivity.ActivityDate,
            }, this.LoggedInUserId);
            return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
        }
        /// <summary>
        /// Edit activity on summary page
        /// </summary>
        /// <remarks>
        /// Edit activity on summary page<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to edit activity date, completed by etc. on summary page.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="id">Id of summary page activity</param>
        /// <param name="editActivity">SummaryPageActivityViewModel</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "This error is returned by the server when we pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided id are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SummaryPageActivityViewModel))]
        [SwaggerRequestExample(typeof(SummaryPageActivityViewModel), typeof(SummaryPageActivityViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(SummaryPageActivityViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPut]
        [Route("api/v1/Review/EditSummaryPageActivity/{id}")]
        public HttpResponseMessage EditSummaryPageActivity(string id, [FromBody]SummaryPageActivityViewModel editActivity)
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
            var addedVariable = _summaryProvider.TestEnvironment_EditSummaryPageActivity(new SummaryPageActivityViewModel()
            {
                Id = new MongoDB.Bson.ObjectId(id),
                ActivityCompletedByGuid = editActivity.ActivityCompletedByGuid,
                PersonEntityId = Convert.ToInt64(editActivity.PersonEntityId),
                ProjectGuid = editActivity.ProjectGuid,
                ActivityGuid = editActivity.ActivityGuid,
                CreatedDate = DateTime.UtcNow,
                ActivityDate = editActivity.ActivityDate,
            }, this.LoggedInUserId);
            return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
        }

        /// <summary>
        /// Delete activities on summary page
        /// </summary>
        /// <remarks>
        /// Delete activities on summary page<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to delete an activity based on id on summary page.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="id">Id of summary page activity</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "This error is returned by the server when we pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided id are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Core.ViewModels.MongoViewModels.SummaryPageActivityViewModel))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(Core.ViewModels.MongoViewModels.SummaryPageActivityViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpDelete]
        [Route("api/v1/Review/DeleteSummaryPageActivity/{id}")]
        public HttpResponseMessage DeleteSummaryPageActivity(string id)
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

            var addedVariable = _summaryProvider.TestEnvironment_DeleteSummaryPageActivity(id, this.LoggedInUserId);
            return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
        }
        #endregion


        #region Project Test API's


        #endregion



        #region Reset Password
        /// <summary>
        /// Reset user password
        /// </summary>
        /// <remarks>
        /// Reset user password<br></br>   
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to create/reset password.
        /// - User can create/reset password from password creation link sent via email.
        /// - Reset password will be saved in SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="resetPassword">ResetPassword Model</param>
        /// <param name="guid">guid of the user whose password to be change</param>
        // PUT: api/ActivityCategory/5
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerRequestExample(typeof(ResetPassword), typeof(ResetPasswordExamples))]
        [HttpPut]
        [Route("api/v1/Review/ResetForgetPassword/{guid}")]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        [AllowAnonymous]
        public HttpResponseMessage ResetForgetPassword(Guid guid, [FromBody]ResetPassword resetPassword)
        {
            resetPassword.Guid = guid;

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var result = _userLoginProvider.ResetNewPassword(resetPassword, true);

            if (!result)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Password was not reset successfully.");
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }


        /// <summary>
        /// Get user by temp guid
        /// </summary>
        /// <remarks>
        /// Get user by temp guid<br></br> 
        /// <strong>Purpose.</strong>
        /// - This api is used to get user by tempguid.
        /// - This api returns UserLoginViewModel in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Temp Guid of an user that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]   
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UserLoginViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(UserLoginViewModelExamples))]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/v1/Review/GetUserByTempGuid/{guid}")]
        [AllowAnonymous]
        [HttpGet]
        public UserLoginViewModel GetUserByTempGuid(Guid? guid)
        {
            if (guid == null)
            {
                GuidExceptionHandler();
            }
            var newguid = guid.Value;
            var user = _userLoginProvider.GetByTempGuid(newguid, true);
            if (user == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("User was not found."),
                    ReasonPhrase = "User was not found."
                });
            }
            return user;
        }

        /// <summary>
        /// Forgot password
        /// </summary>
        /// <remarks>
        /// Forgot password<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to reset forgotten password.
        /// - This api sends a password reset link on e-mail id of the user.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="resetPass">ResetPassword Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerRequestExample(typeof(ResetPasswordViewModel), typeof(ResetPasswordViewModelExamples))]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        [Route("api/v1/Review/ForgotPasswordPost")]
        [AllowAnonymous]
        public HttpResponseMessage ForgotPasswordPost([FromBody]ResetPasswordViewModel resetPass)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.AllErrors());
            }
            var user = _forgotPassword.checkUser(resetPass, true);
            string added = string.Empty;
            if (user != null)
            {
                //if user is not approved by system admin then prevent to send password reset mail.
                if (!user.IsUserApprovedBySystemAdmin)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, false);
                }
                //if user is not approved by system admin then prevent to send password reset mail.
                if (user.Status != (int)Core.Enum.Status.Active)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, false);
                }

                var emailService = new Services.EmailService();
                //bool isSent = emailService.SendForgotPasswordEmail(user.Email, user.FirstName + " " + user.LastName, Utilities.ConfigSettings.WebUrl + "/account/forgotpassword/" + user.Guid.ToString());
                bool isSent = emailService.SendForgotPasswordEmail(user.Email, user.FirstName + " " + user.LastName, Utilities.ConfigSettings.WebUrlTestSite + "/account/forgotpassword/" + user.TempGuid.ToString());
            }
            else
            {
                //added = "Please try again. User is not found!";
                //return Request.CreateResponse(HttpStatusCode.OK, false);
            }
            return Request.CreateResponse(HttpStatusCode.OK, true);
        }


        /// <summary>
        /// Reset user password
        /// </summary>
        /// <remarks>
        /// Reset user password<br></br>   
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to create/reset password.
        /// - User can create/reset password from password creation link sent via email.
        /// - Reset password will be saved in SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="resetPassword">ResetPassword Model</param>
        /// <param name="guid">guid of the user whose password to be change</param>
        // PUT: api/ActivityCategory/5
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerRequestExample(typeof(ResetPassword), typeof(ResetPasswordExamples))]
        [HttpPut]
        [Route("api/v1/Review/ResetForgetPasswordPost/{guid}")]
        [AllowAnonymous]
        public HttpResponseMessage ResetForgetPasswordPost(Guid guid, [FromBody]ResetPassword resetPassword)
        {
            resetPassword.Guid = guid;

            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            var result = _userLoginProvider.ResetPassword(resetPassword,true);

            if (!result)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Password was not reset successfully.");
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);

        }
        #endregion

        #region My Profile
        /// <summary>
        /// Edit existing user
        /// </summary>
        /// <remarks>
        /// Edit existing user<br></br> 
        /// <strong>Purpose.</strong>        
        /// - This api is used to update an existing user.
        /// - This api takes EditUserViewModel and guid of user as input request.
        /// - This api returns UserLoginViewModel of updated user in response model.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editUser">Edit EditUserViewModel Model</param>
        /// <param name="guid">guid of user that has to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The user pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "records already exist with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(UserLoginViewModel))]
        [SwaggerRequestExample(typeof(EditUserViewModel), typeof(EditUserViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(UserLoginViewModelExamples))]
        [Route("api/v1/Review/UpdateMyProfile/{guid}")]
        [HttpPut]
        public HttpResponseMessage UpdateMyProfile(Guid guid, [FromBody]EditUserViewModel editUser)
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
            var updated = _userLoginProvider.UpdateMyProfile(new UserLoginViewModel()
            {
                Guid = guid,
                Address = editUser.Address,
                AuthTypeId = (int)Core.Enum.AuthenticationTypes.Local_Password,
                ModifiedBy = this.LoggedInUserId,
                ModifiedDate = DateTime.UtcNow,
                Email = editUser.Email,
                FirstName = editUser.FirstName,
                LastName = editUser.LastName,
                Mobile = editUser.Mobile,
                RoleId = editUser.RoleId,
                TenantId = editUser.TenantId,
                UserName = editUser.UserName,
                Status = editUser.Status,
                IsUserApprovedBySystemAdmin = editUser.IsUserApprovedBySystemAdmin,
            }, true);
            _userLoginProvider.SaveChanges();
            SaveProfileImage(updated.Guid, editUser.Profile);
            if (updated != null)
                return Request.CreateResponse(HttpStatusCode.OK, updated);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "User was not found.");
        }
        private void SaveProfileImage(Guid userGuid, string imageData)
        {
            try
            {
                var imgPath = System.IO.Path.Combine(ConfigSettings.ProfileImageBasePath + "/", userGuid.ToString() + ".jpg");
                if (string.IsNullOrEmpty(imageData))
                {
                    return;
                }
                using (var fileStream = System.IO.File.OpenWrite(imgPath))
                {
                    byte[] file = Convert.FromBase64String(imageData.Split(',')[1]);
                    fileStream.Write(file, 0, file.Length);
                }
            }
            catch (Exception exp)
            {
                throw;
            }
        }
        #endregion
    }
}