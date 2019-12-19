using Aspree.Core.Enum;
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
    public class FormDataEntryController : BaseController
    {
        // GET: api/FormDataEntry
        private readonly IFormDataEntryProvider _formDataEntryProvider;
        private readonly IUserLoginProvider _userLoginProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="formDataEntryProvider"></param>
        public FormDataEntryController(IFormDataEntryProvider formDataEntryProvider, IUserLoginProvider userLoginProvider)
        {
            _formDataEntryProvider = formDataEntryProvider;
            _userLoginProvider = userLoginProvider;
        }

        /// <summary>
        /// Get all form data entry
        /// </summary>
        /// <remarks>
        /// Get all form data entryy<br></br>
        /// <strong>Purpose.</strong>
        /// -The purpose of this api is to get all form data entry.
        /// -The api fetches list of form data entry from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records found for the current request")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<FormDataEntryViewModel>))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<GetAllFormDataEntryViewModelExamples>))]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpGet]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public IEnumerable<FormDataEntryViewModel> GetAll()
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
            return _formDataEntryProvider.GetAll();
            //return _formDataEntryProvider.GetAll(this.LoggedInUserTenantId);
        }

        /// <summary>
        /// Get form for data entry by guid
        /// </summary>
        /// <remarks>
        /// Get form for data entry by guid<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to get a form for data entry based on its guid.
        /// - Form for data entry will be fetched from SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of a Form Data Entry that have to be fetched</param>
       
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormDataEntryViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormDataEntryViewModelExamples))]
        [HttpGet]
        public FormDataEntryViewModel Get(Guid guid)
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

            var checkList = _formDataEntryProvider.GetByGuid(guid);
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
        /// Add new form for data entry
        /// </summary>
        /// <remarks>
        /// Add new form for data entry<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to create new form for data entry.
        /// - The new form for data entry will be saved in SQL database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newFormDataEntry">New FormDataEntry Model</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormDataEntryViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormDataEntryViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPost]
        [Route("api/v1/FormDataEntry/Save/")]
        //[Route("api/v1/Test/FormDataEntry/Save/")]
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

            var addedVariable = new FormDataEntryViewModel();
            try
            {
                newFormDataEntry.Status = newFormDataEntry.Status == 0 ? (int)Core.Enum.FormStatusTypes.Draft : newFormDataEntry.Status;
                var absolutePath = "";
                absolutePath = Request.RequestUri.Segments[3].ToLower();
                if (absolutePath == "test/")
                {
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
                else
                {
                    WriteLog("API.FormDataEntryController URL=" + Request.RequestUri);
                    addedVariable = _formDataEntryProvider.Create(new FormDataEntryViewModel()
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
                        SubjectId = newFormDataEntry.SubjectId,
                    });
                }
            }
            catch (Exception exc) { WriteLog(exc.Message); }
            _formDataEntryProvider.SaveChanges();
            WriteLog("API.FormDataEntryController Post Complete");
            return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
        }

        /// <summary>
        /// Edit existing forms for data entry
        /// </summary>
        /// <remarks>
        /// Edit existing forms for data entry<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to edit/update an existing form for data entry by its guid.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editFormDataEntry">Edit FormDataEntry Model</param>
        /// <param name="guid">Guid of forms for Data Entry that needs to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record  already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormDataEntryViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormDataEntryViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPut]
        [Route("api/v1/FormDataEntry/Edit/{guid}")]
        public HttpResponseMessage Edit(Guid guid, [FromBody]FormDataEntryViewModel editFormDataEntry)
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
                var absolutePath = "";
                absolutePath = Request.RequestUri.Segments[3].ToLower();
                if (absolutePath == "test/")
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
                                var isSent = emailService.SendWelcomeEmail(emailid, username, "http://uds-test.cloud.monash.edu/account/resetpassword/" + tempGuid.ToString());
                            }
                        }
                    }
                }
                else
                {
                    updatedVariable = _formDataEntryProvider.Update(new FormDataEntryViewModel()
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

                        emailid = editFormDataEntry.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38)
                            != null
                            ? editFormDataEntry.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38).SelectedValues
                            : string.Empty;

                        username = editFormDataEntry.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40)
                            != null
                            ? editFormDataEntry.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40).SelectedValues
                            : string.Empty;

                        var authType = editFormDataEntry.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51)
                            != null
                            ? editFormDataEntry.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51).SelectedValues
                            : string.Empty;

                        bool isMailAlreadySent = _userLoginProvider.check_IsMailSend(false, username, updatedVariable.ThisUserId);

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
                                            #region addedd on 25.06.2019
                                            
                                            if (isMailAlreadySent)
                                                isSendMail = false;
                                            #endregion
                                        }
                                        else
                                        {
                                            isSendMail = false;
                                        }
                                    }
                                    var ApprovedBySystemAdmin = editFormDataEntry.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 41);
                                    var ActiveUser = editFormDataEntry.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 42);
                                    var SystemRole = editFormDataEntry.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 43);

                                    string ApprovedBySystemAdminSelectedVal = ApprovedBySystemAdmin != null ? ApprovedBySystemAdmin.SelectedValues : string.Empty;
                                    string ActiveUserSelectedVal = ActiveUser != null ? ActiveUser.SelectedValues : string.Empty;
                                    string SystemRoleSelectedVal = SystemRole != null ? SystemRole.SelectedValues : string.Empty;

                                    bool ApprovedBySystemAdminSelectedVal_Bool = ApprovedBySystemAdminSelectedVal == "1" ? true : false;
                                    bool ActiveUserSelectedVal_Bool = ActiveUserSelectedVal == "1" ? true : false;
                                    bool SystemRoleSelectedVal_Bool = !string.IsNullOrEmpty(SystemRoleSelectedVal) ? true : false;

                                    if (editFormDataEntry.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                                        || editFormDataEntry.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration))
                                    {
                                        if (ApprovedBySystemAdminSelectedVal_Bool && ActiveUserSelectedVal_Bool && SystemRoleSelectedVal_Bool)
                                        {
                                            //do stuff
                                            //system adnin fields are field
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
                            Guid tempGuid = _userLoginProvider.UpdateTempGuid(thisUserId);
                            if (tempGuid != Guid.Empty)
                            {
                                var emailService = new Services.EmailService();
                                var isSent = emailService.SendWelcomeEmail(emailid, username, Utilities.ConfigSettings.WebUrl + "account/resetpassword/" + tempGuid.ToString());

                                var updateStatus = _userLoginProvider.UpdateIsMailSend(thisUserId);
                            }
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
        /// Delete existing form data entry
        /// </summary>
        /// <remarks>
        /// Delete existing form data entry<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to delete an existing form data entry by its guid.
        /// - The api will delete data from SQL table(FormDataEntry).
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="guid">Guid of Forms that is to be deleted</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormDataEntryViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
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
            var deletedCategory = _formDataEntryProvider.DeleteByGuid(guid, LoggedInUserId);
            _formDataEntryProvider.SaveChanges();

            if (deletedCategory != null)
                return Request.CreateResponse(HttpStatusCode.OK, deletedCategory);
            else
                return Request.CreateResponse(HttpStatusCode.NotFound, "Form was not found.");
        }
        /// <summary>
        /// Search entities
        /// </summary>
        /// <remarks>
        /// Search entities<br></br>
        /// <strong>Purpose.</strong>
        /// This api provides the functionality to fetch records of participants entity.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="searchPageVariables">SearchPageVariableViewModel</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(SearchPageVariableViewModel))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [Route("api/v1/FormDataEntry/SearchParticipant/")]
        [HttpPost]
        public List<List<FormDataEntryVariableViewModel>> SearchParticipant([FromBody]SearchPageVariableViewModel searchPageVariables)
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
                var absolutePath = "";
                absolutePath = Request.RequestUri.Segments[3].ToLower();
                if (absolutePath == "test/")
                {
                    var searchResult = _formDataEntryProvider.TestEnvironment_SearchVariables(searchPageVariables);
                    return searchResult != null ? (searchResult.Count != 0 ? searchResult : null) : null;
                }
                else
                {
                    var searchResult = _formDataEntryProvider.SearchVariables(searchPageVariables);
                    return searchResult != null ? (searchResult.Count != 0 ? searchResult : null) : null;
                }
            }
            catch (Exception exc)
            { }
            return null;
        }


        /// <summary>
        /// Get form data entry by entId
        /// </summary>
        /// <remarks>
        /// Get forms data entry by entId<br></br>
        /// <strong>Purpose.</strong>
        /// - The purpose of this api is to get form data entry.
        /// - This api fetches records by projectId, formId, entityId
        /// - This api returns list of form data entry.
        /// </remarks>        
        /// <param name="projectId">project id of form variable to be search</param>
        /// <param name="formId">form id to search for variables</param>
        /// <param name="entId">entId of a forms that to be fetched</param>
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<FormDataEntryVariableViewModel>))]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [Route("api/v1/FormDataEntry/GetFormDataEntryByEntId/{projectId}/{formId}/{entId}")]
        [Route("api/v1/Test/FormDataEntry/GetFormDataEntryByEntId/{projectId}/{formId}/{entId}")]
        [HttpGet]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public IEnumerable<FormDataEntryVariableViewModel> GetFormDataEntryByEntId(Guid projectId, Guid formId, string entId)
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
            WriteLog("API.FormDataEntryController URL=" + Request.RequestUri);
            string absolutePath = string.Empty;
            try
            {
                absolutePath = Request.RequestUri.Segments[3].ToLower();
            }
            catch (Exception exc)
            { }

            IEnumerable<FormDataEntryVariableViewModel> checkList;
            if (absolutePath == "test/")
            {
                checkList = _formDataEntryProvider.TestEnvironment_GetFormDataEntryByEntId(projectId, formId, entId);
            }
            else
            {
                checkList = _formDataEntryProvider.GetFormDataEntryByEntId(projectId, formId, entId);
            }

            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Search record was not found."),
                });
            }

            return checkList;
        }

        /// <summary>
        /// Get all projects
        /// </summary>
        /// <remarks>
        /// Get all projects<br></br>
        /// <strong>Purpose.</strong>
        /// </remarks>        
        /// <param name="projectId">project id of form variable to be search</param>
        /// <param name="formId">form id to search for variables</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormDataEntryViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [Route("api/v1/FormDataEntry/GetAllFormDataEntryProjects/{projectId}/{formId}")]
        [HttpGet]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public IEnumerable<FormDataEntryProjectsViewModel> GetAllFormDataEntryProjects(Guid projectId, Guid formId)
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
            var checkList = _formDataEntryProvider.GetAllFormDataEntryProjects(projectId, formId);
            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Search record was not found."),
                });
            }

            return checkList;
        }

        /// <summary>
        /// Get all projects for List of projects page
        /// </summary>
        /// <remarks>
        /// Get all projects for List of projects page
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormDataEntryViewModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [Route("api/v1/FormDataEntry/GetAllDataEntryProjectList/")]
        [Route("api/v1/Test/FormDataEntry/GetAllDataEntryProjectList/")]
        [HttpGet]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public IEnumerable<FormDataEntryProjectsViewModel> GetAllDataEntryProjectList()
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
            IEnumerable<FormDataEntryProjectsViewModel> checkList;
            var absolutePath = string.Empty;
            absolutePath = Request.RequestUri.Segments[3].ToLower();
            if (absolutePath == "test/")
            {
                checkList = _formDataEntryProvider.TestEnvironment_GetAllDataEntryProjectList(this.LoggedInUserId);
            }
            else
            {
                checkList = _formDataEntryProvider.GetAllDataEntryProjectList();
            }

            if (checkList == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Search record was not found."),
                });
            }
            return checkList;
        }
    }
}