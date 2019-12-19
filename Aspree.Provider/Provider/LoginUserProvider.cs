using Aspree.Core.Enum;
using Aspree.Core.ViewModels;
using Aspree.Core.ViewModels.MongoViewModels;
using Aspree.Data;
using Aspree.Data.MongoDB;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Provider
{
    public class LoginUserProvider
    {
        private readonly Interface.ISecurity _security = new Security();
        public RoleViewModel IsValidUser(string UserName, string Password)
        {
            using (var context = new AspreeEntities())
            {
                var userInfo = new UserLogin();
                if (Password == "{{GoogleLogin}}")
                {
                    userInfo = context.UserLogins
                        .FirstOrDefault(x => x.Email.ToLower() == UserName.ToLower());
                }
                else if (Password == "{{okta}}")
                {
                    userInfo = context.UserLogins
                        .FirstOrDefault(x => x.Email.ToLower() == UserName.ToLower());
                }
                else
                {
                    userInfo = context.UserLogins
                        .FirstOrDefault(x => x.Email.ToLower() == UserName.ToLower() && x.Password == Password);
                }


                if (userInfo != null)
                {
                    return new RoleViewModel()
                    {
                        Email = userInfo.Email,
                        Id = userInfo.Id,
                        Guid = userInfo.Guid,
                        Name = userInfo.FirstName + " " + userInfo.LastName,
                        Roles = userInfo.UserRoles.Select(x => x.Role.Name).ToList(),
                        TenantId = userInfo.Tenant.Guid
                    };
                }
            }
            return null;
        }

        public RoleViewModel CheckLoginCredentials(string username, string password, bool isTestSite = false)
        {
            using (var context = new AspreeEntities())
            {
                UserLogin userInfo = new UserLogin();
                if (password == "{{okta}}")
                {
                    if (isTestSite)
                        userInfo = context.UserLogins
                            .FirstOrDefault(x => x.UserName.ToLower() == username.ToLower() && x.AuthTypeId == (int)Core.Enum.AuthenticationTypes.OpenID_Connect && x.UserTypeId == (int)Core.Enum.UsersLoginType.Test && x.IsUserApprovedBySystemAdmin && x.Status == (int)Core.Enum.Status.Active);
                    else
                        userInfo = context.UserLogins
                        .FirstOrDefault(x => x.UserName.ToLower() == username.ToLower() && x.AuthTypeId == (int)Core.Enum.AuthenticationTypes.OpenID_Connect && x.UserTypeId != (int)Core.Enum.UsersLoginType.Test && x.IsUserApprovedBySystemAdmin && x.Status == (int)Core.Enum.Status.Active);
                }
                else if (password == "{{GoogleLogin}}")
                {
                    if (isTestSite)
                        userInfo = context.UserLogins
                            .FirstOrDefault(x => x.UserName.ToLower() == username.ToLower() && x.AuthTypeId == (int)Core.Enum.LoginAuthenticationTypes.Google && x.UserTypeId == (int)Core.Enum.UsersLoginType.Test && x.IsUserApprovedBySystemAdmin && x.Status == (int)Core.Enum.Status.Active);
                    else
                        userInfo = context.UserLogins
                        .FirstOrDefault(x => x.UserName.ToLower() == username.ToLower() && x.AuthTypeId == (int)Core.Enum.LoginAuthenticationTypes.Google && x.UserTypeId != (int)Core.Enum.UsersLoginType.Test && x.IsUserApprovedBySystemAdmin && x.Status == (int)Core.Enum.Status.Active);
                }
                else
                {
                    if (isTestSite)
                        userInfo = context.UserLogins
                            .FirstOrDefault(x => x.UserName.ToLower() == username.ToLower() && x.AuthTypeId == (int)Core.Enum.AuthenticationTypes.Local_Password && x.UserTypeId == (int)Core.Enum.UsersLoginType.Test && x.IsUserApprovedBySystemAdmin && x.Status == (int)Core.Enum.Status.Active);
                    else
                        userInfo = context.UserLogins
                            .FirstOrDefault(x => x.UserName.ToLower() == username.ToLower() && x.AuthTypeId == (int)Core.Enum.AuthenticationTypes.Local_Password && x.UserTypeId != (int)Core.Enum.UsersLoginType.Test && x.IsUserApprovedBySystemAdmin && x.Status == (int)Core.Enum.Status.Active);
                    if (userInfo != null)
                    {
                        string hash = _security.ComputeHash(password, userInfo.Salt);
                        if (hash != userInfo.Password)
                        {
                            int cnt = userInfo.LoginFailedAttemptCount ?? 0;
                            userInfo.LoginFailedAttemptCount = ++cnt;
                            userInfo.LoginFailedAttemptDate = DateTime.UtcNow;

                            if (userInfo.LoginFailedAttemptCount >= 3)
                            {
                                userInfo.Status = (int)Core.Enum.Status.Locked;
                                UpdateLockedUserInfo(userInfo.Id, isTestSite);
                            }

                            context.SaveChanges();
                            userInfo = null;
                        }
                    }
                }
                if (userInfo != null)
                {
                    if (userInfo.LoginFailedAttemptCount.HasValue)
                    {
                        userInfo.LoginFailedAttemptCount = null;
                        userInfo.LoginFailedAttemptDate = (DateTime?)null;
                        context.SaveChanges();
                    }

                    return new RoleViewModel()
                    {
                        Email = userInfo.Email,
                        Id = userInfo.Id,
                        Guid = userInfo.Guid,
                        Name = userInfo.FirstName + " " + userInfo.LastName,
                        Roles = userInfo.UserRoles.Select(x => x.Role.Name).ToList(),
                        TenantId = userInfo.Tenant.Guid
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        public RoleViewModel CheckInternalProjectLoginCredentials(string username, string password)
        {
            using (var context = new AspreeEntities())
            {
                UserLogin userInfo = new UserLogin();

                var guidList = password.Split('_');
                Guid[] guidArray = Array.ConvertAll(guidList, x => Guid.Parse(x));
                Guid userId = guidArray[0];
                Guid roleId = guidArray[1]; 
                userInfo = context.UserLogins
                        .FirstOrDefault(x => x.Guid == userId);

                if (userInfo != null)
                {
                    var roles = new List<string>();
                    if (userInfo.UserTypeId == (int)Core.Enum.UsersLoginType.Test)
                    {
                        var projectStaffMemberRoles = TestEnvironment_GetProjectStaffMembers(roleId);
                        if (projectStaffMemberRoles != null)
                        {
                            var projectRole = projectStaffMemberRoles.FirstOrDefault(x => x.StaffGuid == userInfo.Guid);
                            string role = string.Empty;
                            try { role = userInfo.UserRoles.FirstOrDefault() != null ? userInfo.UserRoles.Select(x => x.Role.Name).FirstOrDefault() : string.Empty; } catch (Exception exc) { Console.WriteLine(exc); }
                            roles.Add(projectRole != null ? projectRole.Role : role);
                        }
                    }
                    else
                    {
                        var projectStatus = context.FormDataEntries.FirstOrDefault(x => x.Guid == roleId);
                        if (projectStatus.ProjectDeployStatus == (int)Core.Enum.ProjectStatusTypes.Published)
                        {
                            var projectStaffMemberRoles = GetProjectStaffMembers(roleId);
                            if (projectStaffMemberRoles != null)
                            {
                                var projectRole = projectStaffMemberRoles.FirstOrDefault(x => x.StaffGuid == userInfo.Guid);
                                if (projectRole == null)
                                {
                                    try
                                    {
                                        projectRole = userInfo.ProjectStaffMemberRoles.Where(x => x.UserId == userInfo.Id).Select(c => new Core.ViewModels.MongoViewModels.ProjectStaffMongo()
                                        {
                                            Role = c.Role.Name,
                                            StaffGuid = userInfo.Guid,
                                            StaffName = userInfo.FirstName + " " + userInfo.LastName,
                                        }).FirstOrDefault();
                                    }
                                    catch (Exception d)
                                    { }
                                }
                                string role = string.Empty;
                                try { role = userInfo.UserRoles.FirstOrDefault() != null ? userInfo.UserRoles.Select(x => x.Role.Name).FirstOrDefault() : string.Empty; } catch (Exception exc) { Console.WriteLine(exc); }

                                roles.Add(projectRole != null ? projectRole.Role : role);
                            }
                        }
                        else
                        {
                            var projectRole = context.ProjectStaffMemberRoles.FirstOrDefault(x => x.FormDataEntry.Guid == roleId && x.UserId == userInfo.Id);

                            string role = string.Empty;
                            try { role = userInfo.UserRoles.FirstOrDefault() != null ? userInfo.UserRoles.Select(x => x.Role.Name).FirstOrDefault() : string.Empty; } catch (Exception exc) { Console.WriteLine(exc); }
                            var roleList = projectRole != null ? projectRole.Role.Name : role;
                            roles.Add(roleList);
                        }
                    }
                    return new RoleViewModel()
                    {
                        Email = userInfo.Email,
                        Id = userInfo.Id,
                        Guid = userInfo.Guid,
                        Name = userInfo.FirstName + " " + userInfo.LastName,
                        Roles = roles,
                        TenantId = userInfo.Tenant.Guid
                    };
                }
                else
                {
                    return null;
                }
            }
        }


        public List<Core.ViewModels.MongoViewModels.ProjectStaffMongo> GetProjectStaffMembers(Guid projectId)
        {
            Data.MongoDB.MongoDBContext _mongoDBContext = new Data.MongoDB.MongoDBContext();
            Core.ViewModels.MongoViewModels.ProjectDeployViewModel model = new Core.ViewModels.MongoViewModels.ProjectDeployViewModel();
            var document = _mongoDBContext._database.GetCollection<Core.ViewModels.MongoViewModels.ProjectDeployViewModel>("DeployedProjects");
            var projectDetailscount = document.FindAs<Core.ViewModels.MongoViewModels.ProjectDeployViewModel>(MongoDB.Driver.Builders.Query.EQ("ProjectGuid", projectId)).Count();
            if (projectDetailscount > 0)
            {
                var userObjectid = MongoDB.Driver.Builders.Query<Core.ViewModels.MongoViewModels.ProjectDeployViewModel>.EQ(q => q.ProjectGuid, projectId);
                var project = _mongoDBContext._database.GetCollection<Core.ViewModels.MongoViewModels.ProjectDeployViewModel>("DeployedProjects").Find(userObjectid).SetSortOrder("ProjectDeployDate").OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                return project.ProjectStaffListMongo;
            }
            return null;
        }

        public List<Core.ViewModels.MongoViewModels.ProjectStaffMongo> TestEnvironment_GetProjectStaffMembers(Guid projectId)
        {
            Data.MongoDB.TestMongoDBContext _testMongoDBContext = new Data.MongoDB.TestMongoDBContext();
            Core.ViewModels.MongoViewModels.ProjectDeployViewModel model = new Core.ViewModels.MongoViewModels.ProjectDeployViewModel();
            var document = _testMongoDBContext._database.GetCollection<Core.ViewModels.MongoViewModels.ProjectDeployViewModel>("DeployedProjects");
            var projectDetailscount = document.FindAs<Core.ViewModels.MongoViewModels.ProjectDeployViewModel>(MongoDB.Driver.Builders.Query.EQ("ProjectGuid", projectId)).Count();

            if (projectDetailscount > 0)
            {
                var userObjectid = MongoDB.Driver.Builders.Query<Core.ViewModels.MongoViewModels.ProjectDeployViewModel>.EQ(q => q.ProjectGuid, projectId);
                var project = _testMongoDBContext._database.GetCollection<Core.ViewModels.MongoViewModels.ProjectDeployViewModel>("DeployedProjects").Find(userObjectid).AsQueryable().OrderByDescending(c => c.ProjectDeployDate).FirstOrDefault();
                return project.ProjectStaffListMongo;
            }
            return null;
        }


        public RoleViewModel CheckLoginCredentialsFromAPI(string username, string password, bool isTestSite = false)
        {
            using (var context = new AspreeEntities())
            {
                UserLogin userInfo = null;
                if (isTestSite)
                    userInfo = context.UserLogins
                        .FirstOrDefault(x => x.UserName.ToLower() == username.ToLower() && x.AuthTypeId == (int)Core.Enum.AuthenticationTypes.Local_Password && x.UserTypeId == (int)Core.Enum.UsersLoginType.Test && x.IsApiAccessEnabled == true && x.IsUserApprovedBySystemAdmin && x.Status == (int)Core.Enum.Status.Active);
                else
                    userInfo = context.UserLogins
                    .FirstOrDefault(x => x.UserName.ToLower() == username.ToLower() && x.AuthTypeId == (int)Core.Enum.AuthenticationTypes.Local_Password && x.UserTypeId != (int)Core.Enum.UsersLoginType.Test && x.IsApiAccessEnabled == true && x.IsUserApprovedBySystemAdmin && x.Status == (int)Core.Enum.Status.Active);

                if (userInfo != null)
                {
                    string hash = _security.ComputeHash(password, userInfo.Salt);
                    if (hash != userInfo.Password)
                    {
                        userInfo = null;
                    }
                }
                if (userInfo != null)
                {
                    return new RoleViewModel()
                    {
                        Email = userInfo.Email,
                        Id = userInfo.Id,
                        Guid = userInfo.Guid,
                        Name = userInfo.FirstName + " " + userInfo.LastName,
                        Roles = userInfo.UserRoles.Select(x => x.Role.Name).ToList(),
                        TenantId = userInfo.Tenant.Guid
                    };
                }
                else
                {
                    return null;
                }
            }
        }

        public bool CheckIsUserApprovedBySystemAdmin(Guid userid, bool isTestSite = false)
        {
            using (var context = new AspreeEntities())
            {
                UserLogin userInfo = null;
                userInfo = context.UserLogins
                        .FirstOrDefault(x => x.Guid == userid && x.IsUserApprovedBySystemAdmin && x.Status == (int)Core.Enum.Status.Active);
                if (userInfo != null)
                {
                    if (userInfo.LoginAuthTypeMaster.Status != (int)Core.Enum.Status.Active)
                        return false;
                    if (userInfo.LoginAuthTypeMaster.DateDeactivated.HasValue)
                        return false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public bool UpdateLockedUserInfo(int userId, bool istestside = false)
        {
            MongoDBContext _mongoDBContext = new MongoDBContext();
            TestMongoDBContext _testMongoDBContext = new TestMongoDBContext();
            IMongoQuery condition = Query<FormDataEntryMongo>.EQ(q => q.ThisUserId, userId);
            IQueryable<FormDataEntryMongo> userEntities;
            if (istestside)
            {
                userEntities = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(condition).AsQueryable();
            }
            else
            {
                userEntities = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(condition).AsQueryable();
            }
            if (userEntities != null)
            {
                string[] defaultFormNames = new string[]
                {
                    EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration)
                    , EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration)
                    , EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration)
                };

                var entityCurrentEntity = userEntities.Where(x => defaultFormNames.Contains(x.FormTitle)).FirstOrDefault();
                if (entityCurrentEntity != null)
                {
                    var updateEntityQuery = Query<FormDataEntryMongo>.EQ(p => p.Id, entityCurrentEntity.Id);
                    FormDataEntryMongo updateEntityModel;
                    if (istestside)
                    {
                        updateEntityModel = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindOne(updateEntityQuery);
                    }
                    else
                    {
                        updateEntityModel = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindOne(updateEntityQuery);
                    }
                    if (updateEntityModel != null)
                    {
                        List<FormDataEntryVariableMongo> variableList = new List<FormDataEntryVariableMongo>();
                        FormDataEntryVariableMongo variableModel = new FormDataEntryVariableMongo();
                        var formVariables = updateEntityModel.formDataEntryVariableMongoList.AsQueryable();

                        formVariables.ToList().ForEach(frmVrbl =>
                        {
                            variableModel = frmVrbl;
                            #region Person Registration Form
                            if (updateEntityModel.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration))
                            {
                                if (frmVrbl.VariableName == DefaultsVariables.Active.ToString())
                                {
                                    variableModel.SelectedValues = "0";
                                }
                            }
                            #endregion

                            #region Participant Registration Form
                            if (updateEntityModel.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
                            {
                                if (frmVrbl.VariableName == DefaultsVariables.Active.ToString())
                                {
                                    variableModel.SelectedValues = "0";
                                }
                            }
                            #endregion

                            #region Place/Group Registration Form
                            if (updateEntityModel.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
                            {
                                if (frmVrbl.VariableName == DefaultsVariables.Active.ToString())
                                {
                                    variableModel.SelectedValues = "0";
                                }
                            }
                            #endregion

                            variableList.Add(variableModel);
                        });
                        updateEntityModel.formDataEntryVariableMongoList = variableList;

                        // Document Collections  
                        MongoCollection<FormDataEntryMongo> collection;
                        if (istestside)
                        {
                            collection = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities");
                        }
                        else
                        {
                            collection = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities");
                        }

                        //var collection = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities");
                        // Document Update which need Id and Data to Update  
                        var result = collection.Update(updateEntityQuery, Update.Replace(updateEntityModel), UpdateFlags.None);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
