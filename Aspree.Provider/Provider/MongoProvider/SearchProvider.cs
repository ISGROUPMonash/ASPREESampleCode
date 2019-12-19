using Aspree.Core.ViewModels.MongoViewModels;
using Aspree.Core.ViewModels;
using Aspree.Data;
using Aspree.Data.MongoDB;
using Aspree.Provider.Interface;
using Aspree.Provider.Interface.MongoProvider;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspree.Core.Enum;

namespace Aspree.Provider.Provider.MongoProvider
{
    public class SearchProvider : ISearchProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly IPrivilegeProvider _privilegeProvider;
        private readonly AspreeEntities _dbContext;
        private readonly MongoDBContext _mongoDBContext;
        private readonly TestMongoDBContext _testMongoDBContext;
        private readonly IFormDataEntryProvider _formDataEntryProvider;

        private readonly IProjectDeployProvider _projectDeployProvider;
        public SearchProvider(
            IUserLoginProvider userLoginProvider
            , AspreeEntities dbContext
            , MongoDBContext mongoDBContext
            , IPrivilegeProvider privilegeProvider
            , TestMongoDBContext testMongoDBContext
            , IFormDataEntryProvider formDataEntryProvider
            , IProjectDeployProvider projectDeployProvider
            )
        {
            this._userLoginProvider = userLoginProvider;
            this._dbContext = dbContext;
            this._mongoDBContext = mongoDBContext;
            this._privilegeProvider = privilegeProvider;
            this._testMongoDBContext = testMongoDBContext;
            this._formDataEntryProvider = formDataEntryProvider;
            this._projectDeployProvider = projectDeployProvider;
        }

        public List<List<FormDataEntryVariableViewModel>> SearchEntities(SearchPageVariableViewModel model, string source = null)
        {
            List<FormDataEntryMongo> projectDetailscount = new List<FormDataEntryMongo>();
            if (model.FormTitle == EnumHelpers.GetEnumDescription((Core.Enum.DefaultFormName)(int)Core.Enum.DefaultFormName.Person_Registration)
                || model.FormTitle == EnumHelpers.GetEnumDescription((Core.Enum.DefaultFormName)(int)Core.Enum.DefaultFormName.Place__Group_Registration)
                || model.FormTitle == EnumHelpers.GetEnumDescription((Core.Enum.DefaultFormName)(int)Core.Enum.DefaultFormName.Project_Registration))
            {
                projectDetailscount = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().AsQueryable().Where(x => x.FormTitle == model.FormTitle).ToList();
            }
            else if (model.FormTitle == EnumHelpers.GetEnumDescription((Core.Enum.DefaultFormName)(int)Core.Enum.DefaultFormName.Participant_Registration))
            {
                projectDetailscount = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().AsQueryable().Where(x => x.ProjectGuid == model.ProjectId && x.FormTitle == model.FormTitle).ToList();

                #region Project Recruitment logic
                var condition = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, model.ProjectId);
                var project = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(condition).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                DateTime? startDateString = project.RecruitmentStartDate;
                DateTime? endDateString = project.RecruitmentEndDate;
                if (model.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration)) { }
                else
                {
                    if (startDateString == null)
                    {
                        throw new Exception("Recruitment for the project has not yet started.");
                    }

                    if (startDateString != null)
                    {
                        if (startDateString > DateTime.UtcNow)
                        {
                            throw new Exception("Recruitment for the project has not yet started.");
                        }
                    }
                    if (endDateString != null)
                    {
                        if (endDateString < DateTime.UtcNow)
                        {
                            throw new Exception("Recruitment for the project has ended.");
                        }
                    }
                }
                #endregion
            }

            List<ObjectId> result = new List<ObjectId>();
            string isSearchByEntId = model.SearchVariables.Where(x => x.Key == 3 && x.Value != "").Select(x => x.Value).FirstOrDefault();
            if (isSearchByEntId != null)
            {
                isSearchByEntId = !string.IsNullOrEmpty(isSearchByEntId) ? isSearchByEntId : "0";
                long entNumber = Convert.ToInt64(isSearchByEntId);
                result = projectDetailscount.Where(x => x.EntityNumber == entNumber).Select(x => x.Id).ToList();
            }
            else
            {
                result = new List<ObjectId>();
                foreach (var ent1 in projectDetailscount)
                {
                    var notFound = 0;
                    foreach (var item in model.SearchVariables)
                    {
                        string itemVal = !string.IsNullOrEmpty(item.Value) ? item.Value.ToLower() : string.Empty;
                        var first = ent1.formDataEntryVariableMongoList.Where(x =>
                        x.VariableId == item.Key
                        && itemVal.Equals(x.SelectedValues, StringComparison.InvariantCultureIgnoreCase)
                        ).FirstOrDefault();

                        if (first == null)
                        {
                            notFound++;
                            break;
                        }
                    }
                    if (notFound == 0)
                    {
                        result.Add(ent1.Id);
                    }
                }
            }
            List<List<FormDataEntryVariableViewModel>> returnList = new List<List<FormDataEntryVariableViewModel>>();
            List<FormDataEntryVariableViewModel> formDataEntryVariableViewModelList = new List<FormDataEntryVariableViewModel>();
            FormDataEntryVariableViewModel formDataEntryVariableViewModel = new FormDataEntryVariableViewModel();
            if (result.Count > 0)
            {
                var returnObj = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().AsQueryable().Where(x => result.Contains(x.Id));
                if (returnObj != null)
                {
                    foreach (var searchResult in returnObj)
                    {
                        formDataEntryVariableViewModelList = new List<FormDataEntryVariableViewModel>();
                        foreach (var item in searchResult.formDataEntryVariableMongoList)
                        {
                            formDataEntryVariableViewModel = new FormDataEntryVariableViewModel();
                            if (item.VariableName == Core.Enum.DefaultsVariables.AuthenticationMethod.ToString())
                            {
                                Guid authTypeGuid = Guid.Empty;
                                try { authTypeGuid = new Guid(item.SelectedValues); } catch (Exception ex) { }

                                var authType = _dbContext.LoginAuthTypeMasters.FirstOrDefault(x => x.Guid == authTypeGuid);
                                item.SelectedValues = authType != null ? authType.AuthTypeName : string.Empty;
                            }
                            formDataEntryVariableViewModel.ActivityGuid = searchResult.ActivityGuid;
                            formDataEntryVariableViewModel.ActivityId = searchResult.ActivityId;
                            formDataEntryVariableViewModel.CreatedBy = searchResult.CreatedById;
                            formDataEntryVariableViewModel.CreatedDate = searchResult.CreatedDate;
                            formDataEntryVariableViewModel.FormDataEntryStatus = searchResult.Status;
                            formDataEntryVariableViewModel.FormGuid = searchResult.FormGuid;
                            formDataEntryVariableViewModel.FormId = searchResult.FormId;
                            formDataEntryVariableViewModel.FormTitle = searchResult.FormTitle;
                            formDataEntryVariableViewModel.ParentId = searchResult.ParentEntityNumber.HasValue ? (int?)(searchResult.ParentEntityNumber) : null;
                            formDataEntryVariableViewModel.SelectedValues = item.SelectedValues;
                            formDataEntryVariableViewModel.VariableId = item.VariableId;
                            formDataEntryVariableViewModel.VariableName = item.VariableName;
                            formDataEntryVariableViewModelList.Add(formDataEntryVariableViewModel);
                        }
                        returnList.Add(formDataEntryVariableViewModelList);
                    }
                }
            }

            #region Search entity in SQL database
            if (returnList.Count() == 0 && source == null)
            {
                if (model.FormTitle != EnumHelpers.GetEnumDescription(Core.Enum.DefaultFormName.Participant_Registration))
                {
                    returnList = SearchEntityInSQLDB(model, "MONGO");
                    if (returnList == null)
                    {
                        returnList = new List<List<FormDataEntryVariableViewModel>>();
                    }
                }
            }
            #endregion

            if (returnList.Count() == 0)
                return null;
            else
                return returnList;
        }
        public List<List<FormDataEntryVariableViewModel>> TestEnvironment_SearchEntities(SearchPageVariableViewModel model)
        {
            List<FormDataEntryMongo> projectDetailscount = new List<FormDataEntryMongo>();
            if (model.FormTitle == EnumHelpers.GetEnumDescription((Core.Enum.DefaultFormName)(int)Core.Enum.DefaultFormName.Person_Registration)
                || model.FormTitle == EnumHelpers.GetEnumDescription((Core.Enum.DefaultFormName)(int)Core.Enum.DefaultFormName.Place__Group_Registration)
                || model.FormTitle == EnumHelpers.GetEnumDescription((Core.Enum.DefaultFormName)(int)Core.Enum.DefaultFormName.Project_Registration))
            {
                projectDetailscount = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().AsQueryable().Where(x => x.FormTitle == model.FormTitle).ToList();
            }
            else if (model.FormTitle == EnumHelpers.GetEnumDescription((Core.Enum.DefaultFormName)(int)Core.Enum.DefaultFormName.Participant_Registration))
            {
                projectDetailscount = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().AsQueryable().Where(x => x.ProjectGuid == model.ProjectId && x.FormTitle == model.FormTitle).ToList();

                #region Project Recruitment logic
                var condition = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, model.ProjectId);
                var project = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(condition).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                DateTime? startDateString = project.RecruitmentStartDate;
                DateTime? endDateString = project.RecruitmentEndDate;
                if (model.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration)) { }
                else
                {
                    if (startDateString == null)
                    {
                        throw new Exception("Recruitment for the project has not yet started.");
                    }

                    if (startDateString != null)
                    {
                        if (startDateString > DateTime.UtcNow)
                        {
                            throw new Exception("Recruitment for the project has not yet started.");
                        }
                    }
                    if (endDateString != null)
                    {
                        if (endDateString < DateTime.UtcNow)
                        {
                            throw new Exception("Recruitment for the project has ended.");
                        }
                    }
                }

                #endregion
            }
            List<ObjectId> result = new List<ObjectId>();
            string isSearchByEntId = model.SearchVariables.Where(x => x.Key == 3 && x.Value != "").Select(x => x.Value).FirstOrDefault();
            if (isSearchByEntId != null)
            {
                isSearchByEntId = !string.IsNullOrEmpty(isSearchByEntId) ? isSearchByEntId : "0";
                long entNumber = Convert.ToInt64(isSearchByEntId);
                result = projectDetailscount.Where(x => x.EntityNumber == entNumber).Select(x => x.Id).ToList();
            }
            else
            {
                result = new List<ObjectId>();
                foreach (var ent1 in projectDetailscount)
                {
                    var notFound = 0;
                    foreach (var item in model.SearchVariables)
                    {
                        string itemVal = !string.IsNullOrEmpty(item.Value) ? item.Value.Trim().ToLower() : string.Empty;
                        var first = ent1.formDataEntryVariableMongoList.Where(x =>
                        x.VariableId == item.Key
                        && itemVal.Equals(x.SelectedValues, StringComparison.InvariantCultureIgnoreCase)
                        ).FirstOrDefault();
                        if (first == null)
                        {
                            notFound++;
                            break;
                        }
                    }
                    if (notFound == 0)
                    {
                        result.Add(ent1.Id);
                    }
                }
            }
            List<List<FormDataEntryVariableViewModel>> returnList = new List<List<FormDataEntryVariableViewModel>>();
            List<FormDataEntryVariableViewModel> formDataEntryVariableViewModelList = new List<FormDataEntryVariableViewModel>();
            FormDataEntryVariableViewModel formDataEntryVariableViewModel = new FormDataEntryVariableViewModel();
            if (result.Count > 0)
            {
                var returnObj = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().AsQueryable().Where(x => result.Contains(x.Id));
                if (returnObj != null)
                {
                    foreach (var searchResult in returnObj)
                    {
                        formDataEntryVariableViewModelList = new List<FormDataEntryVariableViewModel>();
                        foreach (var item in searchResult.formDataEntryVariableMongoList)
                        {
                            formDataEntryVariableViewModel = new FormDataEntryVariableViewModel();
                            if (item.VariableName == Core.Enum.DefaultsVariables.AuthenticationMethod.ToString())
                            {
                                Guid authTypeGuid = Guid.Empty;
                                try { authTypeGuid = new Guid(item.SelectedValues); } catch (Exception ex) { }

                                var authType = _dbContext.LoginAuthTypeMasters.FirstOrDefault(x => x.Guid == authTypeGuid);
                                item.SelectedValues = authType != null ? authType.AuthTypeName : string.Empty;
                            }
                            formDataEntryVariableViewModel.ActivityGuid = searchResult.ActivityGuid;
                            formDataEntryVariableViewModel.ActivityId = searchResult.ActivityId;
                            formDataEntryVariableViewModel.CreatedBy = searchResult.CreatedById;
                            formDataEntryVariableViewModel.CreatedDate = searchResult.CreatedDate;
                            formDataEntryVariableViewModel.FormDataEntryStatus = searchResult.Status;
                            formDataEntryVariableViewModel.FormGuid = searchResult.FormGuid;
                            formDataEntryVariableViewModel.FormId = searchResult.FormId;
                            formDataEntryVariableViewModel.FormTitle = searchResult.FormTitle;
                            formDataEntryVariableViewModel.ParentId = searchResult.ParentEntityNumber.HasValue ? (int?)(searchResult.ParentEntityNumber) : null;
                            formDataEntryVariableViewModel.SelectedValues = item.SelectedValues;
                            formDataEntryVariableViewModel.VariableId = item.VariableId;
                            formDataEntryVariableViewModel.VariableName = item.VariableName;
                            formDataEntryVariableViewModelList.Add(formDataEntryVariableViewModel);
                        }
                        returnList.Add(formDataEntryVariableViewModelList);
                    }
                }
            }
            if (returnList.Count() == 0)
                return null;
            else
                return returnList;
        }

        public FormDataEntryVariableViewModel ToSearchModel(FormDataEntryVariable dataentry)
        {
            if (dataentry != null)
            {
                string formTitle = string.Empty;
                try
                {
                    formTitle = dataentry.FormDataEntry.Form.FormTitle;
                }
                catch (Exception ex) { }
                return new FormDataEntryVariableViewModel()
                {
                    Guid = dataentry.Guid,
                    Id = dataentry.Id,
                    SelectedValues = dataentry.SelectedValues,
                    VariableId = dataentry.VariableId,
                    FormDataEntryId = dataentry.FormDataEntryId,
                    ParentId = dataentry.ParentId != null ? dataentry.ParentId : (int?)null,
                    FormId = dataentry.FormDataEntry.FormId != null ? dataentry.FormDataEntry.FormId : (int?)null,
                    ActivityId = dataentry.FormDataEntry.ActivityId,
                    FormGuid = dataentry.FormDataEntry.Form.Guid,
                    ActivityGuid = dataentry.FormDataEntry.Activity.Guid,
                    CreatedDate = dataentry.FormDataEntry.CreatedDate,
                    FormDataEntryStatus = dataentry.FormDataEntry.Status,
                    VariableName = dataentry.Variable.VariableName,
                    FormTitle = formTitle,
                };
            }
            else
            {
                return null;
            }
        }
        public FormDataEntryViewModel Create(FormDataEntryViewModel model)
        {
            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);
            UserLogin thisUserIdDetails = null;
            if (model.ThisUserId != null)
                thisUserIdDetails = _dbContext.UserLogins.FirstOrDefault(x => x.Id == model.ThisUserId);
            var condition = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, model.ProjectId);
            var project = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(condition).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
            Form isDefaultForm = _dbContext.Forms.FirstOrDefault(x => x.Guid == model.FormId);
            bool isRegistrationForm = isDefaultForm != null ? (isDefaultForm.IsDefaultForm == (int)Core.Enum.DefaultFormType.Default ? true : false) : false;
            var activity = new ActivitiesMongo(); 
            var form = new FormsMongo();
            if (isRegistrationForm)
            {
                activity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityName == isDefaultForm.FormTitle);
                form = activity.FormsListMongo.FirstOrDefault(x => x.FormTitle == isDefaultForm.FormTitle);
            }
            else
            {
                activity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == model.ActivityId);
                if (activity == null)
                {
                    var currentProject = _dbContext.Forms.FirstOrDefault(c => c.Guid == model.FormId);
                    if (currentProject != null)
                    {
                        condition = Query<ProjectDeployViewModel>.EQ(q => q.ProjectId, currentProject.ProjectId);
                        project = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(condition).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                        activity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == model.ActivityId);
                    }
                }
                form = activity.FormsListMongo.FirstOrDefault(x => x.FormGuid == model.FormId);
            }
            var entityType = _dbContext.EntityTypes.FirstOrDefault(x => x.Name == form.FormEntityTypes.FirstOrDefault());
            int entityNumber = 0;
            Int64? parentEntityNumber = (int?)null;
            if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Registration))
            {
                entityNumber = GenerateRandomNo();
            }
            else
            {
                entityNumber = GenerateRandomNo6Digit();
                try { parentEntityNumber = Convert.ToInt32(model.ParticipantId); } catch (Exception exc) { }
            }

            #region form-dataentry            
            FormDataEntryMongo formDataEntryMongo = new FormDataEntryMongo();
            formDataEntryMongo.ProjectId = project.ProjectId;
            formDataEntryMongo.ProjectGuid = model.ProjectId;
            formDataEntryMongo.ProjectName = project.ProjectName;

            formDataEntryMongo.ActivityId = activity.ActivityId;
            formDataEntryMongo.ActivityGuid = activity.ActivityGuid;
            formDataEntryMongo.ActivityName = activity.ActivityName;

            formDataEntryMongo.FormId = form.FormId;
            formDataEntryMongo.FormGuid = form.FormGuid;
            formDataEntryMongo.FormTitle = form.FormTitle;

            formDataEntryMongo.EntityTypeName = entityType.Name;
            formDataEntryMongo.EntityTypeId = entityType.Id;
            formDataEntryMongo.EntityTypeGuid = entityType.Guid;

            formDataEntryMongo.Status = model.Status;
            formDataEntryMongo.CreatedById = createdBy.Id;
            formDataEntryMongo.CreatedByGuid = createdBy.Guid;
            formDataEntryMongo.CreatedByName = createdBy.FirstName + " " + createdBy.LastName;
            formDataEntryMongo.CreatedDate = DateTime.UtcNow;

            formDataEntryMongo.ThisUserId = model.ThisUserId;
            formDataEntryMongo.ThisUserName = thisUserIdDetails != null ? thisUserIdDetails.FirstName + " " + thisUserIdDetails.LastName : null;
            formDataEntryMongo.ThisUserGuid = thisUserIdDetails != null ? thisUserIdDetails.Guid : (Guid?)null;

            formDataEntryMongo.EntityNumber = entityNumber;
            formDataEntryMongo.ParentEntityNumber = parentEntityNumber;

            formDataEntryMongo.formDataEntryVariableMongoList = new List<FormDataEntryVariableMongo>();
            FormDataEntryVariableMongo formDataEntryVariableMongo = new FormDataEntryVariableMongo();

            var saveParent = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 3);
            var dbEntid = _dbContext.Variables.FirstOrDefault(x => x.VariableName == "EntID");

            formDataEntryVariableMongo.VariableId = saveParent.VariableId;
            formDataEntryVariableMongo.VariableGuid = dbEntid.Guid;
            formDataEntryVariableMongo.VariableName = dbEntid.VariableName;
            formDataEntryVariableMongo.SelectedValues = entityNumber.ToString();
            formDataEntryVariableMongo.CreatedBy = createdBy.Id;
            formDataEntryVariableMongo.CreatedDate = DateTime.UtcNow;
            formDataEntryVariableMongo.ParentId = null;

            formDataEntryMongo.formDataEntryVariableMongoList.Add(formDataEntryVariableMongo);

            foreach (var item in model.FormDataEntryVariable)
            {
                formDataEntryVariableMongo = new FormDataEntryVariableMongo();
                if (item.VariableId == 3)
                {
                    continue;
                }
                var variableData = form.VariablesListMongo.FirstOrDefault(x => x.VariableId == item.VariableId);
                formDataEntryVariableMongo.VariableId = item.VariableId;
                formDataEntryVariableMongo.VariableGuid = variableData.VariableGuid;
                formDataEntryVariableMongo.VariableName = variableData.VariableName;
                formDataEntryVariableMongo.SelectedValues = !string.IsNullOrEmpty(item.SelectedValues) ? item.SelectedValues.Trim() : string.Empty;
                formDataEntryVariableMongo.CreatedBy = createdBy.Id;
                formDataEntryVariableMongo.CreatedDate = DateTime.UtcNow;
                formDataEntryVariableMongo.ParentId = entityNumber;
                if (variableData.VariableTypeName == VariableTypes.FileType.ToString())
                {
                    formDataEntryVariableMongo.FileName = item.FileName;
                }
                formDataEntryMongo.formDataEntryVariableMongoList.Add(formDataEntryVariableMongo);
            }
            #endregion
            #region save in login table
            string fname = string.Empty;
            string lname = string.Empty;
            if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration))
            {
                var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                var f = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                lname = l != null ? l.SelectedValues : string.Empty;
                fname = f != null ? f.SelectedValues : string.Empty;
            }
            else if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration))
            {
                var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                var f = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                var m = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 17);
                lname = l != null ? l.SelectedValues : string.Empty;
                fname = f != null ? f.SelectedValues : string.Empty + " " + m != null ? m.SelectedValues : string.Empty;
            }
            else if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration))
            {
                var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                fname = l != null ? l.SelectedValues : string.Empty;
            }

            if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration))
            {
                var emali = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38).SelectedValues : string.Empty;
                var username = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40).SelectedValues : string.Empty;
                if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration))
                {
                    var EntGrpVar = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 5) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 5).SelectedValues : string.Empty;
                    if (EntGrpVar != "15")
                    {
                        var loginTbl = _dbContext.UserLogins.FirstOrDefault(x => x.Id == formDataEntryMongo.ThisUserId);
                        if (loginTbl != null)
                        {
                            loginTbl.Password = null;
                            loginTbl.Salt = null;
                            loginTbl.SecurityQuestionId = null;
                            loginTbl.Answer = null;
                            _dbContext.SaveChanges();
                        }
                    }
                }

                var authTypeGuid = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51).SelectedValues : new Guid().ToString();

                int at = 0;
                try
                {
                    at = Convert.ToInt32(authTypeGuid);
                }
                catch (Exception dd)
                {
                    at = 1;
                }
                var authTypeId = _dbContext.LoginAuthTypeMasters.Where(x => x.DateDeactivated == null && x.Id == at).Select(x => new { id = x.Guid.ToString(), name = x.AuthTypeName }).FirstOrDefault();
                var authType = _dbContext.LoginAuthTypeMasters.FirstOrDefault(x => x.Guid == new Guid(authTypeId.id));
                var userrole = _dbContext.Roles.FirstOrDefault(x => x.Name == "Data Entry Supervisor");

                #region Is API Access to export data
                bool isApiAccess = false;
                try
                {
                    if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration))
                    {
                        Variable authVariable = _dbContext.Variables.FirstOrDefault(x => x.VariableName == "AuthenticationMethod");
                        Variable isApiAccessEnabled = _dbContext.Variables.FirstOrDefault(x => x.VariableName == "ApiAccessEnabled");
                        LoginAuthTypeMaster loginAuthTypeMaster = _dbContext.LoginAuthTypeMasters.FirstOrDefault(x => x.AuthTypeName.ToLower() == Core.Enum.AuthenticationTypes.Local_Password.ToString().ToLower().Replace("_", " "));
                        int authVariableId = authVariable != null ? authVariable.Id : 0;

                        if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration))
                        {
                            var isLocalPassword = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == authVariableId);
                            string selectedVal = isLocalPassword != null ? isLocalPassword.SelectedValues : string.Empty;
                            string localPWGuid = loginAuthTypeMaster != null ? Convert.ToString(loginAuthTypeMaster.Guid) : Convert.ToString(Guid.Empty);

                            int isApiAccessEnabledId = isApiAccessEnabled != null ? isApiAccessEnabled.Id : 0;

                            if (selectedVal == localPWGuid)
                            {
                                var isApiAccessEnabledData = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == isApiAccessEnabledId);
                                isApiAccess = isApiAccessEnabledData != null ? Convert.ToBoolean(isApiAccessEnabledData.SelectedValues) : false;
                            }
                        }
                        else if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration))
                        {
                            var variableEntType = _dbContext.Variables.FirstOrDefault(x => x.VariableName == "EntType");
                            var entType = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == variableEntType.Id);
                            string entTypeId = entType != null ? entType.SelectedValues : string.Empty;
                            if (entTypeId == "15")
                            {
                                var isLocalPassword = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == authVariableId);
                                string selectedVal = isLocalPassword != null ? isLocalPassword.SelectedValues : string.Empty;
                                string localPWGuid = loginAuthTypeMaster != null ? Convert.ToString(loginAuthTypeMaster.Guid) : Convert.ToString(Guid.Empty);

                                int isApiAccessEnabledId = isApiAccessEnabled != null ? isApiAccessEnabled.Id : 0;

                                if (selectedVal == localPWGuid)
                                {
                                    var isApiAccessEnabledData = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == isApiAccessEnabledId);
                                    isApiAccess = isApiAccessEnabledData != null ? Convert.ToBoolean(isApiAccessEnabledData.SelectedValues) : false;
                                }
                            }
                        }
                    }
                }
                catch (Exception exc) { Console.WriteLine(exc); }
                #endregion
                UserLoginViewModel entityUserlogin = new UserLoginViewModel();
                entityUserlogin.FirstName = fname;
                entityUserlogin.LastName = lname;
                entityUserlogin.Email = emali;
                entityUserlogin.TenantId = model.TenantId != null ? (Guid)model.TenantId : new Guid();
                entityUserlogin.AuthTypeId = authType != null ? authType.AuthType : 1;
                entityUserlogin.CreatedBy = model.CreatedBy;
                entityUserlogin.UserTypeId = (int)Core.Enum.UsersLoginType.Entity;
                entityUserlogin.TempGuid = Guid.NewGuid();
                entityUserlogin.RoleId = userrole != null ? userrole.Guid : new Guid();
                entityUserlogin.UserName = !string.IsNullOrEmpty(username) ? username : entityNumber.ToString();
                entityUserlogin.IsApiAccessEnabled = isApiAccess;
                entityUserlogin.Status = (int)Core.Enum.Status.InActive;
                entityUserlogin.IsUserApprovedBySystemAdmin = false;
                var savedUser = _userLoginProvider.Create(entityUserlogin);
                if (savedUser != null)
                {
                    formDataEntryMongo.ThisUserId = savedUser != null ? savedUser.Id : (int?)null;
                    formDataEntryMongo.ThisUserGuid = savedUser != null ? savedUser.Guid : (Guid?)null;
                    formDataEntryMongo.ThisUserName = savedUser != null ? savedUser.FirstName + " " + savedUser.LastName : null;
                }
            }
            #endregion
            if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Registration))
            {
                #region save project in formdataentry table in SQL
                var formDataEntry = new FormDataEntry()
                {
                    Guid = Guid.NewGuid(),
                    ActivityId = activity != null ? activity.ActivityId : 0,
                    ProjectId = project != null ? project.ProjectId : 0,
                    EntityId = entityType != null ? entityType.Id : 0,
                    SubjectId = model.SubjectId,
                    Status = model.Status,
                    CreatedBy = createdBy.Id,
                    CreatedDate = DateTime.UtcNow,
                    FormId = form != null ? form.FormId : (int?)null,
                    ThisUserId = model.ThisUserId,
                    EntityNumber = entityNumber,
                    ParentEntityNumber = parentEntityNumber != null ? Convert.ToInt32(parentEntityNumber) : (int?)null,
                };
                _dbContext.FormDataEntries.Add(formDataEntry);
                var saveParentProj = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 3);
                saveParentProj.SelectedValues = entityNumber.ToString();
                var parentFormDataEntryVariable = new FormDataEntryVariable()
                {
                    Guid = Guid.NewGuid(),
                    VariableId = saveParentProj.VariableId,
                    SelectedValues = saveParentProj.SelectedValues,
                    SelectedValues_int = saveParentProj.SelectedValues_int,
                    SelectedValues_float = saveParentProj.SelectedValues_float,
                    FormDataEntryId = formDataEntry.Id,
                    CreatedBy = createdBy.Id,
                    CreatedDate = DateTime.UtcNow,
                };
                _dbContext.FormDataEntryVariables.Add(parentFormDataEntryVariable);
                SaveChanges();
                foreach (var item in model.FormDataEntryVariable)
                {
                    if (item.VariableId == 3)
                    {
                        continue;
                    }
                    var formDataEntryVariable = new FormDataEntryVariable()
                    {
                        Guid = Guid.NewGuid(),
                        VariableId = item.VariableId,
                        SelectedValues = item.SelectedValues,
                        SelectedValues_int = item.SelectedValues_int,
                        SelectedValues_float = item.SelectedValues_float,
                        FormDataEntryId = formDataEntry.Id,
                        CreatedBy = createdBy.Id,
                        CreatedDate = DateTime.UtcNow,
                        ParentId = parentFormDataEntryVariable.Id,
                    };
                    _dbContext.FormDataEntryVariables.Add(formDataEntryVariable);
                }
                SaveChanges();
                #endregion

                #region project default activities and form creation                
                var tenant = _dbContext.Tenants.FirstOrDefault(x => x.Guid == model.TenantId);
                var user = _dbContext.UserLogins.FirstOrDefault(u => u.Email == "systemadmin@aspree.com");
                var role = _dbContext.Roles.FirstOrDefault(v => v.Name == "System Admin");
                this._dbContext.ProjectStaffMemberRoles.Add(new Data.ProjectStaffMemberRole
                {
                    Guid = Guid.NewGuid(),
                    ProjectId = formDataEntry.Id,
                    UserId = user.Id,
                    RoleId = role.Id,
                    CreatedBy = createdBy.Id,
                    StaffCreatedDate = DateTime.UtcNow,
                    IsActiveProjectUser = true,
                    ProjectJoinedDate = DateTime.UtcNow,
                });
                var userTest = _dbContext.UserLogins.FirstOrDefault(u => u.Email == "testsystemadmin@aspree.com");
                this._dbContext.ProjectStaffMemberRoles.Add(new Data.ProjectStaffMemberRole
                {
                    Guid = Guid.NewGuid(),
                    ProjectId = formDataEntry.Id,
                    UserId = userTest != null ? userTest.Id : 1,
                    RoleId = role.Id,
                    CreatedBy = createdBy.Id,
                    StaffCreatedDate = DateTime.UtcNow,
                    IsActiveProjectUser = true,
                    ProjectJoinedDate = DateTime.UtcNow,
                });
                SaveChanges();
                _privilegeProvider.CreateDefaultFormsForProject(formDataEntry.Id, createdBy.Id, tenant != null ? tenant.Id : 0);
                _privilegeProvider.CreateDefaultActivitiesForProject(formDataEntry.Id, createdBy.Id, tenant != null ? tenant.Id : 0);

                SaveChanges();
                #endregion

                #region Project Mongo
                try
                {
                    List<Guid> activitiesList = new List<Guid>();
                    activitiesList = _dbContext.Activities.Where(c => c.ProjectId == formDataEntry.Id && c.IsDefaultActivity == (int)DefaultActivityType.Default).Select(c => c.Guid).ToList();
                    ProjectDeployViewModel projectDeployViewModel = _projectDeployProvider.Create(formDataEntry.Guid, activitiesList, (int)ActivityDeploymentStatus.Deployed);
                }
                catch (Exception projectDeployViewModelEx) { Console.WriteLine(projectDeployViewModelEx); }
                #endregion
            }
            if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Registration))
            {
                #region linkage from summary page                
                UserLogin activityCompletedBy = null;
                if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration))
                {
                    activityCompletedBy = new UserLogin()
                    {
                        Id = createdBy.Id,
                        FirstName = createdBy.FirstName,
                        LastName = createdBy.LastName,
                        Email = createdBy.Email,
                        IsUserApprovedBySystemAdmin = createdBy.IsUserApprovedBySystemAdmin,
                        Status = createdBy.Status,
                        AuthTypeId = createdBy.AuthTypeId,
                        UserTypeId = createdBy.UserTypeId,
                        UserName = createdBy.UserName,
                        Guid = createdBy.Guid,
                        IsApiAccessEnabled = createdBy.IsApiAccessEnabled,
                    };
                }
                else
                {
                    activityCompletedBy = _dbContext.UserLogins.FirstOrDefault(x => x.UserName.ToLower() == "systemadmin@aspree.com");
                }
                SummaryPageActivityViewModel summaryPageActivityViewModel = new SummaryPageActivityViewModel();
                summaryPageActivityViewModel.ActivityId = activity.ActivityId;
                summaryPageActivityViewModel.ActivityGuid = activity.ActivityGuid;
                summaryPageActivityViewModel.ActivityName = activity.ActivityName;
                summaryPageActivityViewModel.ActivityCompletedById = activityCompletedBy.Id;
                summaryPageActivityViewModel.ActivityCompletedByGuid = activityCompletedBy.Guid;
                summaryPageActivityViewModel.ActivityCompletedByName = activityCompletedBy.FirstName + " " + activityCompletedBy.LastName;
                summaryPageActivityViewModel.ActivityDate = DateTime.UtcNow.Date;
                summaryPageActivityViewModel.ProjectGuid = project.ProjectGuid;
                summaryPageActivityViewModel.ProjectName = project.ProjectName;
                summaryPageActivityViewModel.PersonEntityId = entityNumber;
                summaryPageActivityViewModel.CreatedByName = createdBy.FirstName + " " + createdBy.LastName;
                summaryPageActivityViewModel.CreatedDate = DateTime.UtcNow;
                summaryPageActivityViewModel.ProjectVersion = project.ProjectInternalVersion;
                summaryPageActivityViewModel.SummaryPageActivityFormsList = new List<SummaryPageActivityForms>();
                summaryPageActivityViewModel.SummaryPageActivityFormsList.Add(new SummaryPageActivityForms()
                {
                    FormId = form.FormId,
                    FormGuid = form.FormGuid,
                    FormTitle = form.FormTitle,
                    FormStatusId = (int)Core.Enum.FormStatusTypes.Draft,
                    FormStatusName = Enum.GetName(typeof(Core.Enum.FormStatusTypes), 3),
                });
                var summaryPageActivity = _mongoDBContext._database.GetCollection<BsonDocument>("SummaryPageActivity");
                var summaryPageActivityResult = summaryPageActivity.Insert(summaryPageActivityViewModel);

                formDataEntryMongo.SummaryPageActivityObjId = Convert.ToString(summaryPageActivityViewModel.Id);
                #endregion
            }
            else
            {
                formDataEntryMongo.SummaryPageActivityObjId = model.SummaryPageActivityObjId;
            }

            if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Linkage))
            {
                #region assig project to user
                var projectLinkageCondition = Query<FormDataEntryMongo>.EQ(q => q.EntityNumber, parentEntityNumber);
                var projectLinkageConditionEntity = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(projectLinkageCondition).AsQueryable().FirstOrDefault();
                if (projectLinkageConditionEntity != null)
                {
                    UserLogin uLogin = _dbContext.UserLogins.FirstOrDefault(x => x.Id == projectLinkageConditionEntity.ThisUserId);
                    if (uLogin != null)
                    {
                        var proRole = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.ProRole);//ProRole variable of variable table
                        Role urole = new Role();
                        if (proRole != null)
                        {
                            Guid role = !string.IsNullOrEmpty(proRole.SelectedValues) ? new Guid(proRole.SelectedValues) : Guid.Empty;
                            urole = _dbContext.Roles.FirstOrDefault(x => x.Guid == role);
                        }
                        var linkedProjectId = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.LnkPro);  //LnkPro(Linked Project) variable
                        Guid linkedProjectGuid = Guid.Parse(linkedProjectId.SelectedValues);
                        var conditionProjStaff = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, linkedProjectGuid);
                        var conditionProjStaffResult = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(conditionProjStaff).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();

                        if (conditionProjStaffResult != null)
                        {
                            if (!conditionProjStaffResult.ProjectStaffListMongo.Any(x => x.StaffGuid == uLogin.Guid && x.Role == urole.Name))
                            {
                                conditionProjStaffResult.ProjectStaffListMongo.Add(new ProjectStaffMongo()
                                {
                                    Role = urole != null ? urole.Name : string.Empty,
                                    StaffName = uLogin != null ? uLogin.FirstName + " " + uLogin.LastName : string.Empty,
                                    StaffGuid = uLogin != null ? uLogin.Guid : Guid.Empty,
                                });
                                ////Mongo Query    lok-----------------------------------------------------
                                var updateProjectObjectId = Query<ProjectDeployViewModel>.EQ(p => p.Id, conditionProjStaffResult.Id);
                                // Document Collections  
                                var updateProjectCollection = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects");
                                // Document Update which need Id and Data to Update  
                                var isUpdateProj = updateProjectCollection.Update(updateProjectObjectId, MongoDB.Driver.Builders.Update.Replace(conditionProjStaffResult), MongoDB.Driver.UpdateFlags.None);

                                DateTime? projectJoinDate = null;
                                #region Set projectLinkage end date
                                try
                                {
                                    var projJoindDate = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Join);
                                    string jDate = projJoindDate != null ? projJoindDate.SelectedValues : string.Empty;
                                    projectJoinDate = Convert.ToDateTime(jDate);
                                }
                                catch (Exception joinDete)
                                { }

                                try
                                {
                                    formDataEntryMongo.ProjectLinkage_LinkedProjectId = conditionProjStaffResult.ProjectGuid;
                                    formDataEntryMongo.ThisUserId = uLogin.Id;
                                    formDataEntryMongo.ThisUserGuid = uLogin.Guid;
                                    formDataEntryMongo.ThisUserName = uLogin.FirstName + " " + uLogin.LastName;

                                    var projectLeftDateActiveUser = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Actv);
                                    if (projectLeftDateActiveUser != null)
                                    {
                                        formDataEntryMongo.ProjectLinkage_IsActiveProjectUser = !string.IsNullOrEmpty(projectLeftDateActiveUser.SelectedValues) ? (projectLeftDateActiveUser.SelectedValues == "1" ? true : false) : false;
                                        if (projectLeftDateActiveUser.SelectedValues == "0")
                                        {
                                            var projectLeftDateEnd = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.End);
                                            string edate = projectLeftDateEnd != null ? projectLeftDateEnd.SelectedValues : string.Empty;
                                            formDataEntryMongo.ProjectLinkage_ProjectLeftDate = Convert.ToDateTime(edate);
                                        }
                                    }
                                }
                                catch (Exception exLinkage) { }
                                #endregion
                                if (urole == null)
                                {
                                    formDataEntryMongo.ProjectLinkage_IsActiveProjectUser = false;
                                }
                                #region assigne project to user in login table
                                var projectstaffRolesSQL = new ProjectStaffMemberRole()
                                {
                                    Guid = Guid.NewGuid(),
                                    ProjectId = conditionProjStaffResult != null ? conditionProjStaffResult.ProjectId : 1,
                                    UserId = uLogin.Id,
                                    RoleId = urole != null ? urole.Id : (int)Core.Enum.RoleTypes.Data_Entry,
                                    CreatedBy = createdBy.Id,
                                    StaffCreatedDate = DateTime.UtcNow,


                                    IsActiveProjectUser = formDataEntryMongo.ProjectLinkage_IsActiveProjectUser,
                                    ProjectJoinedDate = projectJoinDate,
                                    ProjectLeftDate = formDataEntryMongo.ProjectLinkage_ProjectLeftDate,

                                };
                                _dbContext.ProjectStaffMemberRoles.Add(projectstaffRolesSQL);
                                SaveChanges();
                                #endregion
                            }
                        }
                        else
                        {
                            #region assigne project to user in login table
                            DateTime? projectJoinDate = null;
                            try
                            {
                                var projJoindDate = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Join);
                                string jDate = projJoindDate != null ? projJoindDate.SelectedValues : string.Empty;
                                projectJoinDate = Convert.ToDateTime(jDate);
                            }
                            catch (Exception joinDete)
                            { }

                            var projId = _dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == linkedProjectGuid);
                            #region Set projectLinkage end date
                            try
                            {
                                formDataEntryMongo.ProjectLinkage_LinkedProjectId = projId.Guid;
                                formDataEntryMongo.ThisUserId = uLogin.Id;
                                formDataEntryMongo.ThisUserGuid = uLogin.Guid;
                                formDataEntryMongo.ThisUserName = uLogin.FirstName + " " + uLogin.LastName;

                                var projectLeftDateActiveUser = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Actv);
                                if (projectLeftDateActiveUser != null)
                                {
                                    formDataEntryMongo.ProjectLinkage_IsActiveProjectUser = !string.IsNullOrEmpty(projectLeftDateActiveUser.SelectedValues) ? (projectLeftDateActiveUser.SelectedValues == "1" ? true : false) : false;
                                    if (projectLeftDateActiveUser.SelectedValues == "0")
                                    {
                                        var projectLeftDateEnd = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.End);
                                        string edate = projectLeftDateEnd != null ? projectLeftDateEnd.SelectedValues : string.Empty;
                                        formDataEntryMongo.ProjectLinkage_ProjectLeftDate = Convert.ToDateTime(edate);
                                    }
                                }
                            }
                            catch (Exception exLinkage) { }
                            #endregion

                            IEnumerable<ProjectStaffMemberRole> projectStaffMemberRoles = this._dbContext.ProjectStaffMemberRoles.Where(x => x.UserId == uLogin.Id && x.FormDataEntry.Guid == linkedProjectGuid);
                            _dbContext.ProjectStaffMemberRoles.RemoveRange(projectStaffMemberRoles);
                            SaveChanges();

                            if (urole == null)
                            {
                                formDataEntryMongo.ProjectLinkage_IsActiveProjectUser = false;
                            }
                            var projectstaffRolesSQL = new ProjectStaffMemberRole()
                            {
                                Guid = Guid.NewGuid(),
                                ProjectId = projId != null ? projId.Id : 1,
                                UserId = uLogin.Id,
                                RoleId = urole != null ? urole.Id : (int)Core.Enum.RoleTypes.Data_Entry,
                                CreatedBy = createdBy.Id,
                                StaffCreatedDate = DateTime.UtcNow,
                                IsActiveProjectUser = formDataEntryMongo.ProjectLinkage_IsActiveProjectUser,
                                ProjectJoinedDate = projectJoinDate,
                                ProjectLeftDate = formDataEntryMongo.ProjectLinkage_ProjectLeftDate,
                            };
                            _dbContext.ProjectStaffMemberRoles.Add(projectstaffRolesSQL);
                            SaveChanges();
                            #endregion
                        }
                    }
                }
                #endregion                
            }

            if (model.Status == (int)Core.Enum.FormStatusTypes.Published || model.Status == (int)Core.Enum.FormStatusTypes.Draft || model.Status == (int)Core.Enum.FormStatusTypes.Submitted_for_review)
            {
                #region Update status of summary page form
                var updateSummaryPageActivity_condition = Query<SummaryPageActivityViewModel>.EQ(q => q.Id, new ObjectId(formDataEntryMongo.SummaryPageActivityObjId));
                var summaryPageActivity = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").Find(updateSummaryPageActivity_condition).AsQueryable().FirstOrDefault();
                if (summaryPageActivity != null)
                {
                    List<SummaryPageActivityForms> newForms = new List<SummaryPageActivityForms>();
                    foreach (var frm in summaryPageActivity.SummaryPageActivityFormsList)
                    {
                        if (isRegistrationForm)
                        {
                            if (frm.FormTitle == form.FormTitle)
                            {
                                frm.FormStatusId = model.Status;
                                frm.FormStatusName = Enum.GetName(typeof(Core.Enum.FormStatusTypes), model.Status);

                                newForms.Add(frm);
                            }
                            else
                            {
                                newForms.Add(frm);
                            }
                        }
                        else
                        {
                            if (frm.FormGuid == form.FormGuid)
                            {
                                frm.FormStatusId = model.Status;
                                frm.FormStatusName = Enum.GetName(typeof(Core.Enum.FormStatusTypes), model.Status);
                                newForms.Add(frm);
                            }
                            else
                            {
                                newForms.Add(frm);
                            }
                        }
                    }
                    var document_SummaryPageActivity = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity");
                    var detailscount_SummaryPageActivity = document_SummaryPageActivity.FindAs<SummaryPageActivityViewModel>(Query.EQ("_id", summaryPageActivity.Id)).Count();
                    if (detailscount_SummaryPageActivity > 0)
                    {
                        var qry_summaryPageActivity = Query<SummaryPageActivityViewModel>.EQ(p => p.Id, summaryPageActivity.Id);
                        var summaryPageActivityResult = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").FindOne(qry_summaryPageActivity);
                        summaryPageActivityResult.SummaryPageActivityFormsList = newForms;
                        var collection = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity");
                        try { var result1 = collection.Update(qry_summaryPageActivity, Update.Replace(summaryPageActivityResult), UpdateFlags.None); }
                        catch (Exception e) { }
                    }
                }
                #endregion
            }
            FormDataEntryViewModel mdl = new FormDataEntryViewModel();
            mdl.ParticipantId = Convert.ToString(formDataEntryMongo.EntityNumber);
            formDataEntryMongo.ProjectVersion = project.ProjectInternalVersion;
            var document = _mongoDBContext._database.GetCollection<BsonDocument>("UserEntities");
            var result = document.Insert(formDataEntryMongo);
            return mdl;
        }
        public FormDataEntryViewModel TestEnvironment_Create(FormDataEntryViewModel model)
        {
            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);
            UserLogin thisUserIdDetails = null;
            if (model.ThisUserId != null)
                thisUserIdDetails = _dbContext.UserLogins.FirstOrDefault(x => x.Id == model.ThisUserId);

            var condition = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, model.ProjectId);
            var project = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(condition).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();

            #region 0609
            Form isDefaultForm = _dbContext.Forms.FirstOrDefault(x => x.Guid == model.FormId);
            bool isRegistrationForm = isDefaultForm != null ? (isDefaultForm.IsDefaultForm == (int)Core.Enum.DefaultFormType.Default ? true : false) : false;

            var activity = new ActivitiesMongo(); 
            var form = new FormsMongo();
            if (isRegistrationForm)
            {
                activity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityName == isDefaultForm.FormTitle);
                form = activity.FormsListMongo.FirstOrDefault(x => x.FormTitle == isDefaultForm.FormTitle);
            }
            else
            {
                activity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == model.ActivityId);
                form = activity.FormsListMongo.FirstOrDefault(x => x.FormGuid == model.FormId);
            }
            #endregion

            var entityType = _dbContext.EntityTypes.FirstOrDefault(x => x.Name == form.FormEntityTypes.FirstOrDefault());
            int entityNumber = 0;
            Int64? parentEntityNumber = (int?)null;
            if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Registration))
            {
                entityNumber = TestEnvironment_GenerateRandomNo();
            }
            else
            {
                entityNumber = TestEnvironment_GenerateRandomNo6Digit();
                try { parentEntityNumber = Convert.ToInt32(model.ParticipantId); } catch (Exception exc) { }
            }

            #region form-dataentry            
            FormDataEntryMongo formDataEntryMongo = new FormDataEntryMongo();
            formDataEntryMongo.ProjectId = project.ProjectId;
            formDataEntryMongo.ProjectGuid = model.ProjectId;
            formDataEntryMongo.ProjectName = project.ProjectName;

            formDataEntryMongo.ActivityId = activity.ActivityId;
            formDataEntryMongo.ActivityGuid = activity.ActivityGuid;
            formDataEntryMongo.ActivityName = activity.ActivityName;

            formDataEntryMongo.FormId = form.FormId;
            formDataEntryMongo.FormGuid = form.FormGuid;
            formDataEntryMongo.FormTitle = form.FormTitle;

            formDataEntryMongo.EntityTypeName = entityType.Name;
            formDataEntryMongo.EntityTypeId = entityType.Id;
            formDataEntryMongo.EntityTypeGuid = entityType.Guid;

            formDataEntryMongo.Status = model.Status;
            formDataEntryMongo.CreatedById = createdBy.Id;
            formDataEntryMongo.CreatedByGuid = createdBy.Guid;
            formDataEntryMongo.CreatedByName = createdBy.FirstName + " " + createdBy.LastName;
            formDataEntryMongo.CreatedDate = DateTime.UtcNow;

            formDataEntryMongo.ThisUserId = model.ThisUserId;
            formDataEntryMongo.ThisUserName = thisUserIdDetails != null ? thisUserIdDetails.FirstName + " " + thisUserIdDetails.LastName : null;
            formDataEntryMongo.ThisUserGuid = thisUserIdDetails != null ? thisUserIdDetails.Guid : (Guid?)null;

            formDataEntryMongo.EntityNumber = entityNumber;
            formDataEntryMongo.ParentEntityNumber = parentEntityNumber;

            formDataEntryMongo.formDataEntryVariableMongoList = new List<FormDataEntryVariableMongo>();
            FormDataEntryVariableMongo formDataEntryVariableMongo = new FormDataEntryVariableMongo();

            var saveParent = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 3);
            var dbEntid = _dbContext.Variables.FirstOrDefault(x => x.VariableName == "EntID");
            formDataEntryVariableMongo.VariableId = saveParent.VariableId;
            formDataEntryVariableMongo.VariableGuid = dbEntid.Guid;
            formDataEntryVariableMongo.VariableName = dbEntid.VariableName;
            formDataEntryVariableMongo.SelectedValues = entityNumber.ToString();
            formDataEntryVariableMongo.CreatedBy = createdBy.Id;
            formDataEntryVariableMongo.CreatedDate = DateTime.UtcNow;
            formDataEntryVariableMongo.ParentId = null;
            formDataEntryMongo.formDataEntryVariableMongoList.Add(formDataEntryVariableMongo);

            foreach (var item in model.FormDataEntryVariable)
            {
                formDataEntryVariableMongo = new FormDataEntryVariableMongo();
                if (item.VariableId == 3)
                {
                    continue;
                }

                var variableData = form.VariablesListMongo.FirstOrDefault(x => x.VariableId == item.VariableId);
                formDataEntryVariableMongo.VariableId = item.VariableId;
                formDataEntryVariableMongo.VariableGuid = variableData.VariableGuid;
                formDataEntryVariableMongo.VariableName = variableData.VariableName;
                formDataEntryVariableMongo.SelectedValues = !string.IsNullOrEmpty(item.SelectedValues) ? item.SelectedValues.Trim() : string.Empty;
                formDataEntryVariableMongo.CreatedBy = createdBy.Id;
                formDataEntryVariableMongo.CreatedDate = DateTime.UtcNow;
                formDataEntryVariableMongo.ParentId = entityNumber;
                if (variableData.VariableTypeName == VariableTypes.FileType.ToString())
                {
                    formDataEntryVariableMongo.FileName = item.FileName;
                }
                formDataEntryMongo.formDataEntryVariableMongoList.Add(formDataEntryVariableMongo);
            }
            #endregion

            #region save in login table
            string fname = string.Empty;
            string lname = string.Empty;

            if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration))
            {
                var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                var f = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                lname = l != null ? l.SelectedValues : string.Empty;
                fname = f != null ? f.SelectedValues : string.Empty;
            }
            else if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration))
            {
                var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                var f = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                var m = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 17);
                lname = l != null ? l.SelectedValues : string.Empty;
                fname = f != null ? f.SelectedValues : string.Empty + " " + m != null ? m.SelectedValues : string.Empty;
            }
            else if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration))
            {
                var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                fname = l != null ? l.SelectedValues : string.Empty;
            }

            if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration))
            {
                var emali = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38).SelectedValues : string.Empty;
                var username = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40).SelectedValues : string.Empty;
                if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration))
                {
                    var EntGrpVar = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 5) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 5).SelectedValues : string.Empty;
                    if (EntGrpVar != "15")
                    {
                        var loginTbl = _dbContext.UserLogins.FirstOrDefault(x => x.Id == formDataEntryMongo.ThisUserId);
                        if (loginTbl != null)
                        {
                            loginTbl.Password = null;
                            loginTbl.Salt = null;
                            loginTbl.SecurityQuestionId = null;
                            loginTbl.Answer = null;
                            _dbContext.SaveChanges();
                        }
                    }
                }
                var authTypeGuid = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51).SelectedValues : new Guid().ToString();

                int at = 0;
                try
                {
                    at = Convert.ToInt32(authTypeGuid);
                }
                catch (Exception dd)
                {
                    at = 1;
                }
                var authTypeId = _dbContext.LoginAuthTypeMasters.Where(x => x.DateDeactivated == null && x.Id == at).Select(x => new { id = x.Guid.ToString(), name = x.AuthTypeName }).FirstOrDefault();

                var authType = _dbContext.LoginAuthTypeMasters.FirstOrDefault(x => x.Guid == new Guid(authTypeId.id));

                var userrole = _dbContext.Roles.FirstOrDefault(x => x.Name == "Data Entry Supervisor");

                UserLoginViewModel entityUserlogin = new UserLoginViewModel();

                entityUserlogin.FirstName = fname;
                entityUserlogin.LastName = lname;
                entityUserlogin.Email = emali;
                entityUserlogin.TenantId = model.TenantId != null ? (Guid)model.TenantId : new Guid();
                entityUserlogin.AuthTypeId = authType != null ? authType.AuthType : 1;
                entityUserlogin.CreatedBy = model.CreatedBy;
                entityUserlogin.UserTypeId = (int)Core.Enum.UsersLoginType.Test;
                entityUserlogin.TempGuid = Guid.NewGuid();
                entityUserlogin.RoleId = userrole != null ? userrole.Guid : new Guid();
                entityUserlogin.UserName = !string.IsNullOrEmpty(username) ? username : entityNumber.ToString();
                entityUserlogin.Status = (int)Core.Enum.Status.InActive;
                entityUserlogin.IsUserApprovedBySystemAdmin = false;
                var savedUser = _userLoginProvider.Create(entityUserlogin);

                if (savedUser != null)
                {
                    formDataEntryMongo.ThisUserId = savedUser != null ? savedUser.Id : (int?)null;
                    formDataEntryMongo.ThisUserGuid = savedUser != null ? savedUser.Guid : (Guid?)null;
                    formDataEntryMongo.ThisUserName = savedUser != null ? savedUser.FirstName + " " + savedUser.LastName : null;
                }
            }
            #endregion

            if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Registration))
            {
                #region linkage from summary page   
                UserLogin activityCompletedBy = null;
                if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration)
                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration))
                {
                    activityCompletedBy = new UserLogin()
                    {
                        Id = createdBy.Id,
                        FirstName = createdBy.FirstName,
                        LastName = createdBy.LastName,
                        Email = createdBy.Email,
                        IsUserApprovedBySystemAdmin = createdBy.IsUserApprovedBySystemAdmin,
                        Status = createdBy.Status,
                        AuthTypeId = createdBy.AuthTypeId,
                        UserTypeId = createdBy.UserTypeId,
                        UserName = createdBy.UserName,
                        Guid = createdBy.Guid,
                        IsApiAccessEnabled = createdBy.IsApiAccessEnabled,
                    };
                }
                else
                {
                    activityCompletedBy = _dbContext.UserLogins.FirstOrDefault(x => x.UserName.ToLower() == "testsystemadmin@aspree.com");
                }

                SummaryPageActivityViewModel summaryPageActivityViewModel = new SummaryPageActivityViewModel();
                summaryPageActivityViewModel.ActivityId = activity.ActivityId;
                summaryPageActivityViewModel.ActivityGuid = activity.ActivityGuid;
                summaryPageActivityViewModel.ActivityName = activity.ActivityName;
                summaryPageActivityViewModel.ActivityCompletedById = activityCompletedBy.Id;
                summaryPageActivityViewModel.ActivityCompletedByGuid = activityCompletedBy.Guid;
                summaryPageActivityViewModel.ActivityCompletedByName = activityCompletedBy.FirstName + " " + activityCompletedBy.LastName;
                summaryPageActivityViewModel.ActivityDate = DateTime.UtcNow.Date;

                summaryPageActivityViewModel.ProjectGuid = project.ProjectGuid;
                summaryPageActivityViewModel.ProjectName = project.ProjectName;
                summaryPageActivityViewModel.PersonEntityId = entityNumber;
                summaryPageActivityViewModel.CreatedByName = createdBy.FirstName + " " + createdBy.LastName;
                summaryPageActivityViewModel.CreatedDate = DateTime.UtcNow;
                summaryPageActivityViewModel.ProjectVersion = project.ProjectInternalVersion;

                summaryPageActivityViewModel.SummaryPageActivityFormsList = new List<SummaryPageActivityForms>();
                summaryPageActivityViewModel.SummaryPageActivityFormsList.Add(new SummaryPageActivityForms()
                {
                    FormId = form.FormId,
                    FormGuid = form.FormGuid,
                    FormTitle = form.FormTitle,
                    FormStatusId = (int)Core.Enum.FormStatusTypes.Draft,
                    FormStatusName = Enum.GetName(typeof(Core.Enum.FormStatusTypes), 3),
                });
                var summaryPageActivity = _testMongoDBContext._database.GetCollection<BsonDocument>("SummaryPageActivity");
                var summaryPageActivityResult = summaryPageActivity.Insert(summaryPageActivityViewModel);

                formDataEntryMongo.SummaryPageActivityObjId = Convert.ToString(summaryPageActivityViewModel.Id);
                #endregion
            }
            else
            {
                formDataEntryMongo.SummaryPageActivityObjId = model.SummaryPageActivityObjId;
            }


            if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Linkage))
            {
                #region assig project to user
                var projectLinkageCondition = Query<FormDataEntryMongo>.EQ(q => q.EntityNumber, parentEntityNumber);
                var projectLinkageConditionEntity = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(projectLinkageCondition).AsQueryable().FirstOrDefault();
                if (projectLinkageConditionEntity != null)
                {
                    UserLogin uLogin = _dbContext.UserLogins.FirstOrDefault(x => x.Id == projectLinkageConditionEntity.ThisUserId);
                    if (uLogin != null)
                    {
                        var proRole = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 56);//ProRole variable of variable table
                        Role urole = new Role();
                        if (proRole != null)
                        {
                            Guid role = !string.IsNullOrEmpty(proRole.SelectedValues) ? new Guid(proRole.SelectedValues) : Guid.Empty;
                            urole = _dbContext.Roles.FirstOrDefault(x => x.Guid == role);
                        }

                        var linkedProjectId = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 52);  //LnkPro(Linked Project) variable
                        Guid linkedProjectGuid = Guid.Parse(linkedProjectId.SelectedValues);


                        #region Set projectLinkage end date
                        DateTime? projectJoinDate = null;
                        try
                        {
                            var projJoindDate = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Join);
                            string jDate = projJoindDate != null ? projJoindDate.SelectedValues : string.Empty;
                            projectJoinDate = Convert.ToDateTime(jDate);
                        }
                        catch (Exception joinDete) { }
                        try
                        {

                            var projectLeftDateActiveUser = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Actv);
                            if (projectLeftDateActiveUser != null)
                            {
                                formDataEntryMongo.ProjectLinkage_IsActiveProjectUser = !string.IsNullOrEmpty(projectLeftDateActiveUser.SelectedValues) ? (projectLeftDateActiveUser.SelectedValues == "1" ? true : false) : false;
                                if (projectLeftDateActiveUser.SelectedValues == "0")
                                {
                                    var projectLeftDateEnd = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.End);
                                    string edate = projectLeftDateEnd != null ? projectLeftDateEnd.SelectedValues : string.Empty;
                                    formDataEntryMongo.ProjectLinkage_ProjectLeftDate = Convert.ToDateTime(edate);
                                }
                            }
                        }
                        catch (Exception exLinkage) { }
                        if (urole == null)
                        {
                            formDataEntryMongo.ProjectLinkage_IsActiveProjectUser = false;
                        }
                        #endregion


                        var conditionProjStaff = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, linkedProjectGuid);
                        var conditionProjStaffResult = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(conditionProjStaff).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();

                        if (conditionProjStaffResult != null)
                        {

                            formDataEntryMongo.ProjectLinkage_LinkedProjectId = conditionProjStaffResult.ProjectGuid;
                            if (!conditionProjStaffResult.ProjectStaffListMongo.Any(x => x.StaffGuid == uLogin.Guid && x.Role == urole.Name))
                            {
                                conditionProjStaffResult.ProjectStaffListMongo.Add(new ProjectStaffMongo()
                                {
                                    Role = urole != null ? urole.Name : string.Empty,
                                    StaffName = uLogin != null ? uLogin.FirstName + " " + uLogin.LastName : string.Empty,
                                    StaffGuid = uLogin != null ? uLogin.Guid : Guid.Empty,

                                    IsActiveProjectUser = formDataEntryMongo.ProjectLinkage_IsActiveProjectUser,
                                    ProjectJoinedDate = projectJoinDate,
                                    ProjectLeftDate = formDataEntryMongo.ProjectLinkage_ProjectLeftDate,
                                });


                                ////Mongo Query    lok-----------------------------------------------------
                                var updateProjectObjectId = Query<ProjectDeployViewModel>.EQ(p => p.Id, conditionProjStaffResult.Id);
                                // Document Collections  
                                var updateProjectCollection = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects");
                                // Document Update which need Id and Data to Update  
                                var isUpdateProj = updateProjectCollection.Update(updateProjectObjectId, MongoDB.Driver.Builders.Update.Replace(conditionProjStaffResult), MongoDB.Driver.UpdateFlags.None);


                                #region assigne project to user in login table
                                var projectstaffRolesSQL = new ProjectStaffMemberRole()
                                {
                                    Guid = Guid.NewGuid(),
                                    ProjectId = conditionProjStaffResult != null ? conditionProjStaffResult.ProjectId : 1,
                                    UserId = uLogin.Id,
                                    RoleId = urole != null ? urole.Id : (int)Core.Enum.RoleTypes.Data_Entry,
                                    CreatedBy = createdBy.Id,
                                    StaffCreatedDate = DateTime.UtcNow,

                                    IsActiveProjectUser = formDataEntryMongo.ProjectLinkage_IsActiveProjectUser,
                                    ProjectJoinedDate = projectJoinDate,
                                    ProjectLeftDate = formDataEntryMongo.ProjectLinkage_ProjectLeftDate,
                                };
                                _dbContext.ProjectStaffMemberRoles.Add(projectstaffRolesSQL);
                                SaveChanges();
                                #endregion
                            }
                        }
                        else
                        {
                            #region assigne project to user in login table
                            var projId = _dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == linkedProjectGuid);

                            IEnumerable<ProjectStaffMemberRole> projectStaffMemberRoles = this._dbContext.ProjectStaffMemberRoles.Where(x => x.UserId == uLogin.Id && x.FormDataEntry.Guid == linkedProjectGuid);
                            _dbContext.ProjectStaffMemberRoles.RemoveRange(projectStaffMemberRoles);
                            SaveChanges();
                            var projectstaffRolesSQL = new ProjectStaffMemberRole()
                            {
                                Guid = Guid.NewGuid(),
                                ProjectId = projId != null ? projId.Id : 1,
                                UserId = uLogin.Id,
                                RoleId = urole != null ? urole.Id : (int)Core.Enum.RoleTypes.Data_Entry,
                                CreatedBy = createdBy.Id,
                                StaffCreatedDate = DateTime.UtcNow,

                                IsActiveProjectUser = formDataEntryMongo.ProjectLinkage_IsActiveProjectUser,
                                ProjectJoinedDate = projectJoinDate,
                                ProjectLeftDate = formDataEntryMongo.ProjectLinkage_ProjectLeftDate,
                            };
                            _dbContext.ProjectStaffMemberRoles.Add(projectstaffRolesSQL);
                            SaveChanges();
                            #endregion
                        }
                    }
                }
                #endregion
            }

            if (model.Status == (int)Core.Enum.FormStatusTypes.Published || model.Status == (int)Core.Enum.FormStatusTypes.Draft || model.Status == (int)Core.Enum.FormStatusTypes.Submitted_for_review)
            {
                #region Update status of summary page form

                var updateSummaryPageActivity_condition = Query<SummaryPageActivityViewModel>.EQ(q => q.Id, new ObjectId(formDataEntryMongo.SummaryPageActivityObjId));
                var summaryPageActivity = _testMongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").Find(updateSummaryPageActivity_condition).AsQueryable().FirstOrDefault();

                if (summaryPageActivity != null)
                {
                    List<SummaryPageActivityForms> newForms = new List<SummaryPageActivityForms>();
                    foreach (var frm in summaryPageActivity.SummaryPageActivityFormsList)
                    {
                        if (isRegistrationForm)
                        {
                            if (frm.FormTitle == isDefaultForm.FormTitle)
                            {
                                frm.FormStatusId = model.Status;
                                frm.FormStatusName = Enum.GetName(typeof(Core.Enum.FormStatusTypes), model.Status);
                                newForms.Add(frm);
                            }
                            else
                            {
                                newForms.Add(frm);
                            }
                        }
                        else
                        {
                            if (frm.FormGuid == form.FormGuid)
                            {
                                frm.FormStatusId = model.Status;
                                frm.FormStatusName = Enum.GetName(typeof(Core.Enum.FormStatusTypes), model.Status);

                                newForms.Add(frm);
                            }
                            else
                            {
                                newForms.Add(frm);
                            }
                        }
                    }

                    var document_SummaryPageActivity = _testMongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity");
                    var detailscount_SummaryPageActivity = document_SummaryPageActivity.FindAs<SummaryPageActivityViewModel>(Query.EQ("_id", summaryPageActivity.Id)).Count();
                    if (detailscount_SummaryPageActivity > 0)
                    {
                        var qry_summaryPageActivity = Query<SummaryPageActivityViewModel>.EQ(p => p.Id, summaryPageActivity.Id);

                        var summaryPageActivityResult = _testMongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").FindOne(qry_summaryPageActivity);
                        summaryPageActivityResult.SummaryPageActivityFormsList = newForms;

                        var collection = _testMongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity");
                        try { var result1 = collection.Update(qry_summaryPageActivity, Update.Replace(summaryPageActivityResult), UpdateFlags.None); }
                        catch (Exception e) { }
                    }
                }
                #endregion
            }
            FormDataEntryViewModel mdl = new FormDataEntryViewModel();
            mdl.ParticipantId = Convert.ToString(formDataEntryMongo.EntityNumber);
            formDataEntryMongo.ProjectVersion = project.ProjectInternalVersion;
            var document = _testMongoDBContext._database.GetCollection<BsonDocument>("UserEntities");
            var result = document.Insert(formDataEntryMongo);
            return mdl;
        }
        public FormDataEntryViewModel UpdateSearchForm(string objId, FormDataEntryViewModel model)
        {
            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);

            var condition = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, model.ProjectId);
            var project = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(condition).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();

            Form isDefaultForm = _dbContext.Forms.FirstOrDefault(x => x.Guid == model.FormId);

            bool isRegistrationForm = isDefaultForm != null ? (isDefaultForm.IsDefaultForm == (int)Core.Enum.DefaultFormType.Default ? true : false) : false;

            var activity = new ActivitiesMongo();
            var form = new FormsMongo();

            if (isRegistrationForm)
            {
                activity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityName == isDefaultForm.FormTitle);
                form = activity.FormsListMongo.FirstOrDefault(x => x.FormTitle == isDefaultForm.FormTitle);
            }
            else
            {
                activity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == model.ActivityId);
                if (activity == null)
                {
                    var currentProject = _dbContext.Forms.FirstOrDefault(c => c.Guid == model.FormId);
                    if (currentProject != null)
                    {
                        condition = Query<ProjectDeployViewModel>.EQ(q => q.ProjectId, currentProject.ProjectId);
                        project = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(condition).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                        activity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == model.ActivityId);
                    }
                }
                form = activity.FormsListMongo.FirstOrDefault(x => x.FormGuid == model.FormId);
            }

            var userEntitiesDocument = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities");

            var userDetailscount = userEntitiesDocument.FindAs<FormDataEntryMongo>(Query.EQ("_id", new ObjectId(objId))).Count();

            if (userDetailscount > 0)
            {
                var userObjectid = Query<FormDataEntryMongo>.EQ(p => p.Id, new ObjectId(objId));

                var userDetail = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindOne(userObjectid);

                var parent = userDetail.formDataEntryVariableMongoList.Where(x => x.VariableName == "EntID").FirstOrDefault();


                var new_userDetail_formDataEntryVariableMongoList = new List<FormDataEntryVariableMongo>();
                FormDataEntryVariableMongo formDataEntryVariableMongo = new FormDataEntryVariableMongo();

                formDataEntryVariableMongo.VariableId = parent.VariableId;
                formDataEntryVariableMongo.VariableGuid = parent.VariableGuid;
                formDataEntryVariableMongo.VariableName = parent.VariableName;
                formDataEntryVariableMongo.SelectedValues = parent.SelectedValues;
                formDataEntryVariableMongo.CreatedBy = createdBy.Id;
                formDataEntryVariableMongo.CreatedDate = DateTime.UtcNow;
                formDataEntryVariableMongo.ParentId = parent.ParentId;

                new_userDetail_formDataEntryVariableMongoList.Add(formDataEntryVariableMongo);

                foreach (var item in model.FormDataEntryVariable)
                {
                    formDataEntryVariableMongo = new FormDataEntryVariableMongo();
                    if (item.VariableId == 3)
                    {
                        continue;
                    }

                    var variableData = form.VariablesListMongo.FirstOrDefault(x => x.VariableId == item.VariableId);

                    formDataEntryVariableMongo.VariableId = item.VariableId;
                    formDataEntryVariableMongo.VariableGuid = variableData.VariableGuid;
                    formDataEntryVariableMongo.VariableName = variableData.VariableName;
                    formDataEntryVariableMongo.SelectedValues = !string.IsNullOrEmpty(item.SelectedValues) ? item.SelectedValues.Trim() : string.Empty;
                    formDataEntryVariableMongo.CreatedBy = createdBy.Id;
                    formDataEntryVariableMongo.CreatedDate = DateTime.UtcNow;
                    formDataEntryVariableMongo.ParentId = parent.SelectedValues != null ? Convert.ToInt32(parent.SelectedValues) : (int?)null;

                    if (variableData.VariableTypeName == VariableTypes.FileType.ToString())
                    {
                        formDataEntryVariableMongo.FileName = item.FileName;
                    }


                    new_userDetail_formDataEntryVariableMongoList.Add(formDataEntryVariableMongo);
                }

                #region Update project info(logo, color, display name) SQL DB.
                Guid FormDataEntryId = Guid.Empty;
                if (userDetail.FormTitle == "Project Registration")
                {
                    try
                    {
                        var saveParent_SQL = _dbContext.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == "EntID" && x.SelectedValues == parent.SelectedValues);

                        try { FormDataEntryId = saveParent_SQL.FormDataEntry.Guid; } catch (Exception eFormDataEntryId) { FormDataEntryId = Guid.Empty; }

                        IEnumerable<FormDataEntryVariable> formDataVariablesId = this._dbContext.FormDataEntryVariables.Where(x => x.FormDataEntryId == saveParent_SQL.FormDataEntryId);
                        _dbContext.FormDataEntryVariables.RemoveRange(formDataVariablesId);

                        var parentFormDataEntryVariable = new FormDataEntryVariable()
                        {
                            Guid = Guid.NewGuid(),
                            VariableId = saveParent_SQL.VariableId,
                            SelectedValues = saveParent_SQL.SelectedValues,
                            SelectedValues_int = saveParent_SQL.SelectedValues_int,
                            SelectedValues_float = saveParent_SQL.SelectedValues_float,
                            FormDataEntryId = saveParent_SQL.FormDataEntryId,
                            CreatedBy = createdBy.Id,
                            CreatedDate = DateTime.UtcNow,
                        };
                        _dbContext.FormDataEntryVariables.Add(parentFormDataEntryVariable);
                        SaveChanges();
                        foreach (var item in model.FormDataEntryVariable)
                        {
                            //check entity id to skip
                            if (item.VariableId == 3) { continue; }
                            var formDataEntryVariable = new FormDataEntryVariable()
                            {
                                Guid = Guid.NewGuid(),
                                VariableId = item.VariableId,
                                SelectedValues = item.SelectedValues,
                                SelectedValues_int = item.SelectedValues_int,
                                SelectedValues_float = item.SelectedValues_float,
                                FormDataEntryId = saveParent_SQL.FormDataEntryId,
                                CreatedBy = createdBy.Id,
                                CreatedDate = DateTime.UtcNow,
                                ParentId = parentFormDataEntryVariable.Id,
                            };
                            _dbContext.FormDataEntryVariables.Add(formDataEntryVariable);
                        }
                    }
                    catch (Exception exc) { }
                }
                #endregion

                userDetail.formDataEntryVariableMongoList = new_userDetail_formDataEntryVariableMongoList;

                #region save in login table
                string fname = string.Empty;
                string lname = string.Empty;
                string personRole = string.Empty;
                if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration))
                {
                    var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                    var f = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                    lname = l != null ? l.SelectedValues : string.Empty;
                    fname = f != null ? f.SelectedValues : string.Empty;
                }
                else if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration))
                {
                    var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                    var f = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                    var m = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 17);
                    lname = l != null ? l.SelectedValues : string.Empty;
                    fname = f != null ? f.SelectedValues : string.Empty + " " + m != null ? m.SelectedValues : string.Empty;
                }
                else if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration))
                {
                    var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                    fname = l != null ? l.SelectedValues : string.Empty;
                }
                personRole = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 43) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 43).SelectedValues : string.Empty;  //SysRole


                if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                    || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                    || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration))
                {
                    var emali = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38).SelectedValues : string.Empty;
                    var username = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40).SelectedValues : string.Empty;

                    var authTypeGuid = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51).SelectedValues : new Guid().ToString();
                    if (!string.IsNullOrEmpty(authTypeGuid) && !string.IsNullOrEmpty(username))
                    {
                        LoginAuthTypeMaster authType = null;
                        Guid at = Guid.Empty;
                        try
                        {
                            at = new Guid(authTypeGuid);
                            var authTypeId = _dbContext.LoginAuthTypeMasters.Where(x => x.DateDeactivated == null && x.Guid == at).Select(x => new { id = x.Guid.ToString(), name = x.AuthTypeName }).FirstOrDefault();
                            authType = _dbContext.LoginAuthTypeMasters.FirstOrDefault(x => x.Guid == new Guid(authTypeId.id));
                        }
                        catch (Exception dd)
                        {
                            var authTypeId = _dbContext.LoginAuthTypeMasters.Where(x => x.DateDeactivated == null && x.Id == 1).Select(x => new { id = x.Guid.ToString(), name = x.AuthTypeName }).FirstOrDefault();
                            authType = _dbContext.LoginAuthTypeMasters.FirstOrDefault(x => x.Guid == new Guid(authTypeId.id));
                        }

                        var logintable = _dbContext.UserLogins.FirstOrDefault(x => x.Id == userDetail.ThisUserId);



                        Role userrole = null;

                        int roletypeid = !string.IsNullOrEmpty(personRole) ? Convert.ToInt32(personRole) : 0;
                        if (roletypeid == 1)
                            userrole = _dbContext.Roles.FirstOrDefault(x => x.Id == (int)Core.Enum.RoleTypes.System_Admin);
                        else
                            userrole = _dbContext.Roles.FirstOrDefault(x => x.Id == (int)Core.Enum.RoleTypes.Data_Entry);


                        #region Is API Access to export data
                        bool isApiAccess = false;
                        try
                        {
                            if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                                || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration))
                            {
                                Variable authVariable = _dbContext.Variables.FirstOrDefault(x => x.VariableName == "AuthenticationMethod");
                                Variable isApiAccessEnabled = _dbContext.Variables.FirstOrDefault(x => x.VariableName == "ApiAccessEnabled");
                                LoginAuthTypeMaster loginAuthTypeMaster = _dbContext.LoginAuthTypeMasters.FirstOrDefault(x => x.AuthTypeName.ToLower() == Core.Enum.AuthenticationTypes.Local_Password.ToString().ToLower().Replace("_", " "));
                                int authVariableId = authVariable != null ? authVariable.Id : 0;

                                if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration))
                                {
                                    var isLocalPassword = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == authVariableId);
                                    string selectedVal = isLocalPassword != null ? isLocalPassword.SelectedValues : string.Empty;
                                    string localPWGuid = loginAuthTypeMaster != null ? Convert.ToString(loginAuthTypeMaster.Guid) : Convert.ToString(Guid.Empty);

                                    int isApiAccessEnabledId = isApiAccessEnabled != null ? isApiAccessEnabled.Id : 0;


                                    if (selectedVal == localPWGuid)
                                    {
                                        var isApiAccessEnabledData = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == isApiAccessEnabledId);
                                        isApiAccess = isApiAccessEnabledData != null ? (isApiAccessEnabledData.SelectedValues == "1" ? true : false) : false;
                                    }
                                }
                                else if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration))
                                {
                                    var variableEntType = _dbContext.Variables.FirstOrDefault(x => x.VariableName == "EntType");
                                    var entType = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == variableEntType.Id);
                                    string entTypeId = entType != null ? entType.SelectedValues : string.Empty;
                                    if (entTypeId == "15")
                                    {
                                        var isLocalPassword = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == authVariableId);
                                        string selectedVal = isLocalPassword != null ? isLocalPassword.SelectedValues : string.Empty;
                                        string localPWGuid = loginAuthTypeMaster != null ? Convert.ToString(loginAuthTypeMaster.Guid) : Convert.ToString(Guid.Empty);

                                        int isApiAccessEnabledId = isApiAccessEnabled != null ? isApiAccessEnabled.Id : 0;


                                        if (selectedVal == localPWGuid)
                                        {
                                            var isApiAccessEnabledData = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == isApiAccessEnabledId);
                                            isApiAccess = isApiAccessEnabledData != null ? (isApiAccessEnabledData.SelectedValues == "1" ? true : false) : false;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception exc) { Console.WriteLine(exc); }
                        #endregion


                        UserLoginViewModel entityUserlogin = new UserLoginViewModel();

                        FormDataEntryVariableViewModel userStatusModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 42);
                        int userStatus = 0;
                        if (userStatusModel != null)
                        {
                            userStatus = !string.IsNullOrEmpty(userStatusModel.SelectedValues) ? Convert.ToInt32(userStatusModel.SelectedValues) : 0;
                        }


                        #region is user approved by system admin
                        bool isUserApprovedBySystemAdmin = false;
                        FormDataEntryVariableViewModel isUserApprovedBySystemAdminModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)Core.Enum.DefaultsVariables.SysAppr);
                        if (isUserApprovedBySystemAdminModel != null)
                        {
                            int isUserApprovedBySystemAdminInt = !string.IsNullOrEmpty(isUserApprovedBySystemAdminModel.SelectedValues) ? Convert.ToInt32(isUserApprovedBySystemAdminModel.SelectedValues) : 0;
                            isUserApprovedBySystemAdmin = isUserApprovedBySystemAdminInt == 1 ? true : false;
                        }
                        else if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration))
                        {
                            //Approved by System Admin field is not available into "Participant Registration" form
                            try
                            {
                                isUserApprovedBySystemAdminModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Active);
                                int isUserApprovedBySystemAdminInt = !string.IsNullOrEmpty(isUserApprovedBySystemAdminModel.SelectedValues) ? Convert.ToInt32(isUserApprovedBySystemAdminModel.SelectedValues) : 0;
                                isUserApprovedBySystemAdmin = isUserApprovedBySystemAdminInt == 1 ? true : false; ;
                            }
                            catch (Exception excActive) { Console.WriteLine(excActive.Message); }
                        }
                        #endregion

                        if (userStatus == (int)Core.Enum.Status.Active)
                        {
                            userStatus = (int)Core.Enum.Status.Active;
                        }
                        else if (logintable.Status == (int)Core.Enum.Status.Locked)
                        {
                            userStatus = (int)Core.Enum.Status.Locked;
                        }
                        else
                        {
                            userStatus = (int)Core.Enum.Status.InActive;
                        }
                        entityUserlogin.FirstName = fname;
                        entityUserlogin.LastName = lname;
                        entityUserlogin.Email = emali;
                        entityUserlogin.UserName = username;
                        entityUserlogin.Mobile = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 39) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 39).SelectedValues : null;
                        entityUserlogin.TenantId = model.TenantId != null ? (Guid)model.TenantId : new Guid();
                        entityUserlogin.AuthTypeId = authType != null ? authType.Id : 1;
                        entityUserlogin.UserTypeId = (int)Core.Enum.UsersLoginType.Entity;
                        entityUserlogin.TempGuid = Guid.NewGuid();
                        entityUserlogin.RoleId = userrole != null ? userrole.Guid : new Guid();
                        entityUserlogin.ModifiedBy = model.CreatedBy;
                        entityUserlogin.Guid = logintable.Guid;
                        entityUserlogin.Id = logintable.Id;
                        entityUserlogin.IsApiAccessEnabled = isApiAccess;
                        entityUserlogin.Status = userStatus;
                        entityUserlogin.IsUserApprovedBySystemAdmin = isUserApprovedBySystemAdmin;
                        _userLoginProvider.Update(entityUserlogin);

                        var login = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == logintable.Guid);
                        if (login != null)
                        {
                            login.AuthTypeId = authType != null ? authType.Id : 0;
                            _dbContext.SaveChanges();
                        }
                    }

                    if (form.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
                    {
                        #region place/group form login update
                        var EntGrpVar = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 5) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 5).SelectedValues : string.Empty;
                        if (EntGrpVar != "15")
                        {
                            var loginTbl = _dbContext.UserLogins.FirstOrDefault(x => x.Id == userDetail.ThisUserId);
                            if (loginTbl != null)
                            {
                                loginTbl.Password = null;
                                loginTbl.Salt = null;
                                loginTbl.SecurityQuestionId = null;
                                loginTbl.Answer = null;
                                loginTbl.Status = (int)Core.Enum.Status.InActive;
                                loginTbl.IsUserApprovedBySystemAdmin = false;

                                loginTbl.LoginFailedAttemptCount = null;
                                loginTbl.LoginFailedAttemptDate = null;

                                _dbContext.SaveChanges();
                            }
                        }
                        else
                        {
                            #region place form login update
                            //Approved by System Admin field is not available into "Participant Registration" form
                            try
                            {
                                FormDataEntryVariableViewModel isUserApprovedBySystemAdminModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Active);
                                int isUserApprovedBySystemAdminInt = !string.IsNullOrEmpty(isUserApprovedBySystemAdminModel.SelectedValues) ? Convert.ToInt32(isUserApprovedBySystemAdminModel.SelectedValues) : 0;
                                int userLoginStatus = (int)Core.Enum.Status.InActive;
                                bool isUserApprovedBySystemAdmin = isUserApprovedBySystemAdminInt == 1 ? true : false;

                                var loginTbl = _dbContext.UserLogins.FirstOrDefault(x => x.Id == userDetail.ThisUserId);

                                FormDataEntryVariableViewModel isActiveUserModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)Core.Enum.DefaultsVariables.Active);
                                if (isActiveUserModel != null)
                                {
                                    userLoginStatus = !string.IsNullOrEmpty(isActiveUserModel.SelectedValues) ? Convert.ToInt32(isActiveUserModel.SelectedValues) : 0;
                                    if (userLoginStatus == (int)Core.Enum.Status.Active)
                                    {
                                        userLoginStatus = (int)Core.Enum.Status.Active;
                                    }
                                    else if (loginTbl != null && loginTbl.Status == (int)Core.Enum.Status.Locked)
                                    {
                                        userLoginStatus = (int)Core.Enum.Status.Locked;
                                    }
                                    else
                                    {
                                        userLoginStatus = (int)Core.Enum.Status.InActive;
                                    }

                                }


                                if (loginTbl != null)
                                {
                                    loginTbl.Status = userLoginStatus;
                                    loginTbl.IsUserApprovedBySystemAdmin = isUserApprovedBySystemAdmin;

                                    if (userLoginStatus == (int)Core.Enum.Status.Active)
                                    {
                                        loginTbl.LoginFailedAttemptCount = null;
                                        loginTbl.LoginFailedAttemptDate = null;
                                    }
                                    _dbContext.SaveChanges();
                                }
                            }
                            catch (Exception excActive) { Console.WriteLine(excActive.Message); }
                            #endregion
                        }
                        #endregion
                    }
                    else if (form.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration))
                    {
                        #region person reg form login update
                        bool isUserApprovedBySystemAdmin = false;
                        int userLoginStatus = (int)Core.Enum.Status.InActive;
                        FormDataEntryVariableViewModel isUserApprovedBySystemAdminModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)Core.Enum.DefaultsVariables.SysAppr);
                        if (isUserApprovedBySystemAdminModel != null)
                        {
                            int isUserApprovedBySystemAdminInt = !string.IsNullOrEmpty(isUserApprovedBySystemAdminModel.SelectedValues) ? Convert.ToInt32(isUserApprovedBySystemAdminModel.SelectedValues) : 0;
                            isUserApprovedBySystemAdmin = isUserApprovedBySystemAdminInt == 1 ? true : false;
                        }
                        FormDataEntryVariableViewModel isActiveUserModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)Core.Enum.DefaultsVariables.Active);
                        var loginTbl = _dbContext.UserLogins.FirstOrDefault(x => x.Id == userDetail.ThisUserId);
                        if (isActiveUserModel != null)
                        {
                            userLoginStatus = !string.IsNullOrEmpty(isActiveUserModel.SelectedValues) ? Convert.ToInt32(isActiveUserModel.SelectedValues) : 0;
                            if (userLoginStatus == (int)Core.Enum.Status.Active)
                            {
                                userLoginStatus = (int)Core.Enum.Status.Active;
                            }
                            else if (loginTbl != null && loginTbl.Status == (int)Core.Enum.Status.Locked)
                            {
                                userLoginStatus = (int)Core.Enum.Status.Locked;
                            }
                            else
                            {
                                userLoginStatus = (int)Core.Enum.Status.InActive;
                            }
                        }


                        if (loginTbl != null)
                        {
                            loginTbl.Status = userLoginStatus;
                            loginTbl.IsUserApprovedBySystemAdmin = isUserApprovedBySystemAdmin;

                            if (userLoginStatus == (int)Core.Enum.Status.Active)
                            {
                                loginTbl.LoginFailedAttemptCount = null;
                                loginTbl.LoginFailedAttemptDate = null;
                            }

                            _dbContext.SaveChanges();
                        }
                        #endregion
                    }
                    else if (form.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
                    {
                        #region participant form login update
                        //Approved by System Admin field is not available into "Participant Registration" form
                        try
                        {
                            var loginTbl = _dbContext.UserLogins.FirstOrDefault(x => x.Id == userDetail.ThisUserId);
                            FormDataEntryVariableViewModel isUserApprovedBySystemAdminModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Active);
                            int isUserApprovedBySystemAdminInt = !string.IsNullOrEmpty(isUserApprovedBySystemAdminModel.SelectedValues) ? Convert.ToInt32(isUserApprovedBySystemAdminModel.SelectedValues) : 0;

                            if (isUserApprovedBySystemAdminInt == (int)Core.Enum.Status.Active)
                            {
                                isUserApprovedBySystemAdminInt = (int)Core.Enum.Status.Active;
                            }
                            else if (loginTbl != null && loginTbl.Status == (int)Core.Enum.Status.Locked)
                            {
                                isUserApprovedBySystemAdminInt = (int)Core.Enum.Status.Locked;
                            }
                            else
                            {
                                isUserApprovedBySystemAdminInt = (int)Core.Enum.Status.InActive;
                            }

                            bool isUserApprovedBySystemAdmin = isUserApprovedBySystemAdminInt == 1 ? true : false;

                            if (loginTbl != null)
                            {
                                loginTbl.Status = isUserApprovedBySystemAdminInt;
                                loginTbl.IsUserApprovedBySystemAdmin = isUserApprovedBySystemAdmin;
                                if (isUserApprovedBySystemAdminInt == (int)Core.Enum.Status.Active)
                                {
                                    loginTbl.LoginFailedAttemptCount = null;
                                    loginTbl.LoginFailedAttemptDate = null;
                                }
                                _dbContext.SaveChanges();
                            }
                        }
                        catch (Exception excActive) { Console.WriteLine(excActive.Message); }
                        #endregion
                    }
                    #endregion
                }

                if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Linkage))
                {
                    DateTime? projectJoinDate = null;
                    try
                    {
                        var projJoindDate = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Join);
                        string jDate = projJoindDate != null ? projJoindDate.SelectedValues : string.Empty;
                        projectJoinDate = Convert.ToDateTime(jDate);
                    }
                    catch (Exception joinDete)
                    { }

                    #region assig project to user
                    var projectLinkageCondition = Query<FormDataEntryMongo>.EQ(q => q.EntityNumber, userDetail.ParentEntityNumber);
                    var projectLinkageConditionEntity = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(projectLinkageCondition).AsQueryable().FirstOrDefault();
                    if (projectLinkageConditionEntity != null)
                    {
                        UserLogin uLogin = _dbContext.UserLogins.FirstOrDefault(x => x.Id == projectLinkageConditionEntity.ThisUserId);
                        if (uLogin != null)
                        {
                            var proRole = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 56);//ProRole variable of variable table
                            Role urole = new Role();
                            if (proRole != null)
                            {
                                Guid role = !string.IsNullOrEmpty(proRole.SelectedValues) ? new Guid(proRole.SelectedValues) : Guid.Empty;
                                urole = _dbContext.Roles.FirstOrDefault(x => x.Guid == role);
                            }

                            var linkedProjectId = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 52);  //LnkPro(Linked Project) variable
                            Guid linkedProjectGuid = Guid.Parse(linkedProjectId.SelectedValues);

                            var conditionProjStaff = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, linkedProjectGuid);
                            var conditionProjStaffResult = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(conditionProjStaff).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();

                            if (conditionProjStaffResult != null)
                            {
                                if (conditionProjStaffResult.ProjectStaffListMongo.Any(x => x.StaffGuid == uLogin.Guid))
                                {
                                    conditionProjStaffResult.ProjectStaffListMongo.Remove(conditionProjStaffResult.ProjectStaffListMongo.FirstOrDefault(x => x.StaffGuid == uLogin.Guid));
                                }
                                conditionProjStaffResult.ProjectStaffListMongo.Add(new ProjectStaffMongo()
                                {
                                    Role = urole != null ? urole.Name : string.Empty,
                                    StaffName = uLogin != null ? uLogin.FirstName + " " + uLogin.LastName : string.Empty,
                                    StaffGuid = uLogin != null ? uLogin.Guid : Guid.Empty,
                                });

                                ////Mongo Query    lok-----------------------------------------------------
                                var updateProjectObjectId = Query<ProjectDeployViewModel>.EQ(p => p.Id, conditionProjStaffResult.Id);
                                // Document Collections  
                                var updateProjectCollection = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects");
                                // Document Update which need Id and Data to Update  
                                try
                                {
                                    //conditionProjStaffResult.Id = new ObjectId();
                                    var isUpdateProj = updateProjectCollection.Update(updateProjectObjectId, MongoDB.Driver.Builders.Update.Replace(conditionProjStaffResult), MongoDB.Driver.UpdateFlags.None);
                                }
                                catch (Exception e) { }

                                #region Set projectLinkage end date
                                try
                                {
                                    userDetail.ProjectLinkage_LinkedProjectId = conditionProjStaffResult.ProjectGuid;
                                    userDetail.ThisUserId = uLogin.Id;
                                    userDetail.ThisUserGuid = uLogin.Guid;
                                    userDetail.ThisUserName = uLogin.FirstName + " " + uLogin.LastName;

                                    var projectLeftDateActiveUser = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Actv);
                                    if (projectLeftDateActiveUser != null)
                                    {
                                        userDetail.ProjectLinkage_IsActiveProjectUser = !string.IsNullOrEmpty(projectLeftDateActiveUser.SelectedValues) ? (projectLeftDateActiveUser.SelectedValues == "1" ? true : false) : false;
                                        if (projectLeftDateActiveUser.SelectedValues == "0")
                                        {
                                            var projectLeftDateEnd = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.End);
                                            string edate = projectLeftDateEnd != null ? projectLeftDateEnd.SelectedValues : string.Empty;
                                            userDetail.ProjectLinkage_ProjectLeftDate = Convert.ToDateTime(edate);
                                        }
                                    }
                                }
                                catch (Exception exLinkage) { }
                                #endregion

                                #region assigne project to user in login table
                                IEnumerable<ProjectStaffMemberRole> projectStaffMemberRoles = this._dbContext.ProjectStaffMemberRoles.Where(x => x.UserId == uLogin.Id && x.FormDataEntry.Guid == conditionProjStaffResult.ProjectGuid);
                                _dbContext.ProjectStaffMemberRoles.RemoveRange(projectStaffMemberRoles);
                                SaveChanges();
                                if (urole == null)
                                {
                                    userDetail.ProjectLinkage_IsActiveProjectUser = false;
                                }

                                var projectstaffRolesSQL = new ProjectStaffMemberRole()
                                {
                                    Guid = Guid.NewGuid(),
                                    ProjectId = conditionProjStaffResult != null ? conditionProjStaffResult.ProjectId : 1,
                                    UserId = uLogin.Id,
                                    RoleId = urole != null ? urole.Id : (int)Core.Enum.RoleTypes.Data_Entry,
                                    CreatedBy = createdBy.Id,
                                    StaffCreatedDate = DateTime.UtcNow,

                                    IsActiveProjectUser = userDetail.ProjectLinkage_IsActiveProjectUser,
                                    ProjectJoinedDate = projectJoinDate,
                                    ProjectLeftDate = userDetail.ProjectLinkage_ProjectLeftDate,
                                };
                                _dbContext.ProjectStaffMemberRoles.Add(projectstaffRolesSQL);
                                SaveChanges();
                                #endregion
                            }
                            else
                            {
                                #region assigne project to user in login table
                                var projId = _dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == linkedProjectGuid);

                                #region Set projectLinkage end date
                                try
                                {
                                    userDetail.ProjectLinkage_LinkedProjectId = projId.Guid;
                                    userDetail.ThisUserId = uLogin.Id;
                                    userDetail.ThisUserGuid = uLogin.Guid;
                                    userDetail.ThisUserName = uLogin.FirstName + " " + uLogin.LastName;

                                    var projectLeftDateActiveUser = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Actv);
                                    if (projectLeftDateActiveUser != null)
                                    {
                                        userDetail.ProjectLinkage_IsActiveProjectUser = !string.IsNullOrEmpty(projectLeftDateActiveUser.SelectedValues) ? (projectLeftDateActiveUser.SelectedValues == "1" ? true : false) : false;
                                        if (projectLeftDateActiveUser.SelectedValues == "0")
                                        {
                                            var projectLeftDateEnd = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.End);
                                            string edate = projectLeftDateEnd != null ? projectLeftDateEnd.SelectedValues : string.Empty;
                                            userDetail.ProjectLinkage_ProjectLeftDate = Convert.ToDateTime(edate);
                                        }
                                    }
                                }
                                catch (Exception exLinkage) { }
                                #endregion

                                IEnumerable<ProjectStaffMemberRole> projectStaffMemberRoles = this._dbContext.ProjectStaffMemberRoles.Where(x => x.UserId == uLogin.Id && x.FormDataEntry.Guid == linkedProjectGuid);
                                _dbContext.ProjectStaffMemberRoles.RemoveRange(projectStaffMemberRoles);
                                SaveChanges();
                                if (urole == null)
                                {
                                    userDetail.ProjectLinkage_IsActiveProjectUser = false;
                                }
                                var projectstaffRolesSQL = new ProjectStaffMemberRole()
                                {
                                    Guid = Guid.NewGuid(),
                                    ProjectId = projId != null ? projId.Id : 1,
                                    UserId = uLogin.Id,
                                    RoleId = urole != null ? urole.Id : (int)Core.Enum.RoleTypes.Data_Entry,
                                    CreatedBy = createdBy.Id,
                                    StaffCreatedDate = DateTime.UtcNow,

                                    IsActiveProjectUser = userDetail.ProjectLinkage_IsActiveProjectUser,
                                    ProjectJoinedDate = projectJoinDate,
                                    ProjectLeftDate = userDetail.ProjectLinkage_ProjectLeftDate,
                                };
                                _dbContext.ProjectStaffMemberRoles.Add(projectstaffRolesSQL);
                                SaveChanges();
                                #endregion


                            }
                        }
                    }
                    #endregion
                }


                #region update summary page activity
                if (model.Status == (int)Core.Enum.FormStatusTypes.Published || model.Status == (int)Core.Enum.FormStatusTypes.Draft || model.Status == (int)Core.Enum.FormStatusTypes.Submitted_for_review)
                {
                    Int64? ent = userDetail.ParentEntityNumber != null ? userDetail.ParentEntityNumber : userDetail.EntityNumber;

                    var updateSummaryPageActivity_condition = Query<SummaryPageActivityViewModel>.EQ(q => q.Id, new ObjectId(userDetail.SummaryPageActivityObjId));
                    var summaryPageActivity = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").Find(updateSummaryPageActivity_condition).AsQueryable().FirstOrDefault();
                    if (summaryPageActivity != null)
                    {
                        List<SummaryPageActivityForms> newForms = new List<SummaryPageActivityForms>();
                        foreach (var frm in summaryPageActivity.SummaryPageActivityFormsList)
                        {
                            if (isRegistrationForm)
                            {
                                if (frm.FormTitle == isDefaultForm.FormTitle)
                                {
                                    frm.FormStatusId = model.Status;
                                    frm.FormStatusName = Enum.GetName(typeof(Core.Enum.FormStatusTypes), model.Status);
                                    newForms.Add(frm);
                                }
                                else
                                {
                                    newForms.Add(frm);
                                }
                            }
                            else
                            {
                                if (frm.FormGuid == form.FormGuid)
                                {
                                    frm.FormStatusId = model.Status;
                                    frm.FormStatusName = Enum.GetName(typeof(Core.Enum.FormStatusTypes), model.Status);
                                    newForms.Add(frm);
                                }
                                else
                                {
                                    newForms.Add(frm);
                                }
                            }
                        }


                        var document_SummaryPageActivity = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity");
                        var detailscount_SummaryPageActivity = document_SummaryPageActivity.FindAs<SummaryPageActivityViewModel>(Query.EQ("_id", summaryPageActivity.Id)).Count();

                        if (detailscount_SummaryPageActivity > 0)
                        {
                            var qry_summaryPageActivity = Query<SummaryPageActivityViewModel>.EQ(p => p.Id, summaryPageActivity.Id);

                            var summaryPageActivityResult = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").FindOne(qry_summaryPageActivity);
                            summaryPageActivityResult.SummaryPageActivityFormsList = newForms;

                            // Document Collections  
                            var collection_SummaryPageActivity = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity");
                            try
                            { var result1 = collection_SummaryPageActivity.Update(qry_summaryPageActivity, Update.Replace(summaryPageActivityResult), UpdateFlags.None); }
                            catch (Exception e) { }

                        }
                    }
                }
                #endregion


                FormDataEntryViewModel mdl = new FormDataEntryViewModel();
                mdl.ParticipantId = Convert.ToString(userDetail.EntityNumber);

                userDetail.Id = new ObjectId(objId);
                userDetail.ModifiedDate = DateTime.UtcNow;
                userDetail.ModifiedBy = createdBy != null ? createdBy.Guid : model.CreatedBy;

                //Mongo Query  
                var CarObjectId = Query<FormDataEntryMongo>.EQ(p => p.Id, new ObjectId(objId));
                // Document Collections  
                var collection = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities");
                // Document Update which need Id and Data to Update  
                var result = collection.Update(CarObjectId, MongoDB.Driver.Builders.Update.Replace(userDetail), MongoDB.Driver.UpdateFlags.None);

                mdl.ThisUserId = userDetail.ThisUserId;

                mdl.ProjectId = FormDataEntryId;

                return mdl;

            }
            return null;
        }
        public FormDataEntryViewModel TestEnvironment_UpdateSearchForm(string objId, FormDataEntryViewModel model)
        {
            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);

            var condition = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, model.ProjectId);
            var project = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(condition).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();

            Form isDefaultForm = _dbContext.Forms.FirstOrDefault(x => x.Guid == model.FormId);

            bool isRegistrationForm = isDefaultForm != null ? (isDefaultForm.IsDefaultForm == (int)Core.Enum.DefaultFormType.Default ? true : false) : false;

            var activity = new ActivitiesMongo(); 
            var form = new FormsMongo(); 

            if (isRegistrationForm)
            {
                activity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityName == isDefaultForm.FormTitle);
                form = activity.FormsListMongo.FirstOrDefault(x => x.FormTitle == isDefaultForm.FormTitle);
            }
            else
            {
                activity = project.ProjectActivitiesList.FirstOrDefault(x => x.ActivityGuid == model.ActivityId);
                form = activity.FormsListMongo.FirstOrDefault(x => x.FormGuid == model.FormId);
            }

            var userEntitiesDocument = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities");

            var userDetailscount = userEntitiesDocument.FindAs<FormDataEntryMongo>(Query.EQ("_id", new ObjectId(objId))).Count();

            if (userDetailscount > 0)
            {
                var userObjectid = Query<FormDataEntryMongo>.EQ(p => p.Id, new ObjectId(objId));

                var userDetail = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindOne(userObjectid);

                var parent = userDetail.formDataEntryVariableMongoList.Where(x => x.VariableName == "EntID").FirstOrDefault();


                var new_userDetail_formDataEntryVariableMongoList = new List<FormDataEntryVariableMongo>();
                FormDataEntryVariableMongo formDataEntryVariableMongo = new FormDataEntryVariableMongo();
                formDataEntryVariableMongo.VariableId = parent.VariableId;
                formDataEntryVariableMongo.VariableGuid = parent.VariableGuid;
                formDataEntryVariableMongo.VariableName = parent.VariableName;
                formDataEntryVariableMongo.SelectedValues = parent.SelectedValues;
                formDataEntryVariableMongo.CreatedBy = createdBy.Id;
                formDataEntryVariableMongo.CreatedDate = DateTime.UtcNow;
                formDataEntryVariableMongo.ParentId = parent.ParentId;

                new_userDetail_formDataEntryVariableMongoList.Add(formDataEntryVariableMongo);

                foreach (var item in model.FormDataEntryVariable)
                {
                    formDataEntryVariableMongo = new FormDataEntryVariableMongo();
                    if (item.VariableId == 3)
                    {
                        continue;
                    }

                    var variableData = form.VariablesListMongo.FirstOrDefault(x => x.VariableId == item.VariableId);

                    formDataEntryVariableMongo.VariableId = item.VariableId;
                    formDataEntryVariableMongo.VariableGuid = variableData.VariableGuid;
                    formDataEntryVariableMongo.VariableName = variableData.VariableName;
                    formDataEntryVariableMongo.SelectedValues = !string.IsNullOrEmpty(item.SelectedValues) ? item.SelectedValues.Trim() : string.Empty;
                    formDataEntryVariableMongo.CreatedBy = createdBy.Id;
                    formDataEntryVariableMongo.CreatedDate = DateTime.UtcNow;
                    formDataEntryVariableMongo.ParentId = parent.SelectedValues != null ? Convert.ToInt32(parent.SelectedValues) : (int?)null;

                    if (variableData.VariableTypeName == VariableTypes.FileType.ToString())
                    {
                        formDataEntryVariableMongo.FileName = item.FileName;
                    }

                    new_userDetail_formDataEntryVariableMongoList.Add(formDataEntryVariableMongo);
                }


                userDetail.formDataEntryVariableMongoList = new_userDetail_formDataEntryVariableMongoList;

                #region save in login table
                string fname = string.Empty;
                string lname = string.Empty;
                string personRole = string.Empty;
                if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration))
                {
                    var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                    var f = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                    lname = l != null ? l.SelectedValues : string.Empty;
                    fname = f != null ? f.SelectedValues : string.Empty;
                }
                else if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration))
                {
                    var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                    var f = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                    var m = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 17);
                    lname = l != null ? l.SelectedValues : string.Empty;
                    fname = f != null ? f.SelectedValues : string.Empty + " " + m != null ? m.SelectedValues : string.Empty;
                }
                else if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration))
                {
                    var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                    fname = l != null ? l.SelectedValues : string.Empty;
                }
                personRole = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 43) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 43).SelectedValues : string.Empty;  //SysRole

                if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                    || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                    || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration))
                {
                    var emali = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38).SelectedValues : string.Empty;
                    var username = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40).SelectedValues : string.Empty;
                    if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration))
                    {
                        var EntGrpVar = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 5) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 5).SelectedValues : string.Empty;
                        if (EntGrpVar != "15")
                        {
                            var loginTbl = _dbContext.UserLogins.FirstOrDefault(x => x.Id == userDetail.ThisUserId);
                            if (loginTbl != null)
                            {
                                loginTbl.Password = null;
                                loginTbl.Salt = null;
                                loginTbl.SecurityQuestionId = null;
                                loginTbl.Answer = null;
                                _dbContext.SaveChanges();
                            }
                        }
                    }

                    var authTypeGuid = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51).SelectedValues : new Guid().ToString();
                    if (!string.IsNullOrEmpty(authTypeGuid) && !string.IsNullOrEmpty(username))
                    {
                        LoginAuthTypeMaster authType = null;
                        Guid at = Guid.Empty;
                        try
                        {
                            at = new Guid(authTypeGuid);
                            var authTypeId = _dbContext.LoginAuthTypeMasters.Where(x => x.DateDeactivated == null && x.Guid == at).Select(x => new { id = x.Guid.ToString(), name = x.AuthTypeName }).FirstOrDefault();
                            authType = _dbContext.LoginAuthTypeMasters.FirstOrDefault(x => x.Guid == new Guid(authTypeId.id));
                        }
                        catch (Exception dd)
                        {
                            var authTypeId = _dbContext.LoginAuthTypeMasters.Where(x => x.DateDeactivated == null && x.Id == 1).Select(x => new { id = x.Guid.ToString(), name = x.AuthTypeName }).FirstOrDefault();
                            authType = _dbContext.LoginAuthTypeMasters.FirstOrDefault(x => x.Guid == new Guid(authTypeId.id));
                        }

                        var logintable = _dbContext.UserLogins.FirstOrDefault(x => x.Id == userDetail.ThisUserId);

                        Role userrole = null;

                        int roletypeid = !string.IsNullOrEmpty(personRole) ? Convert.ToInt32(personRole) : 0;
                        if (roletypeid == 1)
                            userrole = _dbContext.Roles.FirstOrDefault(x => x.Id == (int)Core.Enum.RoleTypes.System_Admin);
                        else
                            userrole = _dbContext.Roles.FirstOrDefault(x => x.Id == (int)Core.Enum.RoleTypes.Data_Entry_Supervisor);



                        UserLoginViewModel entityUserlogin = new UserLoginViewModel();
                        FormDataEntryVariableViewModel userStatusModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 42);
                        int userStatus = 0;
                        if (userStatusModel != null)
                        {
                            userStatus = !string.IsNullOrEmpty(userStatusModel.SelectedValues) ? Convert.ToInt32(userStatusModel.SelectedValues) : 0;
                        }

                        #region is user approved by system admin
                        bool isUserApprovedBySystemAdmin = false;
                        FormDataEntryVariableViewModel isUserApprovedBySystemAdminModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)Core.Enum.DefaultsVariables.SysAppr);
                        if (isUserApprovedBySystemAdminModel != null)
                        {
                            int isUserApprovedBySystemAdminInt = !string.IsNullOrEmpty(isUserApprovedBySystemAdminModel.SelectedValues) ? Convert.ToInt32(isUserApprovedBySystemAdminModel.SelectedValues) : 0;
                            isUserApprovedBySystemAdmin = isUserApprovedBySystemAdminInt == 1 ? true : false;
                        }
                        #endregion
                        if (userStatus == (int)Core.Enum.Status.Active)
                        {
                            userStatus = (int)Core.Enum.Status.Active;
                        }
                        else if (logintable.Status == (int)Core.Enum.Status.Locked)
                        {
                            userStatus = (int)Core.Enum.Status.Locked;
                        }
                        else
                        {
                            userStatus = (int)Core.Enum.Status.InActive;
                        }
                        entityUserlogin.FirstName = fname;
                        entityUserlogin.LastName = lname;
                        entityUserlogin.Email = emali;
                        entityUserlogin.UserName = username;
                        entityUserlogin.Mobile = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 39) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 39).SelectedValues : null;
                        entityUserlogin.TenantId = model.TenantId != null ? (Guid)model.TenantId : new Guid();
                        entityUserlogin.AuthTypeId = authType != null ? authType.Id : 0;
                        entityUserlogin.UserTypeId = (int)Core.Enum.UsersLoginType.Test;
                        entityUserlogin.TempGuid = Guid.NewGuid();
                        entityUserlogin.RoleId = userrole != null ? userrole.Guid : new Guid();
                        entityUserlogin.ModifiedBy = model.CreatedBy;
                        entityUserlogin.Guid = logintable.Guid;
                        entityUserlogin.Id = logintable.Id;

                        entityUserlogin.Status = userStatus;
                        entityUserlogin.IsUserApprovedBySystemAdmin = isUserApprovedBySystemAdmin;

                        _userLoginProvider.Update(entityUserlogin);

                        var login = _dbContext.UserLogins.FirstOrDefault(x => x.Guid == logintable.Guid);
                        if (login != null)
                        {
                            login.AuthTypeId = authType != null ? authType.Id : 0;
                            _dbContext.SaveChanges();
                        }
                    }

                    if (form.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
                    {
                        #region place/group form login update
                        var EntGrpVar = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 5) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 5).SelectedValues : string.Empty;
                        if (EntGrpVar != "15")
                        {
                            var loginTbl = _dbContext.UserLogins.FirstOrDefault(x => x.Id == userDetail.ThisUserId);
                            if (loginTbl != null)
                            {
                                loginTbl.Password = null;
                                loginTbl.Salt = null;
                                loginTbl.SecurityQuestionId = null;
                                loginTbl.Answer = null;
                                loginTbl.Status = (int)Core.Enum.Status.InActive;
                                loginTbl.IsUserApprovedBySystemAdmin = false;

                                loginTbl.LoginFailedAttemptCount = null;
                                loginTbl.LoginFailedAttemptDate = null;

                                _dbContext.SaveChanges();
                            }
                        }
                        else
                        {
                            #region place form login update
                            //Approved by System Admin field is not available into "Participant Registration" form
                            try
                            {
                                FormDataEntryVariableViewModel isUserApprovedBySystemAdminModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Active);
                                int isUserApprovedBySystemAdminInt = !string.IsNullOrEmpty(isUserApprovedBySystemAdminModel.SelectedValues) ? Convert.ToInt32(isUserApprovedBySystemAdminModel.SelectedValues) : 0;
                                int userLoginStatus = (int)Core.Enum.Status.InActive;
                                bool isUserApprovedBySystemAdmin = isUserApprovedBySystemAdminInt == 1 ? true : false;

                                var loginTbl = _dbContext.UserLogins.FirstOrDefault(x => x.Id == userDetail.ThisUserId);

                                FormDataEntryVariableViewModel isActiveUserModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)Core.Enum.DefaultsVariables.Active);
                                if (isActiveUserModel != null)
                                {
                                    //int isActiveUserModelInt = !string.IsNullOrEmpty(isActiveUserModel.SelectedValues) ? Convert.ToInt32(isActiveUserModel.SelectedValues) : 0;
                                    //userLoginStatus = isActiveUserModelInt == 1 ? (int)Core.Enum.Status.Active : (int)Core.Enum.Status.InActive;

                                    userLoginStatus = !string.IsNullOrEmpty(isActiveUserModel.SelectedValues) ? Convert.ToInt32(isActiveUserModel.SelectedValues) : 0;
                                    if (userLoginStatus == (int)Core.Enum.Status.Active)
                                    {
                                        userLoginStatus = (int)Core.Enum.Status.Active;
                                    }
                                    else if (loginTbl != null && loginTbl.Status == (int)Core.Enum.Status.Locked)
                                    {
                                        userLoginStatus = (int)Core.Enum.Status.Locked;
                                    }
                                    else
                                    {
                                        userLoginStatus = (int)Core.Enum.Status.InActive;
                                    }

                                }


                                if (loginTbl != null)
                                {
                                    loginTbl.Status = userLoginStatus;
                                    loginTbl.IsUserApprovedBySystemAdmin = isUserApprovedBySystemAdmin;

                                    if (userLoginStatus == (int)Core.Enum.Status.Active)
                                    {
                                        loginTbl.LoginFailedAttemptCount = null;
                                        loginTbl.LoginFailedAttemptDate = null;
                                    }
                                    _dbContext.SaveChanges();
                                }
                            }
                            catch (Exception excActive) { Console.WriteLine(excActive.Message); }
                            #endregion
                        }
                        #endregion
                    }
                    else if (form.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration))
                    {
                        #region person reg form login update
                        bool isUserApprovedBySystemAdmin = false;
                        int userLoginStatus = (int)Core.Enum.Status.InActive;
                        FormDataEntryVariableViewModel isUserApprovedBySystemAdminModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)Core.Enum.DefaultsVariables.SysAppr);
                        if (isUserApprovedBySystemAdminModel != null)
                        {
                            int isUserApprovedBySystemAdminInt = !string.IsNullOrEmpty(isUserApprovedBySystemAdminModel.SelectedValues) ? Convert.ToInt32(isUserApprovedBySystemAdminModel.SelectedValues) : 0;
                            isUserApprovedBySystemAdmin = isUserApprovedBySystemAdminInt == 1 ? true : false;
                        }
                        FormDataEntryVariableViewModel isActiveUserModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)Core.Enum.DefaultsVariables.Active);
                        var loginTbl = _dbContext.UserLogins.FirstOrDefault(x => x.Id == userDetail.ThisUserId);
                        if (isActiveUserModel != null)
                        {
                            userLoginStatus = !string.IsNullOrEmpty(isActiveUserModel.SelectedValues) ? Convert.ToInt32(isActiveUserModel.SelectedValues) : 0;
                            if (userLoginStatus == (int)Core.Enum.Status.Active)
                            {
                                userLoginStatus = (int)Core.Enum.Status.Active;
                            }
                            else if (loginTbl != null && loginTbl.Status == (int)Core.Enum.Status.Locked)
                            {
                                userLoginStatus = (int)Core.Enum.Status.Locked;
                            }
                            else
                            {
                                userLoginStatus = (int)Core.Enum.Status.InActive;
                            }
                            //userLoginStatus = isActiveUserModelInt == 1 ? (int)Core.Enum.Status.Active : (int)Core.Enum.Status.InActive;
                        }


                        if (loginTbl != null)
                        {
                            loginTbl.Status = userLoginStatus;
                            loginTbl.IsUserApprovedBySystemAdmin = isUserApprovedBySystemAdmin;

                            if (userLoginStatus == (int)Core.Enum.Status.Active)
                            {
                                loginTbl.LoginFailedAttemptCount = null;
                                loginTbl.LoginFailedAttemptDate = null;
                            }

                            _dbContext.SaveChanges();
                        }
                        #endregion
                    }
                    else if (form.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
                    {
                        #region participant form login update
                        //Approved by System Admin field is not available into "Participant Registration" form
                        try
                        {
                            var loginTbl = _dbContext.UserLogins.FirstOrDefault(x => x.Id == userDetail.ThisUserId);
                            FormDataEntryVariableViewModel isUserApprovedBySystemAdminModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Active);
                            int isUserApprovedBySystemAdminInt = !string.IsNullOrEmpty(isUserApprovedBySystemAdminModel.SelectedValues) ? Convert.ToInt32(isUserApprovedBySystemAdminModel.SelectedValues) : 0;

                            if (isUserApprovedBySystemAdminInt == (int)Core.Enum.Status.Active)
                            {
                                isUserApprovedBySystemAdminInt = (int)Core.Enum.Status.Active;
                            }
                            else if (loginTbl != null && loginTbl.Status == (int)Core.Enum.Status.Locked)
                            {
                                isUserApprovedBySystemAdminInt = (int)Core.Enum.Status.Locked;
                            }
                            else
                            {
                                isUserApprovedBySystemAdminInt = (int)Core.Enum.Status.InActive;
                            }

                            bool isUserApprovedBySystemAdmin = isUserApprovedBySystemAdminInt == 1 ? true : false;


                            if (loginTbl != null)
                            {
                                loginTbl.Status = isUserApprovedBySystemAdminInt;
                                loginTbl.IsUserApprovedBySystemAdmin = isUserApprovedBySystemAdmin;
                                if (isUserApprovedBySystemAdminInt == (int)Core.Enum.Status.Active)
                                {
                                    loginTbl.LoginFailedAttemptCount = null;
                                    loginTbl.LoginFailedAttemptDate = null;
                                }
                                _dbContext.SaveChanges();
                            }
                        }
                        catch (Exception excActive) { Console.WriteLine(excActive.Message); }
                        #endregion
                    }
                }
                #endregion

                if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Linkage))
                {
                    #region assig project to user
                    var projectLinkageCondition = Query<FormDataEntryMongo>.EQ(q => q.EntityNumber, userDetail.ParentEntityNumber);
                    var projectLinkageConditionEntity = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(projectLinkageCondition).AsQueryable().FirstOrDefault();
                    if (projectLinkageConditionEntity != null)
                    {
                        UserLogin uLogin = _dbContext.UserLogins.FirstOrDefault(x => x.Id == projectLinkageConditionEntity.ThisUserId);
                        if (uLogin != null)
                        {
                            var proRole = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 56);//ProRole variable of variable table
                            Role urole = new Role();
                            if (proRole != null)
                            {
                                Guid role = !string.IsNullOrEmpty(proRole.SelectedValues) ? new Guid(proRole.SelectedValues) : Guid.Empty;
                                urole = _dbContext.Roles.FirstOrDefault(x => x.Guid == role);
                            }

                            var linkedProjectId = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 52);  //LnkPro(Linked Project) variable
                            Guid linkedProjectGuid = Guid.Parse(linkedProjectId.SelectedValues);

                            var conditionProjStaff = Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, linkedProjectGuid);
                            var conditionProjStaffResult = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(conditionProjStaff).AsQueryable().OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();

                            if (conditionProjStaffResult != null)
                            {
                                #region Set projectLinkage end date
                                DateTime? projectJoinDate = null;
                                try
                                {
                                    var projJoindDate = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Join);
                                    string jDate = projJoindDate != null ? projJoindDate.SelectedValues : string.Empty;
                                    projectJoinDate = Convert.ToDateTime(jDate);
                                }
                                catch (Exception joinDete) { }
                                try
                                {
                                    userDetail.ProjectLinkage_LinkedProjectId = conditionProjStaffResult.ProjectGuid;
                                    var projectLeftDateActiveUser = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Actv);
                                    if (projectLeftDateActiveUser != null)
                                    {
                                        userDetail.ProjectLinkage_IsActiveProjectUser = !string.IsNullOrEmpty(projectLeftDateActiveUser.SelectedValues) ? (projectLeftDateActiveUser.SelectedValues == "1" ? true : false) : false;
                                        if (projectLeftDateActiveUser.SelectedValues == "0")
                                        {
                                            var projectLeftDateEnd = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.End);
                                            string edate = projectLeftDateEnd != null ? projectLeftDateEnd.SelectedValues : string.Empty;
                                            userDetail.ProjectLinkage_ProjectLeftDate = Convert.ToDateTime(edate);
                                        }
                                    }
                                }
                                catch (Exception exLinkage) { }
                                if (urole == null)
                                {
                                    userDetail.ProjectLinkage_IsActiveProjectUser = false;
                                }
                                #endregion

                                if (conditionProjStaffResult.ProjectStaffListMongo.Any(x => x.StaffGuid == uLogin.Guid))
                                {
                                    conditionProjStaffResult.ProjectStaffListMongo.Remove(conditionProjStaffResult.ProjectStaffListMongo.FirstOrDefault(x => x.StaffGuid == uLogin.Guid));
                                }
                                conditionProjStaffResult.ProjectStaffListMongo.Add(new ProjectStaffMongo()
                                {
                                    Role = urole != null ? urole.Name : string.Empty,
                                    StaffName = uLogin != null ? uLogin.FirstName + " " + uLogin.LastName : string.Empty,
                                    StaffGuid = uLogin != null ? uLogin.Guid : Guid.Empty,
                                    IsActiveProjectUser = userDetail.ProjectLinkage_IsActiveProjectUser,
                                    ProjectJoinedDate = projectJoinDate,
                                    ProjectLeftDate = userDetail.ProjectLinkage_ProjectLeftDate,
                                });

                                ////Mongo Query    lok-----------------------------------------------------
                                var updateProjectObjectId = Query<ProjectDeployViewModel>.EQ(p => p.Id, conditionProjStaffResult.Id);
                                // Document Collections  
                                var updateProjectCollection = _testMongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects");
                                // Document Update which need Id and Data to Update  
                                try
                                {
                                    var isUpdateProj = updateProjectCollection.Update(updateProjectObjectId, MongoDB.Driver.Builders.Update.Replace(conditionProjStaffResult), MongoDB.Driver.UpdateFlags.None);
                                }
                                catch (Exception e) { }

                                #region assigne project to user in login table
                                IEnumerable<ProjectStaffMemberRole> projectStaffMemberRoles = this._dbContext.ProjectStaffMemberRoles.Where(x => x.UserId == uLogin.Id && x.FormDataEntry.Guid == linkedProjectGuid);
                                _dbContext.ProjectStaffMemberRoles.RemoveRange(projectStaffMemberRoles);
                                SaveChanges();


                                var projLnk = _dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == linkedProjectGuid);
                                var projectstaffRolesSQL = new ProjectStaffMemberRole()
                                {
                                    Guid = Guid.NewGuid(),
                                    ProjectId = projLnk != null ? projLnk.Id : 1,
                                    UserId = uLogin.Id,
                                    RoleId = urole != null ? urole.Id : (int)Core.Enum.RoleTypes.Data_Entry,
                                    CreatedBy = createdBy.Id,
                                    StaffCreatedDate = DateTime.UtcNow,

                                    IsActiveProjectUser = userDetail.ProjectLinkage_IsActiveProjectUser,
                                    ProjectJoinedDate = projectJoinDate,
                                    ProjectLeftDate = userDetail.ProjectLinkage_ProjectLeftDate,
                                };
                                _dbContext.ProjectStaffMemberRoles.Add(projectstaffRolesSQL);
                                SaveChanges();
                                #endregion
                            }
                            else
                            {
                                #region assigne project to user in login table
                                //var linkedProjectId = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 52);  //LnkPro(Linked Project) variable
                                //Guid linkedProjectGuid = Guid.Parse(linkedProjectId.SelectedValues);

                                var projId = _dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == linkedProjectGuid);

                                IEnumerable<ProjectStaffMemberRole> projectStaffMemberRoles = this._dbContext.ProjectStaffMemberRoles.Where(x => x.UserId == uLogin.Id && x.FormDataEntry.Guid == linkedProjectGuid);
                                _dbContext.ProjectStaffMemberRoles.RemoveRange(projectStaffMemberRoles);
                                SaveChanges();
                                var projectstaffRolesSQL = new ProjectStaffMemberRole()
                                {
                                    Guid = Guid.NewGuid(),
                                    ProjectId = projId != null ? projId.Id : 1,
                                    UserId = uLogin.Id,
                                    RoleId = urole != null ? urole.Id : (int)Core.Enum.RoleTypes.Data_Entry,
                                    CreatedBy = createdBy.Id,
                                    StaffCreatedDate = DateTime.UtcNow,
                                };
                                _dbContext.ProjectStaffMemberRoles.Add(projectstaffRolesSQL);
                                SaveChanges();
                                #endregion
                            }
                        }
                    }
                    #endregion
                }

                #region update summary page activity
                if (model.Status == (int)Core.Enum.FormStatusTypes.Published || model.Status == (int)Core.Enum.FormStatusTypes.Draft || model.Status == (int)Core.Enum.FormStatusTypes.Submitted_for_review)
                {
                    Int64? ent = userDetail.ParentEntityNumber != null ? userDetail.ParentEntityNumber : userDetail.EntityNumber;

                    var updateSummaryPageActivity_condition = Query<SummaryPageActivityViewModel>.EQ(q => q.Id, new ObjectId(userDetail.SummaryPageActivityObjId));
                    var summaryPageActivity = _testMongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").Find(updateSummaryPageActivity_condition).AsQueryable().FirstOrDefault();


                    if (summaryPageActivity != null)
                    {
                        List<SummaryPageActivityForms> newForms = new List<SummaryPageActivityForms>();
                        foreach (var frm in summaryPageActivity.SummaryPageActivityFormsList)
                        {
                            if (isRegistrationForm)
                            {
                                if (frm.FormTitle == isDefaultForm.FormTitle)
                                {
                                    frm.FormStatusId = model.Status;
                                    frm.FormStatusName = Enum.GetName(typeof(Core.Enum.FormStatusTypes), model.Status);
                                    newForms.Add(frm);
                                }
                                else
                                {
                                    newForms.Add(frm);
                                }
                            }
                            else
                            {
                                if (frm.FormGuid == form.FormGuid)
                                {
                                    frm.FormStatusId = model.Status;
                                    frm.FormStatusName = Enum.GetName(typeof(Core.Enum.FormStatusTypes), model.Status);
                                    newForms.Add(frm);
                                }
                                else
                                {
                                    newForms.Add(frm);
                                }
                            }
                        }


                        var document_SummaryPageActivity = _testMongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity");
                        var detailscount_SummaryPageActivity = document_SummaryPageActivity.FindAs<SummaryPageActivityViewModel>(Query.EQ("_id", summaryPageActivity.Id)).Count();

                        if (detailscount_SummaryPageActivity > 0)
                        {
                            var qry_summaryPageActivity = Query<SummaryPageActivityViewModel>.EQ(p => p.Id, summaryPageActivity.Id);

                            var summaryPageActivityResult = _testMongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").FindOne(qry_summaryPageActivity);
                            summaryPageActivityResult.SummaryPageActivityFormsList = newForms;

                            // Document Collections  
                            var collection_SummaryPageActivity = _testMongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity");
                            try
                            { var result1 = collection_SummaryPageActivity.Update(qry_summaryPageActivity, Update.Replace(summaryPageActivityResult), UpdateFlags.None); }
                            catch (Exception e) { }

                        }
                    }
                }
                #endregion

                FormDataEntryViewModel mdl = new FormDataEntryViewModel();
                mdl.ParticipantId = Convert.ToString(userDetail.EntityNumber);
                userDetail.Id = new ObjectId(objId);
                userDetail.ModifiedDate = DateTime.UtcNow;
                userDetail.ModifiedBy = createdBy != null ? createdBy.Guid : model.CreatedBy;

                //Mongo Query  
                var CarObjectId = Query<FormDataEntryMongo>.EQ(p => p.Id, new ObjectId(objId));
                // Document Collections  
                var collection = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities");
                // Document Update which need Id and Data to Update  
                var result = collection.Update(CarObjectId, MongoDB.Driver.Builders.Update.Replace(userDetail), MongoDB.Driver.UpdateFlags.None);

                mdl.ThisUserId = userDetail.ThisUserId;
                return mdl;
            }
            return null;
        }



        public Guid? GetCurrentAuthType(string userEntityObjId)
        {
            var entityOnjQuery = Query<FormDataEntryMongo>.EQ(p => p.Id, new ObjectId(userEntityObjId));
            var entityOnjDetails = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindOne(entityOnjQuery);

            var isAuthenticationMethod = entityOnjDetails.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "AuthenticationMethod");
            if (isAuthenticationMethod != null)
            {
                try
                {
                    Guid? g = new Guid(isAuthenticationMethod.SelectedValues);
                    return g;
                }
                catch (Exception ex)
                { }
            }
            return null;
        }
        public Guid? TestEnvironment_GetCurrentAuthType(string userEntityObjId)
        {
            var entityOnjQuery = Query<FormDataEntryMongo>.EQ(p => p.Id, new ObjectId(userEntityObjId));
            var entityOnjDetails = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindOne(entityOnjQuery);

            var isAuthenticationMethod = entityOnjDetails.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == "AuthenticationMethod");
            if (isAuthenticationMethod != null)
            {
                try
                {
                    Guid? g = new Guid(isAuthenticationMethod.SelectedValues);
                    return g;
                }
                catch (Exception ex)
                { }
            }
            return null;
        }

        //Generate RandomNo
        public int GenerateRandomNo()
        {
             
            IMongoQuery conditionRegistrationsID = Query.Or(
                Query<FormDataEntryMongo>.EQ(q => q.FormTitle, Core.Enum.EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration)),
                Query<FormDataEntryMongo>.EQ(q => q.FormTitle, Core.Enum.EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration)),
                Query<FormDataEntryMongo>.EQ(q => q.FormTitle, Core.Enum.EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration)),
                Query<FormDataEntryMongo>.EQ(q => q.FormTitle, Core.Enum.EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration)));

            var IdList = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(conditionRegistrationsID).AsQueryable();


            Int64 cnt = 0;
            if (IdList.Count() != 0)
            {
                cnt = IdList.Max(x => x.EntityNumber);
            }
            if (cnt == 0)
            {
                return (int)(cnt + 2);
            }
            else
            {
                return (int)(cnt + 1);
            }
        }
        public int GenerateRandomNo6Digit()
        {
            Random _rdm = new Random();
            var rno = 0;
            var IdList = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().AsQueryable().Select(x => Convert.ToString(x.EntityNumber)).ToList();
            var IdListSQL = _dbContext.FormDataEntryVariables.Where(x => x.Variable.VariableName == "EntID").Select(x => x.SelectedValues).ToList();
            IdList = IdList.Union(IdListSQL).ToList();
            do
            {
                rno = _rdm.Next(100000000, 900000000);
            } while (IdList.Contains(rno.ToString()));

            return rno;
        }

        public int TestEnvironment_GenerateRandomNo()
        {
            IMongoQuery conditionRegistrationsID = Query.Or(
               Query<FormDataEntryMongo>.EQ(q => q.FormTitle, Core.Enum.EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration)),
               Query<FormDataEntryMongo>.EQ(q => q.FormTitle, Core.Enum.EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration)),
               Query<FormDataEntryMongo>.EQ(q => q.FormTitle, Core.Enum.EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration)),
               Query<FormDataEntryMongo>.EQ(q => q.FormTitle, Core.Enum.EnumHelpers.GetEnumDescription(DefaultFormName.Project_Registration)));

            var IdList = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(conditionRegistrationsID).AsQueryable();

            Int64 cnt = 0;
            if (IdList.Count() != 0)
            {
                cnt = IdList.Max(x => x.EntityNumber);
            }
            if (cnt == 0)
            {
                return (int)(cnt + 2);
            }
            else
            {
                return (int)(cnt + 1);
            }
        }
        public int TestEnvironment_GenerateRandomNo6Digit()
        {
            Random _rdm = new Random();
            var rno = 0;
            var IdList = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().AsQueryable().Select(x => Convert.ToString(x.EntityNumber)).ToList();
            do
            {
                rno = _rdm.Next(100000000, 900000000);
            } while (IdList.Contains(rno.ToString()));

            return rno;
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }





        public List<List<FormDataEntryVariableViewModel>> SearchEntityInSQLDB(SearchPageVariableViewModel model, string source = null)
        {
            return _formDataEntryProvider.SearchVariables(model, source);
        }


        public string CheckEntityExistenceLocation(string entityId, Guid LoggedInUserId)
        {
            string Result = null;

            long? id = !string.IsNullOrEmpty(entityId) ? Convert.ToInt32(entityId) : (int?)null;

            var condition = Query<FormDataEntryMongo>.EQ(q => q.EntityNumber, id);
            var userEntity = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(condition).AsQueryable().FirstOrDefault();
            if (userEntity != null)
            {
                Result = AspreeDatabaseType.Mongo.ToString();
            }
            else
            {
                var userEntitySQL = _dbContext.FormDataEntries.FirstOrDefault(x => x.EntityNumber == id);
                if (userEntitySQL != null)
                    Result = AspreeDatabaseType.SQL.ToString();
            }
            return Result;
        }


        public FormDataEntryMongo Delete(string id, Guid loggedInUserId)
        {
            Guid formId = Guid.Empty;
            string summaryPageActivityObjId = string.Empty;
            try
            {
                //Mongo Query  
                var userObjectid = Query<FormDataEntryMongo>.EQ(p => p.Id, new ObjectId(id));

                var userDetail = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindOne(userObjectid);
                if (userDetail != null)
                {
                    //store values in local for further use
                    formId = userDetail.FormGuid;
                    summaryPageActivityObjId = userDetail.SummaryPageActivityObjId;

                    //set deactivated values in object
                    userDetail.DateDeactivated = DateTime.UtcNow;
                    userDetail.DeactivatedBy = loggedInUserId;
                    userDetail.SummaryPageActivityObjId = string.Empty;


                    // Document Collections
                    var collection = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities");

                    // Document Update which need Id and Data to Update  
                    var result = collection.Update(userObjectid, Update.Replace(userDetail), UpdateFlags.None);

                    #region Update status of summary page form
                    var updateSummaryPageActivity_condition = Query<SummaryPageActivityViewModel>.EQ(q => q.Id, new ObjectId(summaryPageActivityObjId));
                    var summaryPageActivity = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").Find(updateSummaryPageActivity_condition).AsQueryable().FirstOrDefault();
                    if (summaryPageActivity != null)
                    {
                        List<SummaryPageActivityForms> newForms = new List<SummaryPageActivityForms>();
                        foreach (var frm in summaryPageActivity.SummaryPageActivityFormsList)
                        {
                            if (frm.FormGuid == formId)
                            {
                                frm.FormStatusId = (int)FormStatusTypes.Not_entered;
                                frm.FormStatusName = Enum.GetName(typeof(Core.Enum.FormStatusTypes), (int)FormStatusTypes.Not_entered);
                                newForms.Add(frm);
                            }
                            else
                            {
                                newForms.Add(frm);
                            }
                        }
                        var document_SummaryPageActivity = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity");
                        var detailscount_SummaryPageActivity = document_SummaryPageActivity.FindAs<SummaryPageActivityViewModel>(Query.EQ("_id", summaryPageActivity.Id)).Count();
                        if (detailscount_SummaryPageActivity > 0)
                        {
                            var qry_summaryPageActivity = Query<SummaryPageActivityViewModel>.EQ(p => p.Id, summaryPageActivity.Id);

                            var summaryPageActivityResult = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").FindOne(qry_summaryPageActivity);
                            summaryPageActivityResult.SummaryPageActivityFormsList = newForms;

                            var collectionSumm = _mongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity");
                            try { var result1 = collectionSumm.Update(qry_summaryPageActivity, Update.Replace(summaryPageActivityResult), UpdateFlags.None); }
                            catch (Exception e) { }
                        }
                    }
                    #endregion

                    return userDetail;
                }
                return null;
            }
            catch (Exception edx)
            {
                throw new Core.AlreadyExistsException("Some error occured.");
            }
        }

        public FormDataEntryMongo TestEnvironment_Delete(string id, Guid loggedInUserId)
        {
            Guid formId = Guid.Empty;
            string summaryPageActivityObjId = string.Empty;
            try
            {
                //Mongo Query  
                var userObjectid = Query<FormDataEntryMongo>.EQ(p => p.Id, new ObjectId(id));

                var userDetail = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindOne(userObjectid);
                if (userDetail != null)
                {
                    //store values in local for further use
                    formId = userDetail.FormGuid;
                    summaryPageActivityObjId = userDetail.SummaryPageActivityObjId;

                    //set deactivated values in object
                    userDetail.DateDeactivated = DateTime.UtcNow;
                    userDetail.DeactivatedBy = loggedInUserId;
                    userDetail.SummaryPageActivityObjId = string.Empty;


                    // Document Collections
                    var collection = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities");

                    // Document Update which need Id and Data to Update  
                    var result = collection.Update(userObjectid, Update.Replace(userDetail), UpdateFlags.None);

                    #region Update status of summary page form
                    var updateSummaryPageActivity_condition = Query<SummaryPageActivityViewModel>.EQ(q => q.Id, new ObjectId(summaryPageActivityObjId));
                    var summaryPageActivity = _testMongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").Find(updateSummaryPageActivity_condition).AsQueryable().FirstOrDefault();
                    if (summaryPageActivity != null)
                    {
                        List<SummaryPageActivityForms> newForms = new List<SummaryPageActivityForms>();
                        foreach (var frm in summaryPageActivity.SummaryPageActivityFormsList)
                        {
                            if (frm.FormGuid == formId)
                            {
                                frm.FormStatusId = (int)FormStatusTypes.Not_entered;
                                frm.FormStatusName = Enum.GetName(typeof(Core.Enum.FormStatusTypes), (int)FormStatusTypes.Not_entered);
                                newForms.Add(frm);
                            }
                            else
                            {
                                newForms.Add(frm);
                            }
                        }
                        var document_SummaryPageActivity = _testMongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity");
                        var detailscount_SummaryPageActivity = document_SummaryPageActivity.FindAs<SummaryPageActivityViewModel>(Query.EQ("_id", summaryPageActivity.Id)).Count();
                        if (detailscount_SummaryPageActivity > 0)
                        {
                            var qry_summaryPageActivity = Query<SummaryPageActivityViewModel>.EQ(p => p.Id, summaryPageActivity.Id);

                            var summaryPageActivityResult = _testMongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity").FindOne(qry_summaryPageActivity);
                            summaryPageActivityResult.SummaryPageActivityFormsList = newForms;

                            var collectionSumm = _testMongoDBContext._database.GetCollection<SummaryPageActivityViewModel>("SummaryPageActivity");
                            try { var result1 = collectionSumm.Update(qry_summaryPageActivity, Update.Replace(summaryPageActivityResult), UpdateFlags.None); }
                            catch (Exception e) { }
                        }
                    }
                    #endregion

                    return userDetail;
                }
                return null;
            }
            catch (Exception edx)
            {
                throw new Core.AlreadyExistsException("Some error occured.");
            }
        }

    }
}