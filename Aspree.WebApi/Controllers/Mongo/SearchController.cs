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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Aspree.WebApi.Controllers.Mongo
{
    /// <summary>
    /// 
    /// </summary>
    public class SearchController : BaseController
    {
        private readonly ISearchProvider _searchProvider;
        private readonly IFormDataEntryProvider _formDataEntryProvider;
        private readonly IUserLoginProvider _userLoginProvider;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchProvider"></param>
        /// <param name="formDataEntryProvider"></param>
        /// <param name="userLoginProvider"></param>
        public SearchController(ISearchProvider searchProvider, IFormDataEntryProvider formDataEntryProvider, IUserLoginProvider userLoginProvider)
        {
            this._searchProvider = searchProvider;
            this._formDataEntryProvider = formDataEntryProvider;
            this._userLoginProvider = userLoginProvider;
        }

        /// <summary>
        /// Search entities
        /// </summary>
        /// <remarks>
        /// Search entities<br></br>
        /// <strong>Purpose.</strong>
        /// - The purpose of this api is to search an entity which will be fetched from SQL database.
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
        [Route("api/v1/Search/SearchEntities/")]
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
                NewCategory.WriteLog("search method called with url: " + Request.RequestUri.AbsoluteUri);
                var absolutePath = "";
                absolutePath = Request.RequestUri.Segments[3].ToLower();
                NewCategory.WriteLog("search method called" + string.Join(", ", Request.RequestUri.Segments));
                NewCategory.WriteLog("search method called" + absolutePath);
                if (absolutePath == "test/")
                {
                    NewCategory.WriteLog("call TestEnvironment_SearchVariables ");
                    var searchResult = _searchProvider.TestEnvironment_SearchEntities(searchPageVariables);
                    NewCategory.WriteLog("responce TestEnvironment_SearchVariables " + searchResult.Count());
                    return searchResult;
                }
                else
                {
                    NewCategory.WriteLog("call SearchVariables");
                    var searchResult = _searchProvider.SearchEntities(searchPageVariables);
                    NewCategory.WriteLog("responce SearchVariables " + searchResult);
                    return searchResult;
                }
            }
            catch (Exception exc)
            { return null; }
            return new List<List<FormDataEntryVariableViewModel>>();
        }

        /// <summary>
        /// Save new entities into Mongo database
        /// </summary>
        /// <remarks>
        /// Save new entities into Mongo database<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to create new entity into the system which will be saved in Mongo database.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="newFormDataEntry">FormDataEntryViewModel</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormDataEntryViewModel))]
        [SwaggerRequestExample(typeof(FormDataEntryViewModel), typeof(FormDataEntryViewModelExamples))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormDataEntryViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPost]
        [Route("api/v1/Search/SaveEntities/")]
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

                var absolutePath = "";
                absolutePath = Request.RequestUri.Segments[3].ToLower();
                if (absolutePath == "test/")
                {
                    WriteLog("API.FormDataEntryController URL=" + Request.RequestUri);
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
                else
                {
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

                    addedVariable = _searchProvider.Create(new FormDataEntryViewModel()
                    {
                        CreatedBy = this.LoggedInUserId,
                        ActivityId = newFormDataEntry.ActivityId,
                        FormDataEntryVariable = formVariablesEntryList,
                        ProjectId = newFormDataEntry.ProjectId,
                        FormId = newFormDataEntry.FormId,
                        Status = newFormDataEntry.Status,
                        TenantId = this.LoggedInUserTenantId,
                        ParentEntityNumber = newFormDataEntry.ParentEntityNumber,
                        ParticipantId = newFormDataEntry.ParticipantId,
                        SummaryPageActivityObjId = newFormDataEntry.SummaryPageActivityObjId,
                    });
                }
            }
            catch (Exception exc) { WriteLog(exc.Message); }



            _searchProvider.SaveChanges();
            WriteLog("API.FormDataEntryController Post Complete");
            return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
        }

        /// <summary>
        /// Update entities into Mongo database
        /// </summary>
        /// <remarks>
        /// Update entities into Mongo database<br></br>
        /// <strong>Purpose.</strong>
        /// - This api provides the functionality to edit/update an existing entity by its guid.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="editForm">FormDataEntryViewModel</param>
        /// <param name="guid">Guid of forms that needs to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "The datatype of input parameter value is invalid in request api")]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Record already exists with provided details")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided guid are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FormDataEntryViewModel))]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(FormDataEntryViewModelExamples))]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [HttpPut]
        [Route("api/v1/Search/EditEntities/{guid}")]
        //[Route("api/v1/Test/Search/EditEntities/{guid}")]
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
            var absolutePath = string.Empty;
            try { absolutePath = Request.RequestUri.Segments[3].ToLower(); } catch (Exception exc) { }
            Guid? islocalpassword = null; 
            if (absolutePath == "test/")
                islocalpassword = _searchProvider.TestEnvironment_GetCurrentAuthType(guid);
            else
                islocalpassword = _searchProvider.GetCurrentAuthType(guid);

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

                if (absolutePath == "test/")
                {
                    #region test site
                    updatedVariable = _searchProvider.TestEnvironment_UpdateSearchForm(guid, new FormDataEntryViewModel()
                    {

                        ModifiedDate = DateTime.UtcNow,
                        CreatedBy = this.LoggedInUserId,
                        ActivityId = editForm.ActivityId,
                        FormDataEntryVariable = editForm.FormDataEntryVariable,
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
                                            //var alreadySent = _userLoginProvider.check_IsMailSend(true, username, updatedVariable.ThisUserId);
                                            if (isMailAlreadySent)
                                                isSendMail = false;
                                            #endregion
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
                            //var user = _userLoginProvider.GetById(thisUserId);
                            //Guid tempGuid = user != null ? (Guid)user.TempGuid : new Guid();

                            Guid tempGuid = _userLoginProvider.UpdateTempGuid(thisUserId);
                            if (tempGuid != Guid.Empty)
                            {
                                var emailService = new Services.EmailService();
                                var isSent = emailService.SendWelcomeEmail(emailid, username, Utilities.ConfigSettings.WebUrl + "account/resetpassword/" + tempGuid.ToString());

                                var updateStatus = _userLoginProvider.UpdateIsMailSend(thisUserId);
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    List<FormDataEntryVariableViewModel> formVariablesEntryList = new List<FormDataEntryVariableViewModel>();
                    if (!defaultFormNames.Contains(editForm.FormTitle))
                    {
                        if (editForm.FormDataEntryVariable != null)
                        {
                            FormDataEntryVariableViewModel formVariablesEntry = new FormDataEntryVariableViewModel();
                            editForm.FormDataEntryVariable.ForEach(variable =>
                            {
                                formVariablesEntry = variable;
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
                    updatedVariable = _searchProvider.UpdateSearchForm(guid, new FormDataEntryViewModel()
                    {
                        ModifiedDate = DateTime.UtcNow,
                        CreatedBy = this.LoggedInUserId,
                        ActivityId = editForm.ActivityId,
                        FormDataEntryVariable = formVariablesEntryList,
                        ProjectId = editForm.ProjectId,
                        FormId = editForm.FormId,
                        Status = editForm.Status,
                        TenantId = this.LoggedInUserTenantId,
                    });

                    if (updatedVariable == null)
                    {
                        return Request.CreateResponse(HttpStatusCode.NotFound, "Form was not found.");
                    }

                    _searchProvider.SaveChanges();

                    /*
                     send password creation mail to user for authenticationtype=local password.
                     **/
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
                                            //var alreadySent = _userLoginProvider.check_IsMailSend(false, username, updatedVariable.ThisUserId);
                                            if (isMailAlreadySent)
                                                isSendMail = false;
                                            #endregion
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

                        #region email/username/name from model
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
                        #endregion
                        if (emailid != string.Empty && username != string.Empty && isSendMail == true)
                        {
                            Int32 thisUserId = updatedVariable.ThisUserId != null ? (int)updatedVariable.ThisUserId : 0;
                            Guid tempGuid = _userLoginProvider.UpdateTempGuid(thisUserId);
                            if (tempGuid != Guid.Empty)
                            {
                                var emailService = new Services.EmailService();
                                if (!isMailAlreadySent)
                                {
                                    var isSent = emailService.SendWelcomeEmail(emailid, username, Utilities.ConfigSettings.WebUrl + "account/resetpassword/" + tempGuid.ToString());

                                    var updateStatus = _userLoginProvider.UpdateIsMailSend(thisUserId);
                                }
                                else
                                {
                                }
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
        [Route("api/v1/Search/DeleteSummaryPageFormData/{id}")]
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
            var addedVariable = _searchProvider.Delete(id, this.LoggedInUserId);
            return Request.CreateResponse(HttpStatusCode.OK, addedVariable);
        }


        /// <summary>
        /// Check entity existence location
        /// </summary>
        /// <remarks>
        /// Check entity existence location
        /// <br></br>
        /// <strong>Purpose.</strong>
        /// - Check entity existence location
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        /// <param name="entityId">id of entity</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "This error is returned by the server when we pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided id are found")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        //[SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(string))]
        [HttpGet]
        [Route("api/v1/Search/CheckEntityExistenceLocation/{entityId}")]
        [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage CheckEntityExistenceLocation(string entityId)
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
            var checkEntityLocation = _searchProvider.CheckEntityExistenceLocation(entityId, this.LoggedInUserId);
            if (checkEntityLocation == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Entity was not found."),
                });
            }
            return Request.CreateResponse(HttpStatusCode.OK, checkEntityLocation);
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
                WriteLog(exp.Message);
                return string.Empty;
            }
        }
    }
}