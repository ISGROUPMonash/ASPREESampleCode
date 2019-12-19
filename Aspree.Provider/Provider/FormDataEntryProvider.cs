using Aspree.Core.Enum;
using Aspree.Core.ViewModels;
using Aspree.Core.ViewModels.MongoViewModels;
using Aspree.Data;
using Aspree.Data.MongoDB;
using Aspree.Provider.Interface;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Provider
{
    public class FormDataEntryProvider : IFormDataEntryProvider
    {
        private readonly IUserLoginProvider _userLoginProvider;
        private readonly AspreeEntities dbContext;

        private readonly IPrivilegeProvider _privilegeProvider;
        private readonly AspreeEntities _dbContext;
        private readonly MongoDBContext _mongoDBContext;
        private readonly TestMongoDBContext _testMongoDBContext;

        public FormDataEntryProvider(
            AspreeEntities _dbContext
            , IUserLoginProvider userLoginProvider
            , IPrivilegeProvider privilegeProvider
            , MongoDBContext mongoDBContext
            , TestMongoDBContext testMongoDBContext
            )
        {
            this.dbContext = _dbContext;
            this._userLoginProvider = userLoginProvider;
            this._privilegeProvider = privilegeProvider;
            this._dbContext = dbContext;
            this._mongoDBContext = mongoDBContext;
            this._testMongoDBContext = testMongoDBContext;
        }

        public FormDataEntryViewModel Create(FormDataEntryViewModel model)
        {
            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);

            var form = dbContext.Forms.FirstOrDefault(f => f.Guid == model.FormId);
            var entity = dbContext.EntityTypes.FirstOrDefault(x => x.Guid == model.EntityId);
            var activity = dbContext.Activities.FirstOrDefault(x => x.Guid == model.ActivityId);

            string fname = string.Empty;
            string lname = string.Empty;

            if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration))
            {
                activity = dbContext.Activities.FirstOrDefault(x => x.ActivityName == "Person Registration" && x.FormDataEntry.Guid == model.ProjectId);
                entity = dbContext.EntityTypes.FirstOrDefault(x => x.Name == "Person");

                var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                var f = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                lname = l != null ? l.SelectedValues : string.Empty;
                fname = f != null ? f.SelectedValues : string.Empty;

            }
            else if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration))
            {
                activity = dbContext.Activities.FirstOrDefault(x => x.ActivityName == "Participant Registration" && x.FormDataEntry.Guid == model.ProjectId);
                entity = dbContext.EntityTypes.FirstOrDefault(x => x.Name == "Participant");

                var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                var f = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                var m = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 17);

                lname = l != null ? l.SelectedValues : string.Empty;
                fname = f != null ? f.SelectedValues : string.Empty + " " + m != null ? m.SelectedValues : string.Empty;
            }
            else if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration))
            {
                activity = dbContext.Activities.FirstOrDefault(x => x.ActivityName == "Place/Group Registration" && x.FormDataEntry.Guid == model.ProjectId);
                entity = dbContext.EntityTypes.FirstOrDefault(x => x.Name == "Place/Group");

                var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                fname = l != null ? l.SelectedValues : string.Empty;
            }
            else if (form.FormTitle == "Project Registration")
            {
                activity = dbContext.Activities.FirstOrDefault(x => x.ActivityName == "Project Registration" && x.FormDataEntry.Guid == model.ProjectId);
                entity = dbContext.EntityTypes.FirstOrDefault(x => x.Name == "Project");
            }

            var project = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == model.ProjectId);

            int entityNumber = 0;
            int? parentEntityNumber = (int?)null;
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

            var formDataEntry = new FormDataEntry()
            {
                Guid = Guid.NewGuid(),
                ActivityId = activity != null ? activity.Id : 0,
                ProjectId = project != null ? project.Id : 0,
                EntityId = entity != null ? entity.Id : 0,
                SubjectId = model.SubjectId,
                Status = model.Status,
                CreatedBy = createdBy.Id,
                CreatedDate = DateTime.UtcNow,
                FormId = form != null ? form.Id : (int?)null,
                ThisUserId = model.ThisUserId,
                EntityNumber = entityNumber,
                ParentEntityNumber = parentEntityNumber,
            };

            dbContext.FormDataEntries.Add(formDataEntry);

            var saveParent = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 3);
            saveParent.SelectedValues = entityNumber.ToString();
            var parentFormDataEntryVariable = new FormDataEntryVariable()
            {
                Guid = Guid.NewGuid(),
                VariableId = saveParent.VariableId,
                SelectedValues = saveParent.SelectedValues,
                SelectedValues_int = saveParent.SelectedValues_int,
                SelectedValues_float = saveParent.SelectedValues_float,
                FormDataEntryId = formDataEntry.Id,
                CreatedBy = createdBy.Id,
                CreatedDate = DateTime.UtcNow,
            };
            dbContext.FormDataEntryVariables.Add(parentFormDataEntryVariable);
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

                dbContext.FormDataEntryVariables.Add(formDataEntryVariable);
            }
            SaveChanges();



            #region save in login table
            if (form.FormTitle == "Place/Group Registration" || form.FormTitle == "Person Registration" || form.FormTitle == "Participant Registration")
            {
                var emali = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38).SelectedValues : string.Empty;
                var username = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40).SelectedValues : string.Empty;
                if (form.FormTitle == "Place/Group Registration")
                {
                    var entityType = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 5) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 5).SelectedValues : string.Empty;
                    if (entityType != "15")
                    {
                        var loginTbl = dbContext.UserLogins.FirstOrDefault(x => x.Id == formDataEntry.ThisUserId);
                        if (loginTbl != null)
                        {
                            loginTbl.Password = null;
                            loginTbl.Salt = null;
                            loginTbl.SecurityQuestionId = null;
                            loginTbl.Answer = null;
                            loginTbl.Status = (int)Core.Enum.Status.InActive;
                            dbContext.SaveChanges();
                        }
                    }
                }
                var authTypeGuid = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51).SelectedValues : new Guid().ToString();
                var authType = dbContext.LoginAuthTypeMasters.FirstOrDefault(x => x.Guid == new Guid(authTypeGuid));
                var userrole = dbContext.Roles.FirstOrDefault(x => x.Id == (int)Core.Enum.RoleTypes.Data_Entry_Supervisor);

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
                entityUserlogin.Status = (int)Core.Enum.Status.InActive;
                entityUserlogin.IsUserApprovedBySystemAdmin = false;
                var savedUser = _userLoginProvider.Create(entityUserlogin);

                var formdataentryUSer = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == formDataEntry.Guid);
                if (formdataentryUSer != null)
                {
                    formdataentryUSer.ThisUserId = savedUser != null ? savedUser.Id : (int?)null;
                    model.ThisUserId = savedUser != null ? savedUser.Id : (int?)null;
                    dbContext.SaveChanges();
                }
            }
            #endregion

            if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Registration))
            {
                #region project default activities and form creation                
                var tenant = dbContext.Tenants.FirstOrDefault(x => x.Guid == model.TenantId);
                var user = dbContext.UserLogins.FirstOrDefault(u => u.Email == "systemadmin@aspree.com");
                var role = dbContext.Roles.FirstOrDefault(v => v.Name == "System Admin");
                this.dbContext.ProjectStaffMemberRoles.Add(new Data.ProjectStaffMemberRole
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

                var userTest = dbContext.UserLogins.FirstOrDefault(u => u.Email == "testsystemadmin@aspree.com");
                this.dbContext.ProjectStaffMemberRoles.Add(new Data.ProjectStaffMemberRole
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

                var addSummaryPageActivity = this.dbContext.AddActivities.Add(new AddActivity()
                {
                    Guid = Guid.NewGuid(),
                    ProjectId = project.Id,

                    CreatedBy = createdBy != null ? createdBy.Id : 1,
                    CreatedDate = DateTime.UtcNow,
                    ActivityId = activity != null ? activity.Id : 1,
                    ActivityCompletedByUser = activityCompletedBy != null ? activityCompletedBy.Id : 1,
                    ActivityDate = DateTime.UtcNow.Date,
                    IsActivityAdded = true,
                    PersonEntityId = formDataEntry.Id,
                });
                SaveChanges();

                var formdataentryUSer = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == formDataEntry.Guid);
                if (formdataentryUSer != null)
                {
                    formdataentryUSer.SubjectId = addSummaryPageActivity.Id;
                    SaveChanges();
                }
                #endregion
            }
            else
            {
                var formdataentryUSer = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == formDataEntry.Guid);
                if (formdataentryUSer != null)
                {
                    formdataentryUSer.SubjectId = model.SubjectId;
                    SaveChanges();
                }
            }




            if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Linkage))
            {
                #region assign project to user
                var formdataentry = dbContext.FormDataEntries.FirstOrDefault(x => x.EntityNumber == parentEntityNumber);
                if (formdataentry != null)
                {
                    UserLogin uLogin = dbContext.UserLogins.FirstOrDefault(x => x.Id == formdataentry.ThisUserId);
                    if (uLogin != null)
                    {
                        bool? isActiveProjectUser = null;
                        DateTime? projectJoinedDate = null;
                        DateTime? projectLeftDate = null;
                        #region Set projectLinkage end date

                        try
                        {
                            var projJoindDate = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Join);
                            string jDate = projJoindDate != null ? projJoindDate.SelectedValues : string.Empty;
                            projectJoinedDate = Convert.ToDateTime(jDate);
                        }
                        catch (Exception joinDete)
                        { }


                        try
                        {
                            var projectLeftDateActiveUser = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Actv);
                            if (projectLeftDateActiveUser != null)
                            {
                                isActiveProjectUser = !string.IsNullOrEmpty(projectLeftDateActiveUser.SelectedValues) ? (projectLeftDateActiveUser.SelectedValues == "1" ? true : false) : false;
                                if (projectLeftDateActiveUser.SelectedValues == "0")
                                {
                                    var projectLeftDateEnd = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.End);
                                    string edate = projectLeftDateEnd != null ? projectLeftDateEnd.SelectedValues : string.Empty;
                                    projectLeftDate = Convert.ToDateTime(edate);
                                }
                            }
                        }
                        catch (Exception exLinkage) { }

                        #endregion


                        var projectStaffMembersRoles = dbContext.UserRoles.Where(x => x.UserId == uLogin.Id).ToList();
                        var proRole = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 56);
                        Role urole = new Role();
                        if (proRole != null)
                        {
                            Guid role = !string.IsNullOrEmpty(proRole.SelectedValues) ? new Guid(proRole.SelectedValues) : Guid.Empty;
                            urole = dbContext.Roles.FirstOrDefault(x => x.Guid == role);
                        }

                        var linkedProjectId = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.LnkPro);
                        Guid linkedProjectGuid = Guid.Parse(linkedProjectId.SelectedValues);
                        FormDataEntry linkedProject = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == linkedProjectGuid);

                        if (urole == null)
                        {
                            isActiveProjectUser = false;
                        }

                        var projectstaffRoles = new ProjectStaffMemberRole()
                        {
                            Guid = Guid.NewGuid(),
                            ProjectId = linkedProject != null ? linkedProject.Id : 1,
                            UserId = uLogin.Id,
                            RoleId = urole != null ? urole.Id : (int)Core.Enum.RoleTypes.Data_Entry,
                            CreatedBy = createdBy.Id,
                            StaffCreatedDate = DateTime.UtcNow,
                            IsActiveProjectUser = isActiveProjectUser,
                            ProjectJoinedDate = projectJoinedDate,
                            ProjectLeftDate = projectLeftDate,
                        };
                        dbContext.ProjectStaffMemberRoles.Add(projectstaffRoles);
                        SaveChanges();
                    }
                }
                #endregion
            }

            FormDataEntryViewModel mdl = new FormDataEntryViewModel();
            mdl.ParticipantId = saveParent.SelectedValues;
            return mdl;
        }

        public FormDataEntryViewModel DeleteById(int id, Guid DeletedBy)
        {
            var deactivatedBy = _userLoginProvider.GetByGuid(DeletedBy);
            var formdataentry = dbContext.FormDataEntries.FirstOrDefault(fs => fs.Id == id);
            if (formdataentry != null)
            {
                dbContext.FormDataEntries.Remove(formdataentry);
                return ToModel(formdataentry);
            }
            return null;
        }

        public FormDataEntryViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var formdataentry = dbContext.FormDataEntries.FirstOrDefault(fs => fs.Guid == guid);
            if (formdataentry != null)
            {
                dbContext.FormDataEntries.Remove(formdataentry);
                return ToModel(formdataentry);
            }
            return null;
        }

        public IEnumerable<FormDataEntryViewModel> GetAll()
        {
            return dbContext.FormDataEntries
                .Select(ToModel)
                .ToList();
        }

        public IEnumerable<FormDataEntryViewModel> GetAll(Guid projectId)
        {
            return dbContext.FormDataEntries
                .Select(ToModel)
                .ToList();
        }

        public FormDataEntryViewModel GetByGuid(Guid guid)
        {
            var formdataentry = dbContext.FormDataEntries
                .FirstOrDefault(fs => fs.Guid == guid);

            if (formdataentry != null)
                return ToModel(formdataentry);

            return null;
        }

        public FormDataEntryViewModel GetById(int id)
        {
            var formdataentry = dbContext.FormDataEntries
                .FirstOrDefault(fs => fs.Id == id);

            if (formdataentry != null)
                return ToModel(formdataentry);

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public FormDataEntryViewModel ToModel(FormDataEntry dataentry)
        {
            List<FormDataEntryVariableViewModel> formVariables = null;
            try
            {
                if (dataentry.Form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Registration))
                {
                    formVariables = new List<FormDataEntryVariableViewModel>();
                    dataentry.FormDataEntryVariables.ToList().ForEach(x =>
                    {
                        formVariables.Add(new FormDataEntryVariableViewModel
                        {
                            VariableName = x.Variable.VariableName,
                            VariableId = x.VariableId,
                            SelectedValues = x.SelectedValues,
                        });
                    });
                }
            }
            catch (Exception exc) { }
            return new FormDataEntryViewModel()
            {
                Guid = dataentry.Guid,
                Id = dataentry.Id,

                ProjectDeployStatus = dataentry.ProjectDeployStatus,
                ProjectDeployedId = dataentry.ProjectDeployedId,
                ProjectDeployedVersion = dataentry.ProjectDeployedVersion,

                FormDataEntryVariable = formVariables,
            };
        }

        public FormDataEntryViewModel Update(FormDataEntryViewModel model)
        {
            var modifiedBy = _userLoginProvider.GetByGuid(model.CreatedBy);
            var form = dbContext.Forms.FirstOrDefault(f => f.Guid == model.FormId);

            var entity = dbContext.EntityTypes.FirstOrDefault(x => x.Guid == model.EntityId);
            var activity = dbContext.Activities.FirstOrDefault(x => x.Guid == model.ActivityId);
            if (form.FormTitle == "Person Registration")
            {
                activity = dbContext.Activities.FirstOrDefault(x => x.ActivityName == "Person Registration" && x.FormDataEntry.Guid == model.ProjectId);
                entity = dbContext.EntityTypes.FirstOrDefault(x => x.Name == "Person");
            }
            else if (form.FormTitle == "Participant Registration")
            {
                activity = dbContext.Activities.FirstOrDefault(x => x.ActivityName == "Participant Registration" && x.FormDataEntry.Guid == model.ProjectId);
                entity = dbContext.EntityTypes.FirstOrDefault(x => x.Name == "Participant");
            }
            else if (form.FormTitle == "Place/Group Registration")
            {
                activity = dbContext.Activities.FirstOrDefault(x => x.ActivityName == "Place/Group Registration" && x.FormDataEntry.Guid == model.ProjectId);
                entity = dbContext.EntityTypes.FirstOrDefault(x => x.Name == "Place/Group");
            }
            else if (form.FormTitle == "Project Registration")
            {
                activity = dbContext.Activities.FirstOrDefault(x => x.ActivityName == "Project Registration" && x.FormDataEntry.Guid == model.ProjectId);
                entity = dbContext.EntityTypes.FirstOrDefault(x => x.Name == "Project");
            }

            var formdataentry = dbContext.FormDataEntries
              .FirstOrDefault(fs => fs.Guid == model.Guid);

            if (formdataentry != null)
            {
                formdataentry.Status = model.Status;
                formdataentry.ModifiedBy = modifiedBy.Id;
                formdataentry.ModifiedDate = DateTime.UtcNow;
            }
            var saveParent = dbContext.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == "EntID" && x.FormDataEntryId == formdataentry.Id);

            IEnumerable<FormDataEntryVariable> formDataVariablesId = this.dbContext.FormDataEntryVariables.Where(x => x.FormDataEntryId == formdataentry.Id);
            dbContext.FormDataEntryVariables.RemoveRange(formDataVariablesId);

            var parentFormDataEntryVariable = new FormDataEntryVariable()
            {
                Guid = Guid.NewGuid(),
                VariableId = saveParent.VariableId,
                SelectedValues = saveParent.SelectedValues,
                SelectedValues_int = saveParent.SelectedValues_int,
                SelectedValues_float = saveParent.SelectedValues_float,
                FormDataEntryId = formdataentry.Id,
                CreatedBy = modifiedBy.Id,
                CreatedDate = DateTime.UtcNow,
                //ParentId = null,
            };
            dbContext.FormDataEntryVariables.Add(parentFormDataEntryVariable);
            SaveChanges();

            string fname = string.Empty;
            string lname = string.Empty;
            if (form.FormTitle == "Person Registration")
            {
                var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                var f = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                lname = l != null ? l.SelectedValues : string.Empty;
                fname = f != null ? f.SelectedValues : string.Empty;
            }
            else if (form.FormTitle == "Participant Registration")
            {
                var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                var f = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                var m = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 17);

                lname = l != null ? l.SelectedValues : string.Empty;
                fname = f != null ? f.SelectedValues : string.Empty + " " + m != null ? m.SelectedValues : string.Empty;
            }
            else if (form.FormTitle == "Place/Group Registration")
            {
                var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                fname = l != null ? l.SelectedValues : string.Empty;
            }

            foreach (var item in model.FormDataEntryVariable)
            {
                //check entity id to skip
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
                    FormDataEntryId = formdataentry.Id,
                    CreatedBy = modifiedBy.Id,
                    CreatedDate = DateTime.UtcNow,
                    ParentId = parentFormDataEntryVariable.Id,
                };
                dbContext.FormDataEntryVariables.Add(formDataEntryVariable);
            }

            if (form.FormTitle == "Place/Group Registration" || form.FormTitle == "Person Registration" || form.FormTitle == "Participant Registration")
            {
                var emali = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38).SelectedValues : string.Empty;
                var username = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40).SelectedValues : string.Empty;
                var authTypeGuid = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51).SelectedValues : Guid.Empty.ToString();
                if (!string.IsNullOrEmpty(authTypeGuid) && !string.IsNullOrEmpty(username))
                {


                    #region Is API Access to export data
                    bool isApiAccess = false;
                    try
                    {
                        if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                            || form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration))
                        {
                            Variable authVariable = dbContext.Variables.FirstOrDefault(x => x.VariableName == "AuthenticationMethod");
                            Variable isApiAccessEnabled = dbContext.Variables.FirstOrDefault(x => x.VariableName == "ApiAccessEnabled");
                            LoginAuthTypeMaster loginAuthTypeMaster = dbContext.LoginAuthTypeMasters.FirstOrDefault(x => x.AuthTypeName.ToLower() == Core.Enum.AuthenticationTypes.Local_Password.ToString().ToLower().Replace("_", " "));
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
                                var variableEntType = dbContext.Variables.FirstOrDefault(x => x.VariableName == "EntType");
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
                                        //isApiAccess = isApiAccessEnabledData != null ? Convert.ToBoolean(isApiAccessEnabledData.SelectedValues) : false;
                                        isApiAccess = isApiAccessEnabledData != null ? (isApiAccessEnabledData.SelectedValues == "1" ? true : false) : false;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exc) { Console.WriteLine(exc); }
                    #endregion



                    var logintable = dbContext.UserLogins.FirstOrDefault(x => x.Id == formdataentry.ThisUserId);
                    if (logintable == null)
                    {
                        //save in user table
                        var authType = dbContext.LoginAuthTypeMasters.FirstOrDefault(x => x.Guid == new Guid(authTypeGuid));

                        var userrole = dbContext.Roles.FirstOrDefault(x => x.Id == (int)Core.Enum.RoleTypes.Data_Entry_Supervisor);

                        #region is user approved by system admin
                        bool isUserApprovedBySystemAdmin = false;
                        FormDataEntryVariableViewModel isUserApprovedBySystemAdminModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)Core.Enum.DefaultsVariables.SysAppr);
                        if (isUserApprovedBySystemAdminModel != null)
                        {
                            int isUserApprovedBySystemAdminInt = !string.IsNullOrEmpty(isUserApprovedBySystemAdminModel.SelectedValues) ? Convert.ToInt32(isUserApprovedBySystemAdminModel.SelectedValues) : 0;
                            isUserApprovedBySystemAdmin = isUserApprovedBySystemAdminInt == 1 ? true : false;
                        }
                        #endregion


                        UserLoginViewModel entityUserlogin = new UserLoginViewModel();
                        entityUserlogin.FirstName = fname;
                        entityUserlogin.LastName = lname;
                        entityUserlogin.Email = emali;
                        entityUserlogin.Mobile = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 39) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 39).SelectedValues : null;
                        entityUserlogin.TenantId = model.TenantId != null ? (Guid)model.TenantId : new Guid();
                        entityUserlogin.AuthTypeId = authType != null ? authType.Id : 0;
                        entityUserlogin.CreatedBy = model.CreatedBy;
                        entityUserlogin.UserTypeId = (int)Core.Enum.UsersLoginType.Entity;
                        entityUserlogin.TempGuid = Guid.NewGuid();
                        entityUserlogin.RoleId = userrole != null ? userrole.Guid : new Guid();
                        entityUserlogin.UserName = username;
                        entityUserlogin.Status = (int)Core.Enum.Status.InActive;
                        entityUserlogin.IsUserApprovedBySystemAdmin = isUserApprovedBySystemAdmin;
                        _userLoginProvider.Create(entityUserlogin);

                        var formdataentryUSer = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == formdataentry.Guid);
                        if (formdataentryUSer != null)
                        {
                            var userlogin = dbContext.UserLogins.FirstOrDefault(x => x.UserName == username && x.AuthTypeId == entityUserlogin.AuthTypeId);
                            formdataentryUSer.ThisUserId = userlogin != null ? userlogin.Id : (int?)null;
                            dbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        var authType = dbContext.LoginAuthTypeMasters.FirstOrDefault(x => x.Guid == new Guid(authTypeGuid));
                        var userrole = dbContext.Roles.FirstOrDefault(x => x.Id == (int)Core.Enum.RoleTypes.Data_Entry_Supervisor);

                        #region Default Role
                        string personRole = string.Empty;
                        personRole = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 43) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 43).SelectedValues : string.Empty;  //SysRole
                        int roletypeid = !string.IsNullOrEmpty(personRole) ? Convert.ToInt32(personRole) : 0;
                        if (roletypeid == 1)
                            userrole = _dbContext.Roles.FirstOrDefault(x => x.Id == (int)Core.Enum.RoleTypes.System_Admin);
                        else
                            userrole = _dbContext.Roles.FirstOrDefault(x => x.Id == (int)Core.Enum.RoleTypes.Data_Entry);
                        #endregion




                        UserLoginViewModel entityUserlogin = new UserLoginViewModel();
                        FormDataEntryVariableViewModel userStatusModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)Core.Enum.DefaultsVariables.Active);
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
                            isUserApprovedBySystemAdminModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)Core.Enum.DefaultsVariables.Active);
                            int isUserApprovedBySystemAdminInt = !string.IsNullOrEmpty(isUserApprovedBySystemAdminModel.SelectedValues) ? Convert.ToInt32(isUserApprovedBySystemAdminModel.SelectedValues) : 0;
                            isUserApprovedBySystemAdmin = isUserApprovedBySystemAdminInt == 1 ? true : false; ;
                        }

                        #endregion
                        entityUserlogin.FirstName = fname;
                        entityUserlogin.LastName = lname;
                        entityUserlogin.Email = emali;
                        entityUserlogin.UserName = username;
                        entityUserlogin.Mobile = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 39) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 39).SelectedValues : null;
                        entityUserlogin.TenantId = model.TenantId != null ? (Guid)model.TenantId : new Guid();
                        entityUserlogin.AuthTypeId = authType != null ? authType.Id : 0;

                        entityUserlogin.UserTypeId = (int)Core.Enum.UsersLoginType.Entity;
                        entityUserlogin.TempGuid = Guid.NewGuid();
                        entityUserlogin.RoleId = userrole != null ? userrole.Guid : new Guid();
                        entityUserlogin.ModifiedBy = model.CreatedBy;
                        entityUserlogin.Guid = logintable.Guid;
                        entityUserlogin.Id = logintable.Id;
                        entityUserlogin.IsApiAccessEnabled = isApiAccess;
                        entityUserlogin.Status = userStatus == 1 ? (int)Core.Enum.Status.Active : (int)Core.Enum.Status.InActive;
                        entityUserlogin.IsUserApprovedBySystemAdmin = isUserApprovedBySystemAdmin;
                        _userLoginProvider.Update(entityUserlogin);
                        var login = dbContext.UserLogins.FirstOrDefault(x => x.Guid == logintable.Guid);
                        if (login != null)
                        {
                            login.AuthTypeId = authType != null ? authType.Id : 0;
                            dbContext.SaveChanges();
                        }
                    }
                }

                if (form.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
                {
                    #region place/group form login update
                    var EntGrpVar = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 5) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 5).SelectedValues : string.Empty;
                    if (EntGrpVar != "15")
                    {
                        var loginTbl = _dbContext.UserLogins.FirstOrDefault(x => x.Id == formdataentry.ThisUserId);
                        if (loginTbl != null)
                        {
                            loginTbl.Password = null;
                            loginTbl.Salt = null;
                            loginTbl.SecurityQuestionId = null;
                            loginTbl.Answer = null;
                            loginTbl.Status = (int)Core.Enum.Status.InActive;
                            loginTbl.IsUserApprovedBySystemAdmin = false;
                            _dbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        #region place form login update
                        try
                        {
                            FormDataEntryVariableViewModel isUserApprovedBySystemAdminModel1 = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Active);
                            int isUserApprovedBySystemAdminInt = !string.IsNullOrEmpty(isUserApprovedBySystemAdminModel1.SelectedValues) ? Convert.ToInt32(isUserApprovedBySystemAdminModel1.SelectedValues) : 0;

                            bool isUserApprovedBySystemAdmin1 = isUserApprovedBySystemAdminInt == 1 ? true : false;

                            int userLoginStatus = (int)Core.Enum.Status.InActive;
                            FormDataEntryVariableViewModel isActiveUserModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)Core.Enum.DefaultsVariables.Active);
                            if (isActiveUserModel != null)
                            {
                                int isActiveUserModelInt = !string.IsNullOrEmpty(isActiveUserModel.SelectedValues) ? Convert.ToInt32(isActiveUserModel.SelectedValues) : 0;
                                userLoginStatus = isActiveUserModelInt == 1 ? (int)Core.Enum.Status.Active : (int)Core.Enum.Status.InActive;
                            }
                            var loginTbl = _dbContext.UserLogins.FirstOrDefault(x => x.Id == formdataentry.ThisUserId);
                            if (loginTbl != null)
                            {
                                loginTbl.Status = userLoginStatus;
                                loginTbl.IsUserApprovedBySystemAdmin = isUserApprovedBySystemAdmin1;
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
                    bool isUserApprovedBySystemAdmin1 = false;
                    int userLoginStatus = (int)Core.Enum.Status.InActive;
                    FormDataEntryVariableViewModel isUserApprovedBySystemAdminModel1 = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)Core.Enum.DefaultsVariables.SysAppr);
                    if (isUserApprovedBySystemAdminModel1 != null)
                    {
                        int isUserApprovedBySystemAdminInt = !string.IsNullOrEmpty(isUserApprovedBySystemAdminModel1.SelectedValues) ? Convert.ToInt32(isUserApprovedBySystemAdminModel1.SelectedValues) : 0;
                        isUserApprovedBySystemAdmin1 = isUserApprovedBySystemAdminInt == 1 ? true : false;
                    }
                    FormDataEntryVariableViewModel isActiveUserModel = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)Core.Enum.DefaultsVariables.Active);
                    if (isActiveUserModel != null)
                    {
                        int isActiveUserModelInt = !string.IsNullOrEmpty(isActiveUserModel.SelectedValues) ? Convert.ToInt32(isActiveUserModel.SelectedValues) : 0;
                        userLoginStatus = isActiveUserModelInt == 1 ? (int)Core.Enum.Status.Active : (int)Core.Enum.Status.InActive;
                    }

                    var loginTbl = _dbContext.UserLogins.FirstOrDefault(x => x.Id == formdataentry.ThisUserId);
                    if (loginTbl != null)
                    {
                        loginTbl.Status = userLoginStatus;
                        loginTbl.IsUserApprovedBySystemAdmin = isUserApprovedBySystemAdmin1;
                        _dbContext.SaveChanges();
                    }
                    #endregion
                }
                else if (form.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
                {
                    #region participant form login update
                    try
                    {
                        FormDataEntryVariableViewModel isUserApprovedBySystemAdminModel1 = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Active);
                        int isUserApprovedBySystemAdminInt = !string.IsNullOrEmpty(isUserApprovedBySystemAdminModel1.SelectedValues) ? Convert.ToInt32(isUserApprovedBySystemAdminModel1.SelectedValues) : 0;

                        bool isUserApprovedBySystemAdmin1 = isUserApprovedBySystemAdminInt == 1 ? true : false;

                        var loginTbl = _dbContext.UserLogins.FirstOrDefault(x => x.Id == formdataentry.ThisUserId);
                        if (loginTbl != null)
                        {
                            loginTbl.Status = isUserApprovedBySystemAdmin1 ? (int)Core.Enum.Status.Active : (int)Core.Enum.Status.InActive;
                            loginTbl.IsUserApprovedBySystemAdmin = isUserApprovedBySystemAdmin1;
                            _dbContext.SaveChanges();
                        }
                    }
                    catch (Exception excActive) { Console.WriteLine(excActive.Message); }
                    #endregion
                }
            }

            if (form.FormTitle == EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Project_Linkage))
            {
                #region assig project to user
                var formdataentryProjectLinkage = dbContext.FormDataEntries.FirstOrDefault(x => x.EntityNumber == formdataentry.ParentEntityNumber);
                if (formdataentryProjectLinkage != null)
                {
                    UserLogin uLogin = dbContext.UserLogins.FirstOrDefault(x => x.Id == formdataentryProjectLinkage.ThisUserId);
                    if (uLogin != null)
                    {
                        #region Set projectLinkage end date
                        bool? isActiveProjectUser = null;
                        DateTime? projectJoinedDate = null;
                        DateTime? projectLeftDate = null;
                        try
                        {
                            var projJoindDate = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Join);
                            string jDate = projJoindDate != null ? projJoindDate.SelectedValues : string.Empty;
                            projectJoinedDate = Convert.ToDateTime(jDate);
                        }
                        catch (Exception joinDete)
                        { }
                        try
                        {
                            var projectLeftDateActiveUser = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.Actv);
                            if (projectLeftDateActiveUser != null)
                            {
                                isActiveProjectUser = !string.IsNullOrEmpty(projectLeftDateActiveUser.SelectedValues) ? (projectLeftDateActiveUser.SelectedValues == "1" ? true : false) : false;
                                if (projectLeftDateActiveUser.SelectedValues == "0")
                                {
                                    var projectLeftDateEnd = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.End);
                                    string edate = projectLeftDateEnd != null ? projectLeftDateEnd.SelectedValues : string.Empty;
                                    projectLeftDate = Convert.ToDateTime(edate);
                                }
                            }
                        }
                        catch (Exception exLinkage) { }
                        #endregion



                        var projectStaffMembersRoles = dbContext.UserRoles.Where(x => x.UserId == uLogin.Id).ToList();
                        var proRole = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.ProRole);
                        Role urole = new Role();
                        if (proRole != null)
                        {
                            Guid role = !string.IsNullOrEmpty(proRole.SelectedValues) ? new Guid(proRole.SelectedValues) : Guid.Empty;
                            urole = dbContext.Roles.FirstOrDefault(x => x.Guid == role);
                        }

                        var linkedProjectId = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == (int)DefaultsVariables.LnkPro);
                        Guid linkedProjectGuid = Guid.Parse(linkedProjectId.SelectedValues);
                        FormDataEntry linkedProject = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == linkedProjectGuid);

                        IEnumerable<ProjectStaffMemberRole> projectStaffMemberRoles = this.dbContext.ProjectStaffMemberRoles.Where(x => x.UserId == uLogin.Id && x.FormDataEntry.Guid == linkedProject.Guid);
                        dbContext.ProjectStaffMemberRoles.RemoveRange(projectStaffMemberRoles);
                        SaveChanges();

                        if (urole == null)
                        {
                            isActiveProjectUser = false;
                        }

                        var projectstaffRoles = new ProjectStaffMemberRole()
                        {
                            Guid = Guid.NewGuid(),
                            ProjectId = linkedProject != null ? linkedProject.Id : 1,
                            UserId = uLogin.Id,
                            RoleId = urole != null ? urole.Id : (int)Core.Enum.RoleTypes.Data_Entry,
                            CreatedBy = modifiedBy.Id,
                            StaffCreatedDate = DateTime.UtcNow,
                            IsActiveProjectUser = isActiveProjectUser,
                            ProjectJoinedDate = projectJoinedDate,
                            ProjectLeftDate = projectLeftDate,
                        };
                        dbContext.ProjectStaffMemberRoles.Add(projectstaffRoles);
                        SaveChanges();
                    }
                }
                #endregion
            }

            SaveChanges();
            FormDataEntryViewModel mdl = new FormDataEntryViewModel();
            mdl.ParticipantId = saveParent.SelectedValues;
            mdl.ThisUserId = formdataentry.ThisUserId;
            return mdl;
        }


        public List<List<FormDataEntryVariableViewModel>> SearchVariables(SearchPageVariableViewModel model, string source = null)
        {
            Core.ViewModels.NewCategory.WriteLog("start SearchVariables in provider");
            var form = dbContext.Forms.FirstOrDefault(x => x.Guid == model.FormId);

            IQueryable<FormDataEntryVariable> dataEntry = dbContext.FormDataEntryVariables.Where(x => x.FormDataEntry.Form.Guid == model.FormId);
            if (form.FormTitle == "Person Registration" || form.FormTitle == "Place/Group Registration" || form.FormTitle == "Project Registration")
            {
                dataEntry = dbContext.FormDataEntryVariables.Where(x => x.FormDataEntry.Form.FormTitle == form.FormTitle);
            }
            if (form != null)
            {
                model.FormTitle = form.FormTitle;
                var title = form.FormTitle;
                if (title == "Participant Registration")
                {
                    dataEntry = dbContext.FormDataEntryVariables.Where(x => x.FormDataEntry.Form.Guid == model.FormId);

                    var project = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == model.ProjectId);
                    if (project != null)
                    {
                        #region Project Recruitment logic
                        var recStartDate = project.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.RecruitStart.ToString());
                        var recEndDate = project.FormDataEntryVariables.FirstOrDefault(x => x.Variable.VariableName == DefaultsVariables.RecruitEnd.ToString());
                        DateTime? startDateString = null;
                        DateTime? endDateString = null;
                        try { startDateString = recStartDate != null ? Convert.ToDateTime(recStartDate.SelectedValues) : (DateTime?)null; } catch (Exception ex) { }
                        try { endDateString = recEndDate != null ? Convert.ToDateTime(recEndDate.SelectedValues) : (DateTime?)null; } catch (Exception ex) { }
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
                        #endregion
                    }
                }
            }
            Core.ViewModels.NewCategory.WriteLog("start SearchVariables in provider");
            var searchResult = dbContext.FormDataEntryVariables.ToList();

            var participantId = model.SearchVariables.FirstOrDefault(x => x.Key == 3);
            if (participantId.Value != "" && participantId.Value != null)
            {
                searchResult = null;
                var d = dataEntry.Where(x =>
                x.VariableId == participantId.Key &&
                x.SelectedValues == participantId.Value
                ).FirstOrDefault();
                if (d != null)
                {
                    searchResult = dataEntry.Where(x =>
                    x.FormDataEntryId == d.FormDataEntryId
                    ).ToList();
                }
                Core.ViewModels.NewCategory.WriteLog("start SearchVariables in provider");
            }
            else
            {
                searchResult = null;
                bool isFound = true;

                List<int> formDataEntryId = new List<int>();
                var tempFormDataEntryId = 0;

                List<FormDataEntryVariableViewModel> fromModel = new List<FormDataEntryVariableViewModel>();
                var result = new List<int?>();
                foreach (var item in model.SearchVariables)
                {
                    if (item.Key != 3)
                    {
                        result = dataEntry.Where(x => x.VariableId == item.Key && x.SelectedValues == item.Value).Select(x => x.ParentId).ToList();
                        break;
                    }
                }

                foreach (var parentId in result)
                {
                    if (parentId != null)
                    {
                        var parentData = dataEntry.Where(x => x.ParentId == parentId).Select(x => x.FormDataEntryId).FirstOrDefault();
                        var formVariables = dataEntry.Where(x => x.FormDataEntryId == parentData);
                        foreach (var item in model.SearchVariables)
                        {
                            if (item.Key != 3)
                            {
                                var searchVar = formVariables.Where(x => x.VariableId == item.Key && x.SelectedValues == item.Value).ToList(); //&& x.ParentId == parentId
                                if (searchVar.Count == 0)
                                {
                                    isFound = false;
                                    break;
                                }
                                else
                                {
                                    isFound = true;
                                    tempFormDataEntryId = searchVar.FirstOrDefault().FormDataEntryId;
                                }
                            }
                        }
                        if (isFound)
                        {
                            formDataEntryId.Add(tempFormDataEntryId);
                        }

                    }
                }

                searchResult = dataEntry.Where(cus => formDataEntryId.Contains(cus.FormDataEntryId)).ToList();

                Core.ViewModels.NewCategory.WriteLog("start SearchVariables in provider");
            }


            #region Search entity in Mongo database
            if (model.FormTitle != EnumHelpers.GetEnumDescription(Core.Enum.DefaultFormName.Participant_Registration))
            {
                if (searchResult == null && source == null)
                {
                    return SearchEntityInMONGODB(model, "SQL");
                }
                else if (searchResult.Count() == 0 && source == null)
                {
                    return SearchEntityInMONGODB(model, "SQL");
                }
            }
            #endregion



            if (searchResult == null)
            {
                return null;
            }
            var resultVariables = searchResult.Select(ToSearchModel)
                .ToList();
            Core.ViewModels.NewCategory.WriteLog("start SearchVariables in provider");
            var groupedVariableList = resultVariables
             .GroupBy(u => u.FormDataEntryId)
             .Select(grp => grp.ToList())
             .ToList();
            Core.ViewModels.NewCategory.WriteLog("end SearchVariables in provider " + groupedVariableList);
            return groupedVariableList;
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

                string variableName = dataentry.Variable != null ? dataentry.Variable.VariableName : string.Empty;

                if (variableName == Core.Enum.DefaultsVariables.AuthenticationMethod.ToString())
                {
                    Guid authTypeGuid = Guid.Empty;
                    try { authTypeGuid = new Guid(dataentry.SelectedValues); } catch (Exception ex) { }

                    var authType = dbContext.LoginAuthTypeMasters.FirstOrDefault(x => x.Guid == authTypeGuid);
                    dataentry.SelectedValues = authType != null ? authType.AuthTypeName : string.Empty;
                }
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
                    SelectedValues_int = dataentry.FormDataEntry.ThisUserId,
                };
            }
            else
            {
                return null;
            }
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
            return (int)(cnt + 1);
        }
        public int GenerateRandomNo6Digit()
        {
            Random _rdm = new Random();
            var rno = 0;
            var IdList = dbContext.FormDataEntries.Select(x => x.EntityNumber.ToString()).ToList();
            var IdListMongo = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().Select(x => Convert.ToString(x.EntityNumber)).ToList();

            IdList = IdList.Union(IdListMongo).ToList();

            do
            {
                rno = _rdm.Next(100000, 900000);
            } while (IdList.Contains(rno.ToString()));

            return rno;
        }
        public IEnumerable<FormDataEntryVariableViewModel> GetFormDataEntryByEntId(Guid projectId, Guid formId, string entId)
        {
            int formDataEntryid = 0;
            var formDataEntry = dbContext.FormDataEntryVariables.FirstOrDefault(et => et.VariableId == 3 && et.SelectedValues == entId);
            if (formDataEntry != null)
            {
                formDataEntryid = formDataEntry.FormDataEntryId;
            }
            return dbContext.FormDataEntryVariables
                .Where(et => et.FormDataEntryId == formDataEntryid)
                .Select(ToSearchModel)
                .ToList();
        }


        public UserLoginViewModel CheckDuplicateUsername(string userName, string emial, Guid authType, Guid? userguid)
        {
            if (userguid == null)
            {
                var user = dbContext.UserLogins.FirstOrDefault(et => et.Email.ToLower() == emial.ToLower() && et.UserName.ToLower() == userName.ToLower() && et.LoginAuthTypeMaster.Guid == authType);
                if (user != null)
                {
                    return _userLoginProvider.ToModel(user);
                }
            }
            else
            {
                var user = dbContext.UserLogins.FirstOrDefault(et => et.Email.ToLower() == emial.ToLower() && et.UserName.ToLower() == userName.ToLower() && et.LoginAuthTypeMaster.Guid == authType && et.Guid != userguid);
                if (user != null)
                {
                    return _userLoginProvider.ToModel(user);
                }
            }
            return null;
        }

        public Guid? LocalPasswordGuid()
        {
            var localpwd = dbContext.LoginAuthTypeMasters.FirstOrDefault(x => x.AuthTypeName == "Local password");
            return localpwd != null ? localpwd.Guid : (Guid?)null;
        }

        public Guid? AuthTypeLocalMailsend(Guid dataentryguid)
        {
            var dataentryvar = dbContext.FormDataEntryVariables.Where(x => x.FormDataEntry.Guid == dataentryguid);
            var isAuthenticationMethod = dataentryvar.FirstOrDefault(x => x.Variable.VariableName == "AuthenticationMethod");
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
        public IEnumerable<FormDataEntryProjectsViewModel> GetAllFormDataEntryProjects(Guid projectId, Guid formId)
        {
            FormDataEntryProjectsViewModel model = new FormDataEntryProjectsViewModel();
            List<FormDataEntryProjectsViewModel> projectList = new List<FormDataEntryProjectsViewModel>();
            int formDataEntryid = 0;
            var formDataEntry = dbContext.FormDataEntries.Where(et => et.Form.Guid == formId);
            if (formDataEntry != null)
            {
                foreach (var proj in formDataEntry)
                {
                    model = new FormDataEntryProjectsViewModel();
                    formDataEntryid = proj.Id;
                    var formDataEntryVariables = dbContext.FormDataEntryVariables.Where(et => et.FormDataEntryId == formDataEntryid);

                    model.ProjectName = formDataEntryVariables.Where(x => x.Variable.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();

                    var ProjectSubtype = formDataEntryVariables.Where(x => x.Variable.VariableName == "ProSType").Select(x => x.SelectedValues).FirstOrDefault();
                    {
                        var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == "ProSType");
                        var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                        var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                        for (int i = 0; i < variableValues.Count; i++)
                        {
                            if (variableValues[i] == ProjectSubtype)
                            {
                                model.ProjectSubtype = variableValuesDesc[i];
                            }
                        }
                    }
                    var ConfData = formDataEntryVariables.Where(x => x.Variable.VariableName == "ConfData").Select(x => x.SelectedValues).FirstOrDefault();
                    {
                        var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == "ConfData");
                        var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                        var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                        for (int i = 0; i < variableValues.Count; i++)
                        {
                            if (variableValues[i] == ConfData)
                            {
                                model.ConfData = variableValuesDesc[i];
                            }
                        }
                    }

                    var CnstModel = formDataEntryVariables.Where(x => x.Variable.VariableName == "CnstModel").Select(x => x.SelectedValues).FirstOrDefault();
                    {
                        var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == "CnstModel");
                        var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                        var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                        for (int i = 0; i < variableValues.Count; i++)
                        {
                            if (variableValues[i] == CnstModel)
                            {
                                model.CnstModel = variableValuesDesc[i];
                            }
                        }
                    }
                    var Ethics = formDataEntryVariables.Where(x => x.Variable.VariableName == "Ethics").Select(x => x.SelectedValues).FirstOrDefault();
                    {
                        var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == "Ethics");
                        var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                        var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                        for (int i = 0; i < variableValues.Count; i++)
                        {
                            if (variableValues[i] == Ethics)
                            {
                                model.Ethics = variableValuesDesc[i];
                            }
                        }
                    }
                    var DataStore = formDataEntryVariables.Where(x => x.Variable.VariableName == "DataStore").Select(x => x.SelectedValues).FirstOrDefault();
                    {
                        var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == "DataStore");
                        var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                        var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                        for (int i = 0; i < variableValues.Count; i++)
                        {
                            if (variableValues[i] == DataStore)
                            {
                                model.DataStore = variableValuesDesc[i];
                            }
                        }
                    }
                    model.ProDt = formDataEntryVariables.Where(x => x.Variable.VariableName == "ProDt").Select(x => x.SelectedValues).FirstOrDefault();

                    projectList.Add(model);
                }
            }
            return projectList;
        }

        public bool checkEmailExistUserLogin(string email, Guid? userid)
        {
            if (userid != null)
            {
                var userlogin = dbContext.UserLogins.FirstOrDefault(x => x.UserName.ToLower() == email.ToLower() && x.Guid != userid);
                if (userlogin == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                var userlogin = dbContext.UserLogins.FirstOrDefault(x => x.UserName.ToLower() == email.ToLower());
                if (userlogin == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool checkUsernameExistUserLogin(string username, Guid authType, Guid? userid)
        {
            if (userid != null)
            {
                var userlogin = dbContext.UserLogins.FirstOrDefault(x => x.UserName.ToLower() == username.ToLower() && x.LoginAuthTypeMaster.Guid == authType && x.Guid != userid);
                if (userlogin == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                var userlogin = dbContext.UserLogins.FirstOrDefault(x => x.UserName.ToLower() == username.ToLower() && x.LoginAuthTypeMaster.Guid == authType);
                if (userlogin == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public IEnumerable<FormDataEntryProjectsViewModel> GetAllDataEntryProjectList()
        {
            FormDataEntryProjectsViewModel model = new FormDataEntryProjectsViewModel();
            List<FormDataEntryProjectsViewModel> projectList = new List<FormDataEntryProjectsViewModel>();
            int formDataEntryid = 0;
            var formDataEntry = dbContext.FormDataEntries.Where(x => x.Form.FormTitle == "Project Registration" || x.FormId == null);
            if (formDataEntry != null)
            {
                formDataEntry.ToList().ForEach(proj =>
                {
                    model = new FormDataEntryProjectsViewModel();
                    formDataEntryid = proj.Id;

                    var formDataEntryVariables = proj.FormDataEntryVariables;

                    model.ProjectName = formDataEntryVariables.Where(x => x.Variable.VariableName == DefaultsVariables.Name.ToString()).Select(x => x.SelectedValues).FirstOrDefault();

                    var ProjectSubtype = formDataEntryVariables.Where(x => x.Variable.VariableName == DefaultsVariables.ProSType.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                    {
                        var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == DefaultsVariables.ProSType.ToString());
                        var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                        var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                        for (int i = 0; i < variableValues.Count; i++)
                        {
                            if (variableValues[i] == ProjectSubtype)
                            {
                                model.ProjectSubtype = variableValuesDesc[i];
                            }
                        }
                    }
                    var ConfData = formDataEntryVariables.Where(x => x.Variable.VariableName == DefaultsVariables.ConfData.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                    {
                        var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == DefaultsVariables.ConfData.ToString());
                        var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                        var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                        for (int i = 0; i < variableValues.Count; i++)
                        {
                            if (variableValues[i] == ConfData)
                            {
                                model.ConfData = variableValuesDesc[i];
                            }
                        }
                    }
                    var CnstModel = formDataEntryVariables.Where(x => x.Variable.VariableName == DefaultsVariables.CnstModel.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                    {
                        var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == DefaultsVariables.CnstModel.ToString());
                        var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                        var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                        for (int i = 0; i < variableValues.Count; i++)
                        {
                            if (variableValues[i] == CnstModel)
                            {
                                model.CnstModel = variableValuesDesc[i];
                            }
                        }
                    }
                    var Ethics = formDataEntryVariables.Where(x => x.Variable.VariableName == DefaultsVariables.Ethics.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                    {
                        var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == DefaultsVariables.Ethics.ToString());
                        var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                        var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                        for (int i = 0; i < variableValues.Count; i++)
                        {
                            if (variableValues[i] == Ethics)
                            {
                                model.Ethics = variableValuesDesc[i];
                            }
                        }
                    }
                    var DataStore = formDataEntryVariables.Where(x => x.Variable.VariableName == DefaultsVariables.DataStore.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                    {
                        var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == DefaultsVariables.DataStore.ToString());
                        var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                        var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                        for (int i = 0; i < variableValues.Count; i++)
                        {
                            if (variableValues[i] == DataStore)
                            {
                                model.DataStore = variableValuesDesc[i];
                            }
                        }
                    }
                    model.ProDt = formDataEntryVariables.Where(x => x.Variable.VariableName == DefaultsVariables.ProDt.ToString()).Select(x => x.SelectedValues).FirstOrDefault();

                    model.ProjectDisplayName = formDataEntryVariables.Where(x => x.Variable.VariableName == DefaultsVariables.ProjectDisplayName.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                    model.ProjectColor = formDataEntryVariables.Where(x => x.Variable.VariableName == DefaultsVariables.ProjectColor.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                    model.ProjectLogo = formDataEntryVariables.Where(x => x.Variable.VariableName == DefaultsVariables.ProjectLogo.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                    model.ProjectDisplayNameTextColour = formDataEntryVariables.Where(x => x.Variable.VariableName == DefaultsVariables.ProjectDisplayNameTextColour.ToString()).Select(x => x.SelectedValues).FirstOrDefault();
                    model.ProjectStaffMembersRoles = ToNewProjectStaffMemberRoleViewModelList(proj.ProjectStaffMemberRoles.ToList());
                    model.Guid = proj.Guid;
                    projectList.Add(model);
                });
            }
            return projectList;
        }


        public List<NewProjectStaffMemberRoleViewModel> ToNewProjectStaffMemberRoleViewModelList(List<ProjectStaffMemberRole> projectStaffMemberRoles)
        {
            List<NewProjectStaffMemberRoleViewModel> projectStaffMemberRoleViewModelList = new List<NewProjectStaffMemberRoleViewModel>();
            NewProjectStaffMemberRoleViewModel mdl = new NewProjectStaffMemberRoleViewModel();

            Guid uGuid = Guid.Empty;
            projectStaffMemberRoles.Where(x => x.Role.DateDeactivated == null).ToList().ForEach(c =>
               {
                   mdl = new NewProjectStaffMemberRoleViewModel();
                   if (c.IsActiveProjectUser == true)
                   {
                       var userProjectJoinDate = Convert.ToDateTime(c.ProjectJoinedDate);
                       if (DateTime.Now.Date >= userProjectJoinDate.Date)
                       {
                           mdl.Id = c.Id;
                           mdl.ProjectGuid = c.FormDataEntry.Guid;
                           mdl.UserGuid = c.UserLogin != null ? c.UserLogin.Guid : Guid.Empty;
                           mdl.RoleGuid = c.Role.Guid;
                           mdl.ProjectUserName = c.UserLogin != null ? c.UserLogin.FirstName + " " + c.UserLogin.LastName : "";
                           mdl.ProjectUserRoleName = c.Role.Name;
                           mdl.StaffCreatedDate = c.StaffCreatedDate;
                           mdl.StaffCreatedDateString = c.StaffCreatedDate != null ? c.StaffCreatedDate.Value.ToString("yyyy-MM-dd hh:mm:ss") : "";
                           mdl.Guid = c.Guid;
                           projectStaffMemberRoleViewModelList.Add(mdl);
                       }
                   }
                   else
                   {
                       var userProjectJoinDate = Convert.ToDateTime(c.ProjectJoinedDate);
                       var userProjectLeftDate = Convert.ToDateTime(c.ProjectLeftDate);
                       if (DateTime.Now.Date >= userProjectJoinDate.Date && DateTime.Now.Date < userProjectLeftDate.Date)
                       {
                           mdl.Id = c.Id;
                           mdl.ProjectGuid = c.FormDataEntry.Guid;
                           mdl.UserGuid = c.UserLogin != null ? c.UserLogin.Guid : Guid.Empty;
                           mdl.RoleGuid = c.Role.Guid;
                           mdl.ProjectUserName = c.UserLogin != null ? c.UserLogin.FirstName + " " + c.UserLogin.LastName : "";
                           mdl.ProjectUserRoleName = c.Role.Name;
                           mdl.StaffCreatedDate = c.StaffCreatedDate;
                           mdl.StaffCreatedDateString = c.StaffCreatedDate != null ? c.StaffCreatedDate.Value.ToString("yyyy-MM-dd hh:mm:ss") : "";
                           mdl.Guid = c.Guid;
                           projectStaffMemberRoleViewModelList.Add(mdl);
                       }
                   }
               });
            return projectStaffMemberRoleViewModelList;
        }

        public IEnumerable<FormDataEntryProjectsViewModel> TestEnvironment_GetAllDataEntryProjectList(Guid loggedInUser)
        {
            FormDataEntryProjectsViewModel model = new FormDataEntryProjectsViewModel();
            List<FormDataEntryProjectsViewModel> projectList = new List<FormDataEntryProjectsViewModel>();
            int formDataEntryid = 0;
            List<ProjectBuilderJSON> projectJson = dbContext.ProjectBuilderJSONs.Where(x => x.ProjectType == (int)Core.Enum.ActivityDeploymentStatus.Pushed).ToList();
            List<Guid> projectIdList = new List<Guid>();
            foreach (ProjectBuilderJSON projectBuilder in projectJson)
            {
                DeployProjectJsonViewModel deserialized = new System.Web.Script.Serialization.JavaScriptSerializer().
                        Deserialize<DeployProjectJsonViewModel>(projectBuilder.ProjectBuilderJSONData);
                if (deserialized.projectStaffMemberList.Count() > 0)
                {
                    foreach (var project in deserialized.projectStaffMemberList)
                    {
                        projectIdList.Add(project.ProjectGuid);
                    }
                }
            }
            IQueryable<FormDataEntry> formDataEntry = dbContext.FormDataEntries.Where(x => projectIdList.Contains(x.Guid));

            if (formDataEntry != null)
            {
                UserLogin userLogin = dbContext.UserLogins.FirstOrDefault(x => x.Guid == loggedInUser);
                int userid = userLogin != null ? userLogin.Id : 0;

                foreach (var proj in formDataEntry)
                {
                    if (!userLogin.UserRoles.Any(x => x.Role.Name == "System Admin"))
                    {
                        if (!proj.ProjectStaffMemberRoles.Any(x => x.UserId == userid))
                        {
                            continue;
                        }
                    }
                    model = new FormDataEntryProjectsViewModel();
                    formDataEntryid = proj.Id;
                    
                    var formDataEntryVariables = dbContext.FormDataEntryVariables.Where(et => et.FormDataEntryId == formDataEntryid);

                    model.ProjectName = formDataEntryVariables.Where(x => x.Variable.VariableName == "Name").Select(x => x.SelectedValues).FirstOrDefault();

                    var ProjectSubtype = formDataEntryVariables.Where(x => x.Variable.VariableName == "ProSType").Select(x => x.SelectedValues).FirstOrDefault();
                    {
                        var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == "ProSType");
                        var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                        var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                        for (int i = 0; i < variableValues.Count; i++)
                        {
                            if (variableValues[i] == ProjectSubtype)
                            {
                                model.ProjectSubtype = variableValuesDesc[i];
                            }
                        }
                    }
                    var ConfData = formDataEntryVariables.Where(x => x.Variable.VariableName == "ConfData").Select(x => x.SelectedValues).FirstOrDefault();
                    {
                        var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == "ConfData");
                        var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                        var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                        for (int i = 0; i < variableValues.Count; i++)
                        {
                            if (variableValues[i] == ConfData)
                            {
                                model.ConfData = variableValuesDesc[i];
                            }
                        }
                    }
                    var CnstModel = formDataEntryVariables.Where(x => x.Variable.VariableName == "CnstModel").Select(x => x.SelectedValues).FirstOrDefault();
                    {
                        var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == "CnstModel");
                        var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                        var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                        for (int i = 0; i < variableValues.Count; i++)
                        {
                            if (variableValues[i] == CnstModel)
                            {
                                model.CnstModel = variableValuesDesc[i];
                            }
                        }
                    }
                    var Ethics = formDataEntryVariables.Where(x => x.Variable.VariableName == "Ethics").Select(x => x.SelectedValues).FirstOrDefault();
                    {
                        var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == "Ethics");
                        var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                        var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                        for (int i = 0; i < variableValues.Count; i++)
                        {
                            if (variableValues[i] == Ethics)
                            {
                                model.Ethics = variableValuesDesc[i];
                            }
                        }
                    }
                    var DataStore = formDataEntryVariables.Where(x => x.Variable.VariableName == "DataStore").Select(x => x.SelectedValues).FirstOrDefault();
                    {
                        var prostype = dbContext.Variables.FirstOrDefault(x => x.VariableName == "DataStore");
                        var variableValues = prostype != null ? prostype.Values != null ? prostype.Values.Split('|').ToList() : null : null;
                        var variableValuesDesc = prostype != null ? prostype.VariableValueDescription != null ? prostype.VariableValueDescription.Split('|').ToList() : null : null;
                        for (int i = 0; i < variableValues.Count; i++)
                        {
                            if (variableValues[i] == DataStore)
                            {
                                model.DataStore = variableValuesDesc[i];
                            }
                        }
                    }
                    model.ProDt = formDataEntryVariables.Where(x => x.Variable.VariableName == "ProDt").Select(x => x.SelectedValues).FirstOrDefault();
                    var projStaff = projectJson.FirstOrDefault(x => x.ProjectId == proj.Id);

                    DeployProjectJsonViewModel projDeserialized = new System.Web.Script.Serialization.JavaScriptSerializer().
                                Deserialize<Core.ViewModels.DeployProjectJsonViewModel>(projStaff.ProjectBuilderJSONData);

                    List<NewProjectStaffMemberRoleViewModel> newProjStaffMember = new List<NewProjectStaffMemberRoleViewModel>();
                    if (projDeserialized.projectStaffMemberList.Count() > 0)
                    {
                        foreach (var staff in projDeserialized.projectStaffMemberList)
                        {
                            newProjStaffMember.Add(new NewProjectStaffMemberRoleViewModel()
                            {

                                Id = staff.ProjectId,
                                ProjectGuid = staff.ProjectGuid,
                                UserGuid = staff.ProjectUserGuid,
                                RoleGuid = staff.ProjectUserRoleGuid,
                                ProjectUserName = staff.ProjectUserName,
                                ProjectUserRoleName = staff.ProjectUserRoleName,
                            });
                        }
                    }
                    model.ProjectStaffMembersRoles = newProjStaffMember;
                    model.Guid = proj.Guid;
                    projectList.Add(model);
                }
            }
            return projectList;
        }
        public FormDataEntryViewModel TestEnvironment_Create(FormDataEntryViewModel model)
        {
            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);
            var form = dbContext.Forms.FirstOrDefault(f => f.Guid == model.FormId);

            var entity = dbContext.EntityTypes.FirstOrDefault(x => x.Guid == model.EntityId);
            var activity = dbContext.Activities.FirstOrDefault(x => x.Guid == model.ActivityId);

            string fname = string.Empty;
            string lname = string.Empty;

            if (form.FormTitle == "Person Registration")
            {
                activity = dbContext.Activities.FirstOrDefault(x => x.ActivityName == "Person Registration" && x.FormDataEntry.Guid == model.ProjectId);
                entity = dbContext.EntityTypes.FirstOrDefault(x => x.Name == "Person");

                var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                var f = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                lname = l != null ? l.SelectedValues : string.Empty;
                fname = f != null ? f.SelectedValues : string.Empty;
            }
            else if (form.FormTitle == "Participant Registration")
            {
                activity = dbContext.Activities.FirstOrDefault(x => x.ActivityName == "Participant Registration" && x.FormDataEntry.Guid == model.ProjectId);
                entity = dbContext.EntityTypes.FirstOrDefault(x => x.Name == "Participant");

                var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                var f = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                var m = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 17);

                lname = l != null ? l.SelectedValues : string.Empty;
                fname = f != null ? f.SelectedValues : string.Empty + " " + m != null ? m.SelectedValues : string.Empty;
            }
            else if (form.FormTitle == "Place/Group Registration")
            {


                activity = dbContext.Activities.FirstOrDefault(x => x.ActivityName == "Place/Group Registration" && x.FormDataEntry.Guid == model.ProjectId);
                entity = dbContext.EntityTypes.FirstOrDefault(x => x.Name == "Place/Group");

                var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                fname = l != null ? l.SelectedValues : string.Empty;
            }
            else if (form.FormTitle == "Project Registration")
            {
                activity = dbContext.Activities.FirstOrDefault(x => x.ActivityName == "Project Registration" && x.FormDataEntry.Guid == model.ProjectId);
                entity = dbContext.EntityTypes.FirstOrDefault(x => x.Name == "Project");
            }
            var project = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == model.ProjectId);
            string projName = string.Empty;//
            try
            {
                projName = dbContext.FormDataEntryVariables.FirstOrDefault(x => x.FormDataEntryId == project.Id && x.Variable.VariableName == "Name").SelectedValues;
            }
            catch (Exception ex)
            { }

            JsonFormDataEntries jsonFormDataEntries = new JsonFormDataEntries();
            jsonFormDataEntries.ActivityId = activity != null ? activity.Id : 0;
            jsonFormDataEntries.ActivityGuid = activity != null ? activity.Guid : Guid.Empty;
            jsonFormDataEntries.ActivityName = activity != null ? activity.ActivityName : string.Empty;
            jsonFormDataEntries.ProjectId = project != null ? project.Id : 0;
            jsonFormDataEntries.ProjectGuid = project != null ? project.Guid : Guid.Empty;
            jsonFormDataEntries.ProjectName = projName;
            jsonFormDataEntries.EntityId = entity != null ? entity.Id : 0;
            jsonFormDataEntries.EntityGuid = entity != null ? entity.Guid : Guid.Empty;
            jsonFormDataEntries.EntityName = entity != null ? entity.Name : string.Empty;
            jsonFormDataEntries.Status = model.Status;
            jsonFormDataEntries.CreatedById = createdBy != null ? createdBy.Id : 1;
            jsonFormDataEntries.CreatedByGuid = createdBy != null ? createdBy.Guid : Guid.Empty;
            jsonFormDataEntries.CreatedDate = DateTime.UtcNow;
            jsonFormDataEntries.CreatedDateString = DateTime.UtcNow.ToString("dd-MMM-yyyy");
            jsonFormDataEntries.FormId = form != null ? form.Id : (int?)null;
            jsonFormDataEntries.FormGuid = form != null ? form.Guid : Guid.Empty;
            jsonFormDataEntries.FormTitle = form != null ? form.FormTitle : string.Empty;
            jsonFormDataEntries.ThisUserId = model.ThisUserId;

            jsonFormDataEntries.JsonFormDataEntryVariableList = new List<JsonFormDataEntryVariables>();

            JsonFormDataEntryVariables jsonFormDataEntryVariables = new JsonFormDataEntryVariables();
            int entId = 0;
            int? parentEntityNumber = (int?)null;
            if (form.FormTitle == "Person Registration" || form.FormTitle == "Participant Registration" || form.FormTitle == "Place/Group Registration" || form.FormTitle == "Project Registration")
            {
                entId = GenerateRandomNo();
            }
            else
            {
                entId = GenerateRandomNo6Digit();
                try { parentEntityNumber = Convert.ToInt32(model.ParticipantId); } catch (Exception exc) { }
            }
            
            foreach (var item in model.FormDataEntryVariable)
            {
                if (item.VariableId == 3)
                {
                    item.SelectedValues = entId.ToString();
                }

                var variable = dbContext.Variables.FirstOrDefault(x => x.Id == item.VariableId);
                jsonFormDataEntryVariables = new JsonFormDataEntryVariables();
                jsonFormDataEntryVariables.VariableGuid = variable != null ? variable.Guid : Guid.Empty;
                jsonFormDataEntryVariables.VariableId = item.VariableId;
                jsonFormDataEntryVariables.VariableName = variable != null ? variable.VariableName : string.Empty;
                jsonFormDataEntryVariables.SelectedValue = item.SelectedValues;
                jsonFormDataEntries.JsonFormDataEntryVariableList.Add(jsonFormDataEntryVariables);
            }



            var json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(jsonFormDataEntries);

            var formDataEntryJson = this.dbContext.ProjectBuilderJSONEntityValues.Add(new Data.ProjectBuilderJSONEntityValue
            {
                Guid = Guid.NewGuid(),
                ProjectId = jsonFormDataEntries.ProjectId,
                ActivityId = jsonFormDataEntries.ActivityId,
                FormId = (int)jsonFormDataEntries.FormId,
                ProjectBuilderJSONValues = json,
                CreatedBy = jsonFormDataEntries.CreatedById,
                CreatedDate = DateTime.UtcNow,
                EntId = Convert.ToInt32(entId),
                ParentEntityNumber = parentEntityNumber,
            });
            SaveChanges();
            int enid = Convert.ToInt32(entId);

            if (form.FormTitle == "Person Registration" || form.FormTitle == "Participant Registration" || form.FormTitle == "Place/Group Registration")
            {
                #region linkage from summary page
                ProjectBuilderJSON projectBuilderJSONs = dbContext.ProjectBuilderJSONs.FirstOrDefault(x => x.ProjectId == project.Id);
                if (projectBuilderJSONs != null)
                {
                    DeployProjectJsonViewModel deserialized = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Core.ViewModels.DeployProjectJsonViewModel>(projectBuilderJSONs.ProjectBuilderJSONData);
                    AddSummaryPageActivityViewModel addsummarypageactivityModel = new AddSummaryPageActivityViewModel();
                    var activityCompletedBy = dbContext.UserLogins.FirstOrDefault(x => x.Email.ToLower() == "testsystemadmin@aspree.com");
                    addsummarypageactivityModel.ProjectId = model.ProjectId;
                    addsummarypageactivityModel.CreatedBy = model.CreatedBy;
                    addsummarypageactivityModel.CreatedDate = DateTime.UtcNow;
                    addsummarypageactivityModel.ActivityId = activity.Guid;
                    addsummarypageactivityModel.ActivityName = activity.ActivityName;
                    addsummarypageactivityModel.ActivityCompletedByUser = activityCompletedBy != null ? activityCompletedBy.Guid : Guid.Empty;
                    addsummarypageactivityModel.ActivityCompletedByUserName = activityCompletedBy != null ? activityCompletedBy.FirstName + " " + activityCompletedBy.LastName : string.Empty;
                    addsummarypageactivityModel.ActivityDate = DateTime.UtcNow;
                    addsummarypageactivityModel.IsActivityAdded = true;
                    addsummarypageactivityModel.PersonEntityId = Convert.ToString(enid);
                    addsummarypageactivityModel.Forms = activity.ActivityForms.Select(c => new FormActivityViewModel()
                    {
                        FormTitle = c.Form.FormTitle,
                        Id = c.Form.Guid,
                        Status = Enum.GetName(typeof(Core.Enum.FormStatusTypes), (int)Core.Enum.FormStatusTypes.Draft),
                    }).ToList();

                    if (deserialized.AddSummaryPageActivityViewModelList != null)
                        deserialized.AddSummaryPageActivityViewModelList.Add(addsummarypageactivityModel);
                    else
                    {
                        deserialized.AddSummaryPageActivityViewModelList = new List<AddSummaryPageActivityViewModel>();
                        deserialized.AddSummaryPageActivityViewModelList.Add(addsummarypageactivityModel);
                    }


                    string newjsonData = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(deserialized);
                    projectBuilderJSONs.ProjectBuilderJSONData = newjsonData;
                    SaveChanges();
                }
                #endregion

                #region save in login table
                if (form.FormTitle == "Place/Group Registration" || form.FormTitle == "Person Registration" || form.FormTitle == "Participant Registration")
                {
                    var emali = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38).SelectedValues : string.Empty;
                    var username = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40).SelectedValues : string.Empty;
                    if (form.FormTitle == "Place/Group Registration")
                    {
                        var entityType = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 5) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 5).SelectedValues : string.Empty;
                        if (entityType != "15")
                        {
                            var loginTbl = dbContext.UserLogins.FirstOrDefault(x => x.Id == jsonFormDataEntries.ThisUserId);
                            if (loginTbl != null)
                            {
                                loginTbl.Password = null;
                                loginTbl.Salt = null;
                                loginTbl.SecurityQuestionId = null;
                                loginTbl.Answer = null;
                                loginTbl.Status = (int)Core.Enum.Status.InActive;
                                dbContext.SaveChanges();
                            }
                        }
                    }
                    //save in user table
                    var authTypeGuid = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51).SelectedValues : new Guid().ToString();
                    var authType = dbContext.LoginAuthTypeMasters.FirstOrDefault(x => x.Guid == new Guid(authTypeGuid));

                    var userrole = dbContext.Roles.FirstOrDefault(x => x.Id == (int)Core.Enum.RoleTypes.Data_Entry_Supervisor);

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
                    entityUserlogin.UserName = !string.IsNullOrEmpty(username) ? username : entId.ToString();
                    var savedUser = _userLoginProvider.Create(entityUserlogin);
                    jsonFormDataEntries.ThisUserId = savedUser != null ? savedUser.Id : (int?)null;
                }
                #endregion
            }
            if (form.FormTitle == "Project Linkage")
            {
                #region assig project to user
                var formdataentryProjectLinkage = dbContext.ProjectBuilderJSONEntityValues.FirstOrDefault(x => x.EntId == formDataEntryJson.ParentEntityNumber);
                if (formdataentryProjectLinkage != null)
                {
                    Int64 ThisUserId = 0;
                    ProjectBuilderJSONEntityValue parentENtity = dbContext.ProjectBuilderJSONEntityValues.FirstOrDefault(x => x.EntId == formDataEntryJson.ParentEntityNumber);
                    if (parentENtity != null)
                    {
                        var parentDeserialized = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Core.ViewModels.JsonFormDataEntries>(parentENtity.ProjectBuilderJSONValues);
                        ThisUserId = parentDeserialized.ThisUserId ?? 0;
                    }


                    UserLogin uLogin = dbContext.UserLogins.FirstOrDefault(x => x.Id == ThisUserId);
                    if (uLogin != null)
                    {
                        var projectStaffMembersRoles = dbContext.UserRoles.Where(x => x.UserId == uLogin.Id).ToList();
                        var proRole = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 56);
                        Role urole = new Role();
                        if (proRole != null)
                        {
                            string role = string.Empty;
                            switch (proRole.SelectedValues)
                            {
                                case "1":
                                    role = "Project Admin";
                                    break;
                                case "2":
                                    role = "Data Entry Supervisor";
                                    break;
                                case "3":
                                    role = "Data Entry Operator";
                                    break;
                                case "4":
                                    role = "Data Entry";
                                    break;
                                default:
                                    break;
                            }
                            urole = dbContext.Roles.FirstOrDefault(x => x.Name == role);
                        }
                        var linkedProjectId = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 52);
                        Guid linkedProjectGuid = Guid.Parse(linkedProjectId.SelectedValues);
                        FormDataEntry linkedProject = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == linkedProjectGuid);

                        IEnumerable<ProjectStaffMemberRole> projectStaffMemberRoles = this.dbContext.ProjectStaffMemberRoles.Where(x => x.UserId == uLogin.Id);
                        dbContext.ProjectStaffMemberRoles.RemoveRange(projectStaffMemberRoles);
                        SaveChanges();

                        var projectstaffRoles = new ProjectStaffMemberRole()
                        {
                            Guid = Guid.NewGuid(),
                            ProjectId = linkedProject != null ? linkedProject.Id : 1,
                            UserId = uLogin.Id,
                            RoleId = urole != null ? urole.Id : (int)Core.Enum.RoleTypes.Data_Entry,
                            CreatedBy = createdBy.Id,
                            StaffCreatedDate = DateTime.UtcNow,
                        };
                        dbContext.ProjectStaffMemberRoles.Add(projectstaffRoles);



                        var fullProjJson = dbContext.ProjectBuilderJSONs.FirstOrDefault(x => x.ProjectId == project.Id);
                        string jsonString = fullProjJson != null ? fullProjJson.ProjectBuilderJSONData : string.Empty;
                        DeployProjectJsonViewModel deserializedDeployProjectJsonViewModel = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Core.ViewModels.DeployProjectJsonViewModel>(jsonString);

                        if (deserializedDeployProjectJsonViewModel.projectStaffMemberList != null)
                        {
                            deserializedDeployProjectJsonViewModel.projectStaffMemberList.Add(new JsonProjectStaffMembers()
                            {
                                ProjectGuid = project.Guid,
                                ProjectId = project.Id,
                                ProjectName = projName,
                                ProjectUserGuid = uLogin.Guid,
                                ProjectUserId = uLogin.Id,
                                ProjectUserName = uLogin.FirstName + " " + uLogin.LastName,
                                ProjectUserRoleGuid = urole.Guid,
                                ProjectUserRoleId = urole.Id,
                                ProjectUserRoleName = urole.Name,
                            });

                            var serializedProject = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(deserializedDeployProjectJsonViewModel);

                            fullProjJson.ProjectBuilderJSONData = serializedProject;
                        }
                        SaveChanges();
                    }
                }
                #endregion
            }

            ProjectBuilderJSONEntityValue entDetails = dbContext.ProjectBuilderJSONEntityValues.FirstOrDefault(x => x.EntId == enid);
            if (entDetails != null)
            {
                jsonFormDataEntries.FormDataEntryId = formDataEntryJson.Id;
                jsonFormDataEntries.FormDataEntryGuid = formDataEntryJson.Guid;
                string newjsonData = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(jsonFormDataEntries);
                entDetails.ProjectBuilderJSONValues = newjsonData;
                SaveChanges();
            }






            if (model.Status == (int)Core.Enum.FormStatusTypes.Published || model.Status == (int)Core.Enum.FormStatusTypes.Draft)
            {
                #region update form-status
                try
                {
                    ProjectBuilderJSON projectBuilderJSONs = dbContext.ProjectBuilderJSONs.FirstOrDefault(x => x.FormDataEntry.Guid == model.ProjectId);
                    if (projectBuilderJSONs != null)
                    {
                        DeployProjectJsonViewModel deserializedDeployProjectJsonViewModel = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Core.ViewModels.DeployProjectJsonViewModel>(projectBuilderJSONs.ProjectBuilderJSONData);
                        var jsonForms = deserializedDeployProjectJsonViewModel.AddSummaryPageActivityViewModelList.Where(x => x.PersonEntityId == model.ParticipantId && x.ActivityId == model.ActivityId)
                            .Select(x => x.Forms).FirstOrDefault();

                        jsonForms.FirstOrDefault(x => x.Id == model.FormId).Status = Enum.GetName(typeof(Core.Enum.FormStatusTypes), model.Status);
                        var c = deserializedDeployProjectJsonViewModel.AddSummaryPageActivityViewModelList.Where(x => x.PersonEntityId == model.ParticipantId && x.ActivityId == model.ActivityId).FirstOrDefault();

                        deserializedDeployProjectJsonViewModel.AddSummaryPageActivityViewModelList.Remove(c);

                        c.Forms = jsonForms;

                        deserializedDeployProjectJsonViewModel.AddSummaryPageActivityViewModelList.Add(c);
                        var jsonnew = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(deserializedDeployProjectJsonViewModel);

                        projectBuilderJSONs.ProjectBuilderJSONData = jsonnew;
                        SaveChanges();
                    }
                }
                catch (Exception dxd)
                { }
                #endregion
            }

            FormDataEntryViewModel mdl = new FormDataEntryViewModel();
            mdl.ParticipantId = entId.ToString();
            return mdl;// ToModel(formDataEntry);
        }
        public FormDataEntryViewModel TestEnvironment_Update(FormDataEntryViewModel model)
        {
            var createdBy = _userLoginProvider.GetByGuid(model.CreatedBy);
            var form = dbContext.Forms.FirstOrDefault(f => f.Guid == model.FormId);

            var entity = dbContext.EntityTypes.FirstOrDefault(x => x.Guid == model.EntityId);
            var activity = dbContext.Activities.FirstOrDefault(x => x.Guid == model.ActivityId);

            if (form.FormTitle == "Person Registration")
            {
                activity = dbContext.Activities.FirstOrDefault(x => x.ActivityName == "Person Registration" && x.FormDataEntry.Guid == model.ProjectId);
                entity = dbContext.EntityTypes.FirstOrDefault(x => x.Name == "Person");
            }
            else if (form.FormTitle == "Participant Registration")
            {
                activity = dbContext.Activities.FirstOrDefault(x => x.ActivityName == "Participant Registration" && x.FormDataEntry.Guid == model.ProjectId);
                entity = dbContext.EntityTypes.FirstOrDefault(x => x.Name == "Participant");
            }
            else if (form.FormTitle == "Place/Group Registration")
            {
                activity = dbContext.Activities.FirstOrDefault(x => x.ActivityName == "Place/Group Registration" && x.FormDataEntry.Guid == model.ProjectId);
                entity = dbContext.EntityTypes.FirstOrDefault(x => x.Name == "Place/Group");
            }
            else if (form.FormTitle == "Project Registration")
            {
                activity = dbContext.Activities.FirstOrDefault(x => x.ActivityName == "Project Registration" && x.FormDataEntry.Guid == model.ProjectId);
                entity = dbContext.EntityTypes.FirstOrDefault(x => x.Name == "Project");
            }
            var project = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == model.ProjectId);
            string projName = string.Empty;//
            try
            {
                projName = dbContext.FormDataEntryVariables.FirstOrDefault(x => x.FormDataEntryId == project.Id && x.Variable.VariableName == "Name").SelectedValues;
            }
            catch (Exception ex)
            { }

            List<JsonFormDataEntryVariables> jsonFormDataEntryVariableList = new List<JsonFormDataEntryVariables>();

            string entId = string.Empty;
            JsonFormDataEntryVariables jsonFormDataEntryVariables = new JsonFormDataEntryVariables();
            foreach (var item in model.FormDataEntryVariable)
            {
                if (item.VariableId == 3)
                {
                    int perentEntId = model.ParticipantId != null ? Convert.ToInt32(model.ParticipantId) : 0;
                    var isCustomEntity = dbContext.ProjectBuilderJSONEntityValues.FirstOrDefault(x => x.ParentEntityNumber == perentEntId && x.Form.Guid == model.FormId);
                    if (isCustomEntity != null)
                    {
                        entId = Convert.ToString(isCustomEntity.EntId);
                        item.SelectedValues = entId;
                    }
                    else
                    {

                        entId = model.ParticipantId;
                        item.SelectedValues = entId;
                    }
                }

                var variable = dbContext.Variables.FirstOrDefault(x => x.Id == item.VariableId);
                jsonFormDataEntryVariables = new JsonFormDataEntryVariables();
                jsonFormDataEntryVariables.VariableGuid = variable != null ? variable.Guid : Guid.Empty;
                jsonFormDataEntryVariables.VariableId = item.VariableId;
                jsonFormDataEntryVariables.VariableName = variable != null ? variable.VariableName : string.Empty;
                jsonFormDataEntryVariables.SelectedValue = item.SelectedValues;
                jsonFormDataEntryVariableList.Add(jsonFormDataEntryVariables);
            }


            int int_entId = Convert.ToInt32(entId);
            JsonFormDataEntries deserialized = new JsonFormDataEntries();
            ProjectBuilderJSONEntityValue testFormdataentry = dbContext.ProjectBuilderJSONEntityValues.FirstOrDefault(x => x.EntId == int_entId);
            if (testFormdataentry != null)
            {
                deserialized = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Core.ViewModels.JsonFormDataEntries>(testFormdataentry.ProjectBuilderJSONValues);
                deserialized.JsonFormDataEntryVariableList = jsonFormDataEntryVariableList;
            }

            var json = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(deserialized);

            var formDataEntryJson = this.dbContext.ProjectBuilderJSONEntityValues.FirstOrDefault(x => x.EntId == int_entId);
            if (formDataEntryJson != null)
            {
                formDataEntryJson.ProjectBuilderJSONValues = json;
                formDataEntryJson.ModifiedBy = createdBy.Id;
                formDataEntryJson.ModifiedDate = DateTime.UtcNow;
                SaveChanges();
            }
            //formDataEntryJson
            if (form.FormTitle == "Person Registration" || form.FormTitle == "Participant Registration" || form.FormTitle == "Place/Group Registration")
            {
                
            }

            if (form.FormTitle == "Project Linkage")
            {
                #region assig project to user
                var formdataentryProjectLinkage = dbContext.ProjectBuilderJSONEntityValues.FirstOrDefault(x => x.EntId == formDataEntryJson.ParentEntityNumber);
                if (formdataentryProjectLinkage != null)
                {
                    ProjectBuilderJSONEntityValue parentENtity = dbContext.ProjectBuilderJSONEntityValues.FirstOrDefault(x => x.EntId == formDataEntryJson.ParentEntityNumber);
                    if (parentENtity != null)
                    {
                        var parentDeserialized = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Core.ViewModels.JsonFormDataEntries>(parentENtity.ProjectBuilderJSONValues);
                        deserialized.ThisUserId = parentDeserialized.ThisUserId;
                    }


                    UserLogin uLogin = dbContext.UserLogins.FirstOrDefault(x => x.Id == deserialized.ThisUserId);
                    if (uLogin != null)
                    {
                        var projectStaffMembersRoles = dbContext.UserRoles.Where(x => x.UserId == uLogin.Id).ToList();
                        var proRole = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 56);
                        Role urole = new Role();
                        if (proRole != null)
                        {
                            string role = string.Empty;
                            switch (proRole.SelectedValues)
                            {
                                case "1":
                                    role = "Project Admin";
                                    break;
                                case "2":
                                    role = "Data Entry Supervisor";
                                    break;
                                case "3":
                                    role = "Data Entry Operator";
                                    break;
                                case "4":
                                    role = "Data Entry";
                                    break;
                                default:
                                    break;
                            }
                            urole = dbContext.Roles.FirstOrDefault(x => x.Name == role);
                        }
                        var linkedProjectId = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 52);
                        Guid linkedProjectGuid = Guid.Parse(linkedProjectId.SelectedValues);
                        FormDataEntry linkedProject = dbContext.FormDataEntries.FirstOrDefault(x => x.Guid == linkedProjectGuid);

                        IEnumerable<ProjectStaffMemberRole> projectStaffMemberRoles = this.dbContext.ProjectStaffMemberRoles.Where(x => x.UserId == uLogin.Id);
                        dbContext.ProjectStaffMemberRoles.RemoveRange(projectStaffMemberRoles);
                        SaveChanges();

                        var projectstaffRoles = new ProjectStaffMemberRole()
                        {
                            Guid = Guid.NewGuid(),
                            ProjectId = linkedProject != null ? linkedProject.Id : 1,
                            UserId = uLogin.Id,
                            RoleId = urole != null ? urole.Id : (int)Core.Enum.RoleTypes.Data_Entry,
                            CreatedBy = createdBy.Id,
                            StaffCreatedDate = DateTime.UtcNow,
                        };
                        dbContext.ProjectStaffMemberRoles.Add(projectstaffRoles);



                        var fullProjJson = dbContext.ProjectBuilderJSONs.FirstOrDefault(x => x.ProjectId == project.Id);
                        string jsonString = fullProjJson != null ? fullProjJson.ProjectBuilderJSONData : string.Empty;
                        DeployProjectJsonViewModel deserializedDeployProjectJsonViewModel = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Core.ViewModels.DeployProjectJsonViewModel>(jsonString);

                        if (deserializedDeployProjectJsonViewModel.projectStaffMemberList != null)
                        {
                            deserializedDeployProjectJsonViewModel.projectStaffMemberList.Add(new JsonProjectStaffMembers()
                            {
                                ProjectGuid = project.Guid,
                                ProjectId = project.Id,
                                ProjectName = projName,
                                ProjectUserGuid = uLogin.Guid,
                                ProjectUserId = uLogin.Id,
                                ProjectUserName = uLogin.FirstName + " " + uLogin.LastName,
                                ProjectUserRoleGuid = urole.Guid,
                                ProjectUserRoleId = urole.Id,
                                ProjectUserRoleName = urole.Name,
                            });

                            var serializedProject = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(deserializedDeployProjectJsonViewModel);

                            fullProjJson.ProjectBuilderJSONData = serializedProject;
                        }
                        SaveChanges();
                    }
                }
                #endregion
            }

            #region save user logins

            string fname = string.Empty;
            string lname = string.Empty;
            if (form.FormTitle == "Person Registration")
            {
                var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                var f = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                lname = l != null ? l.SelectedValues : string.Empty;
                fname = f != null ? f.SelectedValues : string.Empty;
            }
            else if (form.FormTitle == "Participant Registration")
            {
                var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                var f = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 16);
                var m = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 17);

                lname = l != null ? l.SelectedValues : string.Empty;
                fname = f != null ? f.SelectedValues : string.Empty + " " + m != null ? m.SelectedValues : string.Empty;
            }
            else if (form.FormTitle == "Place/Group Registration")
            {
                var l = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 15);
                fname = l != null ? l.SelectedValues : string.Empty;
            }

            if (form.FormTitle == "Place/Group Registration" || form.FormTitle == "Person Registration" || form.FormTitle == "Participant Registration")
            {
                var emali = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 38).SelectedValues : string.Empty;
                var username = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 40).SelectedValues : string.Empty;
                if (form.FormTitle == "Place/Group Registration")
                {
                    var entityType = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 5) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 5).SelectedValues : string.Empty;
                    if (entityType != "15")
                    {
                        var loginTbl = dbContext.UserLogins.FirstOrDefault(x => x.Id == deserialized.ThisUserId);
                        if (loginTbl != null)
                        {
                            loginTbl.Password = null;
                            loginTbl.Salt = null;
                            loginTbl.SecurityQuestionId = null;
                            loginTbl.Answer = null;
                            loginTbl.Status = (int)Core.Enum.Status.InActive;
                            dbContext.SaveChanges();
                        }
                    }
                }
                if (!string.IsNullOrEmpty(emali) && !string.IsNullOrEmpty(username))
                {
                    var logintable = dbContext.UserLogins.FirstOrDefault(x => x.Id == deserialized.ThisUserId);
                    if (logintable == null)
                    {
                    }
                    else
                    {
                        var authTypeGuid = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 51).SelectedValues : new Guid().ToString();
                        var authType = dbContext.LoginAuthTypeMasters.FirstOrDefault(x => x.Guid == new Guid(authTypeGuid));
                        var userrole = dbContext.Roles.FirstOrDefault(x => x.Id == (int)Core.Enum.RoleTypes.Data_Entry_Operator);

                        UserLoginViewModel entityUserlogin = new UserLoginViewModel();
                        entityUserlogin.FirstName = fname;
                        entityUserlogin.LastName = lname;
                        entityUserlogin.Email = emali;
                        entityUserlogin.UserName = username;
                        entityUserlogin.Mobile = model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 39) != null ? model.FormDataEntryVariable.FirstOrDefault(x => x.VariableId == 39).SelectedValues : null;
                        entityUserlogin.TenantId = model.TenantId != null ? (Guid)model.TenantId : new Guid();
                        entityUserlogin.AuthTypeId = authType != null ? authType.AuthType : 0;

                        entityUserlogin.UserTypeId = (int)Core.Enum.UsersLoginType.Test;
                        entityUserlogin.TempGuid = Guid.NewGuid();
                        entityUserlogin.RoleId = userrole != null ? userrole.Guid : new Guid();

                        entityUserlogin.ModifiedBy = model.CreatedBy;
                        entityUserlogin.Guid = logintable.Guid;
                        entityUserlogin.Id = logintable.Id;
                        _userLoginProvider.Update(entityUserlogin);
                        var login = dbContext.UserLogins.FirstOrDefault(x => x.Guid == logintable.Guid);
                        if (login != null)
                        {
                            login.AuthTypeId = authType != null ? authType.AuthType : 0;
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            #endregion



            if (model.Status == (int)Core.Enum.FormStatusTypes.Published || model.Status == (int)Core.Enum.FormStatusTypes.Draft)
            {
                #region update form-status
                try
                {
                    ProjectBuilderJSON projectBuilderJSONs = dbContext.ProjectBuilderJSONs.FirstOrDefault(x => x.FormDataEntry.Guid == model.ProjectId);
                    if (projectBuilderJSONs != null)
                    {
                        DeployProjectJsonViewModel deserializedDeployProjectJsonViewModel = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Core.ViewModels.DeployProjectJsonViewModel>(projectBuilderJSONs.ProjectBuilderJSONData);
                        var jsonForms = deserializedDeployProjectJsonViewModel.AddSummaryPageActivityViewModelList.Where(x => x.PersonEntityId == model.ParticipantId && x.ActivityId == model.ActivityId)
                            .Select(x => x.Forms).FirstOrDefault();

                        jsonForms.FirstOrDefault(x => x.Id == model.FormId).Status = Enum.GetName(typeof(Core.Enum.FormStatusTypes), model.Status);

                        var c = deserializedDeployProjectJsonViewModel.AddSummaryPageActivityViewModelList.Where(x => x.PersonEntityId == model.ParticipantId && x.ActivityId == model.ActivityId).FirstOrDefault();

                        deserializedDeployProjectJsonViewModel.AddSummaryPageActivityViewModelList.Remove(c);

                        c.Forms = jsonForms;

                        deserializedDeployProjectJsonViewModel.AddSummaryPageActivityViewModelList.Add(c);
                        var jsonnew = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(deserializedDeployProjectJsonViewModel);

                        projectBuilderJSONs.ProjectBuilderJSONData = jsonnew;
                        SaveChanges();
                    }
                }
                catch (Exception dxd)
                { }
                #endregion
            }

            FormDataEntryViewModel mdl = new FormDataEntryViewModel();
            mdl.ParticipantId = entId;
            mdl.ThisUserId = deserialized.ThisUserId;
            return mdl;// ToModel(formDataEntry);
        }

        public List<List<FormDataEntryVariableViewModel>> TestEnvironment_SearchVariables(SearchPageVariableViewModel model)
        {
            Core.ViewModels.NewCategory.WriteLog("start TestEnvironment_SearchVariables in provider ");
            var form = dbContext.Forms.FirstOrDefault(x => x.Guid == model.FormId);

            List<string> allRows = new List<string>();
            if (form.FormTitle == "Participant Registration")
                allRows = dbContext.ProjectBuilderJSONEntityValues.Where(x => x.FormDataEntry.Guid == model.ProjectId).Select(x => x.ProjectBuilderJSONValues).ToList();
            else
                allRows = dbContext.ProjectBuilderJSONEntityValues.Select(x => x.ProjectBuilderJSONValues).ToList();

            List<FormDataEntryVariable> searchResult = new List<FormDataEntryVariable>();
            List<FormDataEntryVariable> searchResultFinal = new List<FormDataEntryVariable>();
            FormDataEntryVariable formDataEntryVariable = new FormDataEntryVariable();
            int i = 0;
            bool isSearched = false;

            var isEnt = model.SearchVariables.FirstOrDefault(x => x.Key == 3 && x.Value != null);
            if (!string.IsNullOrEmpty(isEnt.Value))
            {
                int searchEntityId = Convert.ToInt32(isEnt.Value);
                ProjectBuilderJSONEntityValue entityValuesByEntId = null;
                if (form.FormTitle == "Participant Registration")
                    entityValuesByEntId = dbContext.ProjectBuilderJSONEntityValues.FirstOrDefault(x => x.FormDataEntry.Guid == model.ProjectId && x.EntId == searchEntityId);
                else
                    entityValuesByEntId = dbContext.ProjectBuilderJSONEntityValues.FirstOrDefault(x => x.EntId == searchEntityId);
                var deserialized = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Core.ViewModels.JsonFormDataEntries>(entityValuesByEntId.ProjectBuilderJSONValues);
                var ds = form.FormEntityTypes.FirstOrDefault(x => x.EntityType.Name == deserialized.EntityName);
                if (ds != null)
                {
                    foreach (var item in deserialized.JsonFormDataEntryVariableList)
                    {
                        formDataEntryVariable = new FormDataEntryVariable();
                        formDataEntryVariable.CreatedBy = 1;
                        formDataEntryVariable.CreatedDate = DateTime.UtcNow;
                        formDataEntryVariable.DateDeactivated = DateTime.UtcNow;
                        formDataEntryVariable.DeactivatedBy = 1;
                        formDataEntryVariable.FormDataEntry = new FormDataEntry();
                        formDataEntryVariable.FormDataEntry.Form = new Form();
                        formDataEntryVariable.FormDataEntry.Form.FormTitle = deserialized.FormTitle;
                        formDataEntryVariable.SelectedValues = item.SelectedValue;
                        formDataEntryVariable.VariableId = item.VariableId;
                        formDataEntryVariable.FormDataEntryId = deserialized.FormDataEntryId;
                        formDataEntryVariable.ParentId = deserialized.EntityId;
                        formDataEntryVariable.FormDataEntry.ActivityId = deserialized.ActivityId;
                        formDataEntryVariable.FormDataEntry.Form.Guid = deserialized.FormGuid;
                        formDataEntryVariable.FormDataEntry.Activity = new Activity();
                        formDataEntryVariable.FormDataEntry.Activity.Guid = deserialized.ActivityGuid;
                        formDataEntryVariable.CreatedDate = deserialized.CreatedDate;
                        formDataEntryVariable.FormDataEntry.Status = deserialized.Status;
                        formDataEntryVariable.Variable = new Variable();
                        formDataEntryVariable.Variable.VariableName = item.VariableName;
                        searchResult.Add(formDataEntryVariable);
                    }
                    searchResultFinal = searchResult;
                }
            }
            else
            {
                foreach (var row in allRows)
                {
                    if (i > 0)
                    {
                        formDataEntryVariable = new FormDataEntryVariable();
                        i = 0;
                        searchResult = new List<FormDataEntryVariable>();
                    }
                    else
                    {
                        formDataEntryVariable = new FormDataEntryVariable();
                    }

                    if (searchResult.Count() > 0)
                    {
                        isSearched = true;
                    }
                    var deserialized = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Core.ViewModels.JsonFormDataEntries>(row);
                    int cnt = 0;
                    foreach (var searchInputs in model.SearchVariables)
                    {
                        string itemVal = !string.IsNullOrEmpty(searchInputs.Value) ? searchInputs.Value.ToLower() : string.Empty;

                        formDataEntryVariable = new FormDataEntryVariable();
                        var isFound = deserialized.JsonFormDataEntryVariableList
                            .FirstOrDefault(x =>
                            x.VariableId == searchInputs.Key
                            && itemVal.Equals(x.SelectedValue, StringComparison.InvariantCultureIgnoreCase)
                        );
                        if (isFound == null)
                        {
                            i++;
                            break;
                        }
                        cnt++;
                        formDataEntryVariable.CreatedBy = 1;
                        formDataEntryVariable.CreatedDate = DateTime.UtcNow;
                        formDataEntryVariable.DateDeactivated = DateTime.UtcNow;
                        formDataEntryVariable.DeactivatedBy = 1;
                        formDataEntryVariable.FormDataEntry = new FormDataEntry();
                        formDataEntryVariable.FormDataEntry.Form = new Form();
                        formDataEntryVariable.FormDataEntry.Form.FormTitle = deserialized.FormTitle;
                        formDataEntryVariable.SelectedValues = searchInputs.Value;
                        formDataEntryVariable.VariableId = searchInputs.Key;
                        formDataEntryVariable.FormDataEntryId = deserialized.FormDataEntryId;
                        formDataEntryVariable.ParentId = deserialized.EntityId;
                        formDataEntryVariable.FormDataEntry.ActivityId = deserialized.ActivityId;
                        formDataEntryVariable.FormDataEntry.Form.Guid = deserialized.FormGuid;
                        formDataEntryVariable.FormDataEntry.Activity = new Activity();
                        formDataEntryVariable.FormDataEntry.Activity.Guid = deserialized.ActivityGuid;
                        formDataEntryVariable.CreatedDate = deserialized.CreatedDate;
                        formDataEntryVariable.FormDataEntry.Status = deserialized.Status;
                        formDataEntryVariable.Variable = new Variable();
                        formDataEntryVariable.Variable.VariableName = isFound.VariableName;
                        searchResult.Add(formDataEntryVariable);

                        if (model.SearchVariables.Count() == cnt)
                        {
                            formDataEntryVariable = new FormDataEntryVariable();
                            formDataEntryVariable.CreatedBy = 1;
                            formDataEntryVariable.CreatedDate = DateTime.UtcNow;
                            formDataEntryVariable.DateDeactivated = DateTime.UtcNow;
                            formDataEntryVariable.DeactivatedBy = 1;
                            formDataEntryVariable.FormDataEntry = new FormDataEntry();
                            formDataEntryVariable.FormDataEntry.Form = new Form();
                            formDataEntryVariable.FormDataEntry.Form.FormTitle = deserialized.FormTitle;
                            formDataEntryVariable.SelectedValues = deserialized.JsonFormDataEntryVariableList.FirstOrDefault(x => x.VariableId == 3).SelectedValue;
                            formDataEntryVariable.VariableId = 3;
                            formDataEntryVariable.FormDataEntryId = deserialized.FormDataEntryId;
                            formDataEntryVariable.ParentId = deserialized.EntityId;
                            formDataEntryVariable.FormDataEntry.ActivityId = deserialized.ActivityId;
                            formDataEntryVariable.FormDataEntry.Form.Guid = deserialized.FormGuid;
                            formDataEntryVariable.FormDataEntry.Activity = new Activity();
                            formDataEntryVariable.FormDataEntry.Activity.Guid = deserialized.ActivityGuid;
                            formDataEntryVariable.CreatedDate = deserialized.CreatedDate;
                            formDataEntryVariable.FormDataEntry.Status = deserialized.Status;
                            formDataEntryVariable.Variable = new Variable();
                            formDataEntryVariable.Variable.VariableName = "EntID";
                            searchResult.Add(formDataEntryVariable);
                            searchResultFinal = searchResult;
                        }
                    }
                }
            }
            var resultVariables = searchResultFinal.Select(ToSearchModel)
                .ToList();

            var groupedVariableList = resultVariables
                .GroupBy(u => u.FormDataEntryId)
                .Select(grp => grp.ToList())
                .ToList();
            Core.ViewModels.NewCategory.WriteLog("end TestEnvironment_SearchVariables in provider ");
            return groupedVariableList;
        }


        public IEnumerable<FormDataEntryVariableViewModel> TestEnvironment_GetFormDataEntryByEntId(Guid projectId, Guid formId, string entId)
        {
            int searchEntityId = Convert.ToInt32(entId);
            var entityValuesByEntId = dbContext.ProjectBuilderJSONEntityValues.FirstOrDefault(x => x.EntId == searchEntityId);
            var deserialized = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Core.ViewModels.JsonFormDataEntries>(entityValuesByEntId.ProjectBuilderJSONValues);
            List<FormDataEntryVariable> searchResult = new List<FormDataEntryVariable>();
            FormDataEntryVariable formDataEntryVariable = new FormDataEntryVariable();
            foreach (var item in deserialized.JsonFormDataEntryVariableList)
            {
                formDataEntryVariable = new FormDataEntryVariable();
                formDataEntryVariable.CreatedBy = 1;
                formDataEntryVariable.CreatedDate = DateTime.UtcNow;
                formDataEntryVariable.DateDeactivated = DateTime.UtcNow;
                formDataEntryVariable.DeactivatedBy = 1;
                formDataEntryVariable.FormDataEntry = new FormDataEntry();
                formDataEntryVariable.FormDataEntry.Form = new Form();
                formDataEntryVariable.FormDataEntry.Form.FormTitle = deserialized.FormTitle;
                formDataEntryVariable.SelectedValues = item.SelectedValue;
                formDataEntryVariable.VariableId = item.VariableId;
                formDataEntryVariable.FormDataEntryId = deserialized.FormDataEntryId;
                formDataEntryVariable.ParentId = deserialized.EntityId;
                formDataEntryVariable.FormDataEntry.ActivityId = deserialized.ActivityId;
                formDataEntryVariable.FormDataEntry.Form.Guid = deserialized.FormGuid;
                formDataEntryVariable.FormDataEntry.Activity = new Activity();
                formDataEntryVariable.FormDataEntry.Activity.Guid = deserialized.ActivityGuid;
                formDataEntryVariable.CreatedDate = deserialized.CreatedDate;
                formDataEntryVariable.FormDataEntry.Status = deserialized.Status;
                formDataEntryVariable.Variable = new Variable();
                formDataEntryVariable.Variable.VariableName = item.VariableName;
                searchResult.Add(formDataEntryVariable);
            }
            return searchResult.Select(ToSearchModel).ToList();
        }



        public List<List<FormDataEntryVariableViewModel>> SearchEntityInMONGODB(SearchPageVariableViewModel model, string source = null)
        {
            return new MongoProvider.SearchProvider(
                _userLoginProvider
                , _dbContext
                , _mongoDBContext
                , _privilegeProvider
                , _testMongoDBContext
                , this
                , null
                ).SearchEntities(model, source);
        }
    }
}