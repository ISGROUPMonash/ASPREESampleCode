using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspree.Core.ViewModels;
using Aspree.Data;
using Aspree.Core.ViewModels.MongoViewModels;
using Aspree.Data.MongoDB;
using MongoDB.Driver.Builders;
using Aspree.Core.Enum;
using System.Security.Cryptography;

namespace Aspree.Provider.Provider
{
    public class UserLoginProvider : IUserLoginProvider
    {
        private readonly AspreeEntities dbContext;
        private readonly MongoDBContext _mongoDBContext;
        private readonly IRoleProvider _roleProvider;
        private readonly ISecurityQuestionProvider _securityQuestionProvider;
        private readonly ITenantProvider _tenantProvider;
        private readonly ISecurity _security = new Security();

        public UserLoginProvider(AspreeEntities _dbContext
            , ITenantProvider tenantProvider
            , MongoDBContext mongoDBContext
            )
        {
            this.dbContext = _dbContext;
            this._tenantProvider = tenantProvider;
            this._roleProvider = new RoleProvider(this.dbContext, this);
            this._securityQuestionProvider = new SecurityQuestionProvider(this.dbContext);
            this._mongoDBContext = mongoDBContext;
        }

        public RoleViewModel IsValidUser(string UserName, string Password)
        {

            var userInfo = this.dbContext.UserLogins
                .FirstOrDefault(x => x.Email.ToLower() == UserName.ToLower() && x.Password == Password);

            if (userInfo != null)
            {
                return new RoleViewModel()
                {
                    Email = userInfo.Email,
                    Id = userInfo.Id,
                    Guid = userInfo.Guid,
                    Name = userInfo.FirstName + " " + userInfo.LastName,
                    Roles = userInfo.UserRoles.Select(x => x.Role.Name).ToList()
                };
            }

            return null;
        }


        public UserLoginViewModel Create(UserLoginViewModel model)
        {
            if (this.dbContext.UserLogins.Any(vc => vc.UserName.ToLower() == model.UserName.ToLower() && vc.AuthTypeId == model.AuthTypeId))
            {
                throw new Core.AlreadyExistsException("Username already exist.", "Username");
            }

            var createdBy = this.GetByGuid(model.CreatedBy);
            var tenant = _tenantProvider.GetByGuid(model.TenantId);

            String salt = Guid.NewGuid().ToString().Replace("-", "");
            string password = _security.ComputeHash((new Random()).Next(10000000, 99999999).ToString(), salt);

            var newUser = new UserLogin()
            {
                Email = model.Email,
                Address = model.Address,
                Answer = model.Answer,
                AuthTypeId = model.AuthTypeId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Mobile = model.Mobile,
                TenantId = tenant.Id,   
                Salt = salt,
                Password = password,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = createdBy.Id,
                Guid = Guid.NewGuid(),
                Status = model.Status,
                TempGuid = Guid.NewGuid(),
                UserTypeId = model.UserTypeId,
                UserName = model.UserName,
                IsApiAccessEnabled = model.IsApiAccessEnabled,
                IsUserApprovedBySystemAdmin = model.IsUserApprovedBySystemAdmin,
            };

            this.dbContext.UserLogins.Add(newUser);

            SaveChanges();

            var role = _roleProvider.GetByGuid(model.RoleId);

            this.dbContext.UserRoles.Add(new UserRole()
            {
                UserId = newUser.Id,
                RoleId = role.Id,
                Guid = Guid.NewGuid()
            });

            SaveChanges();

            return GetByGuid(newUser.Guid);
        }

        public UserLoginViewModel DeleteByGuid(Guid guid, Guid DeletedBy)
        {
            var deactivatedBy = this.GetByGuid(DeletedBy);
            var userLogin = this.dbContext.UserLogins
                .FirstOrDefault(vc => vc.Guid == guid);

            if (userLogin != null)
            {
                if (userLogin.DateDeactivated.HasValue)
                {
                    userLogin.DateDeactivated = null;
                    userLogin.DeactivatedBy = null;
                }
                else
                {
                    userLogin.DateDeactivated = DateTime.UtcNow;
                    userLogin.DeactivatedBy = deactivatedBy.Id;
                }
                SaveChanges();
                return ToModel(userLogin);
            }
            return null;
        }

        public UserLoginViewModel DeleteById(int id, Guid DeletedBy)
        {
            var deactivatedBy = this.GetByGuid(DeletedBy);
            var userLogin = this.dbContext.UserLogins
                .FirstOrDefault(vc => vc.Id == id);

            if (userLogin != null)
            {
                if (userLogin.DateDeactivated.HasValue)
                {
                    userLogin.DateDeactivated = null;
                    userLogin.DeactivatedBy = null;
                }
                else
                {
                    userLogin.DateDeactivated = DateTime.UtcNow;
                    userLogin.DeactivatedBy = deactivatedBy.Id;
                }
                SaveChanges();
                return ToModel(userLogin);
            }
            return null;
        }

        public IEnumerable<UserLoginViewModel> GetAll()
        {
            return this.dbContext.UserLogins
                .OrderByDescending(d => d.Id)
                .Select(ToModel)
                .ToList();
        }

        public IEnumerable<UserLoginViewModel> GetAll(Guid tenantId)
        {
            return this.dbContext.UserLogins
                .Where(u => u.Tenant.Guid == tenantId && u.UserTypeId != (int)Core.Enum.UsersLoginType.Test)
                .OrderByDescending(d => d.Id)
                .Select(ToModel)
                .ToList();
        }

        public UserLoginViewModel GetByGuid(Guid guid)
        {
            var userLogin = dbContext.UserLogins
                .FirstOrDefault(ul => ul.Guid == guid);

            if (userLogin != null)
                return ToModel(userLogin);

            return null;
        }

        public UserLoginViewModel GetById(int id)
        {
            var userLogin = dbContext.UserLogins
                .FirstOrDefault(ul => ul.Id == id);

            if (userLogin != null)
                return ToModel(userLogin);

            return null;
        }

        public List<string> GetUserRolesByEmail(string email)
        {
            var userLogin = dbContext.UserLogins
                .FirstOrDefault(ul => ul.Email == email && !ul.DateDeactivated.HasValue);

            if (userLogin != null)
            {
                return userLogin.UserRoles.Select(ul => ul.Role.Name).ToList();
            }

            return null;
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        public UserLoginViewModel ToModel(UserLogin entity)
        {
            var createdBy = dbContext.UserLogins
                .FirstOrDefault(ul => ul.Id == entity.CreatedBy);
            var modifiedBy = entity.ModifiedBy.HasValue ? dbContext.UserLogins
                .FirstOrDefault(ul => ul.Id == entity.ModifiedBy.Value) : null;
            var deactivatedBy = entity.DeactivatedBy.HasValue ? dbContext.UserLogins
                .FirstOrDefault(ul => ul.Id == entity.DeactivatedBy.Value) : null;

            return new UserLoginViewModel
            {
                Address = entity.Address,
                Answer = entity.Answer,
                AuthTypeId = entity.AuthTypeId,
                CreatedBy = createdBy.Guid,
                CreatedDate = entity.CreatedDate,
                DateDeactivated = entity.DateDeactivated,
                DeactivatedBy = deactivatedBy == null ? (Guid?)null : deactivatedBy.Guid,
                Email = entity.Email,
                FirstName = entity.FirstName,
                Guid = entity.Guid,
                Id = entity.Id,
                LastName = entity.LastName,
                Mobile = entity.Mobile,
                ModifiedBy = modifiedBy == null ? (Guid?)null : modifiedBy.Guid,
                ModifiedDate = entity.ModifiedDate,
                Password = entity.Password,
                SecurityQuestionId = entity.SecurityQuestionId.HasValue ? entity.SecurityQuestion.Guid : Guid.Empty,
                Salt = entity.Salt = entity.Salt,
                TenantId = entity.Tenant.Guid,
                RoleId = entity.UserRoles.Any() ? entity.UserRoles.First().Role.Guid : Guid.Empty,
                RoleName = entity.UserRoles.Any() ? entity.UserRoles.First().Role.Name : string.Empty,
                Status = entity.Status,
                TempGuid = entity.TempGuid.HasValue ? entity.TempGuid : Guid.Empty,
                UserName = entity.UserName,
                IsUserApprovedBySystemAdmin = entity.IsUserApprovedBySystemAdmin,
                UserTypeId = entity.UserTypeId
            };
        }

        public UserLoginViewModel Update(UserLoginViewModel model)
        {
            if (this.dbContext.UserLogins.Any(vc => vc.UserName.ToLower() == model.UserName.ToLower() && vc.LoginAuthTypeMaster.Id == model.AuthTypeId
              && vc.Guid != model.Guid))
            {
                throw new Core.AlreadyExistsException("Username already exist.", "Username");
            }

            var editUser = this.dbContext.UserLogins
                .FirstOrDefault(vc => vc.Guid == model.Guid);

            var modifieddBy = this.GetByGuid((Guid)model.ModifiedBy);
            var tenant = _tenantProvider.GetByGuid(model.TenantId);

            editUser.Email = model.Email;
            editUser.FirstName = model.FirstName;
            editUser.LastName = model.LastName;
            editUser.Mobile = model.Mobile;

            if (model.Status == (int)Core.Enum.Status.Active)
            {
                editUser.LoginFailedAttemptCount = null;
                editUser.LoginFailedAttemptDate = null;
            }

            editUser.Status = model.Status;
            editUser.ModifiedDate = DateTime.UtcNow;
            editUser.ModifiedBy = modifieddBy.Id;
            editUser.UserName = model.UserName;
            editUser.IsApiAccessEnabled = model.IsApiAccessEnabled;
            editUser.IsUserApprovedBySystemAdmin = model.IsUserApprovedBySystemAdmin;

            if (model.AuthTypeId != (int)Core.Enum.AuthenticationTypes.Local_Password)
            {
                editUser.IsMailSend = null;
            }
            SaveChanges();

            var role = _roleProvider.GetByGuid(model.RoleId);

            this.dbContext.UserRoles.RemoveRange(editUser.UserRoles.ToList());

            this.dbContext.UserRoles.Add(new UserRole()
            {
                UserId = editUser.Id,
                RoleId = role.Id,
                Guid = Guid.NewGuid()
            });

            SaveChanges();

            return ToModel(editUser);
        }

        public bool ChangePassword(Guid guid, ChangePassword changePassword)
        {
            var user = this.dbContext.UserLogins.FirstOrDefault(ul => ul.Guid == guid);

            if (user == null)
            {
                throw new Core.NotFoundException("User was not found");
            }

            string hash = _security.ComputeHash(changePassword.OldPassword, user.Salt);
            if (hash == user.Password)
            {

                String salt = Guid.NewGuid().ToString().Replace("-", "");
                string password = _security.ComputeHash(changePassword.Password, user.Salt);

                if (user.Password != password)
                {
                    String salt1 = Guid.NewGuid().ToString().Replace("-", "");
                    string password1 = _security.ComputeHash(changePassword.Password, salt1);
                    user.Password = password1;
                    user.Salt = salt1;
                    SaveChanges();

                    return true;
                }
                else
                {
                    throw new Core.NotFoundException("New password cannot be the same as your previous password.");
                }
            }
            else
            {
                throw new Core.BadRequestException("Incorrect Old Password", "OldPassword");
            }
        }

        public bool ResetPassword(ResetPassword resetPassword, bool isTestSite = false)
        {
            UserLogin user = null;
            if (isTestSite)
            {
                user = this.dbContext.UserLogins.FirstOrDefault(ul => ul.Guid == resetPassword.Guid && ul.UserTypeId == (int)UsersLoginType.Test);
            }
            else
            {
                user = this.dbContext.UserLogins.FirstOrDefault(ul => ul.Guid == resetPassword.Guid && ul.UserTypeId != (int)UsersLoginType.Test);
            }
            if (user == null)
            {
                throw new Core.NotFoundException("User was not found!");
            }
            if (user != null && user.SecurityQuestion.Guid != resetPassword.QuestionGuid)
            {
                throw new Core.NotFoundException("Incorrect security question or answer!");
            }
            if (user != null && user.Answer != resetPassword.Answer)
            {
                throw new Core.NotFoundException("Incorrect security question or answer!");
            }
            var sq = this.dbContext.SecurityQuestions.FirstOrDefault(s => s.Guid == resetPassword.QuestionGuid);
            String salt = Guid.NewGuid().ToString().Replace("-", "");
            string password = _security.ComputeHash(resetPassword.Password, user.Salt);
            if (user.Password != password)
            {
                password = _security.ComputeHash(resetPassword.Password, salt);
                user.Password = password;
                user.Salt = salt;
                user.Answer = resetPassword.Answer;
                user.SecurityQuestionId = sq.Id;
                user.TempGuid = null;
                SaveChanges();
                return true;
            }
            else
            {
                throw new Core.NotFoundException("New password cannot be the same as your previous password.");
            }
        }

        public bool ResetNewPassword(ResetPassword resetPassword, bool isTestSite = false)
        {
            var user = this.dbContext.UserLogins.FirstOrDefault(ul => ul.Guid == resetPassword.Guid);

            if (user == null)
            {
                throw new Core.NotFoundException("User was not found!");
            }
            var sq = this.dbContext.SecurityQuestions.FirstOrDefault(s => s.Guid == resetPassword.QuestionGuid);
            String salt = Guid.NewGuid().ToString().Replace("-", "");
            string password = _security.ComputeHash(resetPassword.Password, user.Salt);
            if (user.Password != password)
            {
                password = _security.ComputeHash(resetPassword.Password, salt);
                user.Password = password;
                user.Salt = salt;
                user.Answer = resetPassword.Answer;
                user.SecurityQuestionId = sq.Id;
                user.TempGuid = null;
                SaveChanges();

                return true;
            }
            else
            {
                throw new Core.NotFoundException("New password cannot be the same as your previous password.");
            }
        }

        public bool EditSecurityQuestion(UpdateUserSecurityQuestion securityQuestion)
        {
            var user = this.dbContext.UserLogins.FirstOrDefault(ul => ul.Guid == securityQuestion.UserGuid);
            if (user == null)
            {
                throw new Core.NotFoundException("User was not found");
            }
            var sq = this.dbContext.SecurityQuestions.FirstOrDefault(s => s.Guid == securityQuestion.Guid);
            user.SecurityQuestionId = sq.Id;
            user.Answer = securityQuestion.Answer;
            SaveChanges();
            return true;
        }

        public bool ChangeStatus(Guid userGuid, int newStatus)
        {
            var user = this.dbContext.UserLogins.FirstOrDefault(ul => ul.Guid == userGuid);
            if (user == null)
            {
                throw new Core.NotFoundException("User was not found");
            }
            user.Status = newStatus;
            SaveChanges();
            return true;
        }

        public RoleViewModel CheckLoginCredentials(string username, string password)
        {
            UserLogin userInfo = dbContext.UserLogins.FirstOrDefault(x => x.Email.ToLower() == username);
            if (userInfo != null)
            {
                string hash = _security.ComputeHash(password, userInfo.Salt);
                if (hash == userInfo.Password)
                {
                    if (userInfo != null)
                    {
                        return new RoleViewModel()
                        {
                            Email = userInfo.Email,
                            Id = userInfo.Id,
                            Guid = userInfo.Guid,
                            Name = userInfo.FirstName + " " + userInfo.LastName,
                            Roles = userInfo.UserRoles.Select(x => x.Role.Name).ToList()
                        };
                    }
                }
                else
                {
                    return null;
                }

            }
            else
            {
                return null;
            }
            return null;
        }

        public UserLoginViewModel GetUserByEmailId(string EmailId)
        {
            var user = dbContext.UserLogins.FirstOrDefault(x => x.Email == EmailId);
            if (user != null)
                return ToModel(user);
            return null;
        }

        public SecurityQuestionViewModel GetUserByUsername(string username)
        {
            var user = dbContext.UserLogins.FirstOrDefault(x => x.UserName == username && x.AuthTypeId == (int)Core.Enum.AuthenticationTypes.Local_Password);
            if (user != null)
                return ToSecurityQuestionViewModel(user);
            return null;
        }
        public SecurityQuestionViewModel ToSecurityQuestionViewModel(UserLogin entity)
        {
            return new SecurityQuestionViewModel
            {
                Question = entity.SecurityQuestionId.HasValue ? entity.SecurityQuestion.Question : string.Empty,
                Guid = entity.SecurityQuestionId.HasValue ? entity.SecurityQuestion.Guid : Guid.Empty,
                Id = entity.SecurityQuestionId.HasValue ? entity.SecurityQuestion.Id : 0,
            };
        }

        public UserLoginViewModel GetByTempGuid(Guid guid, bool isTestSite = false)
        {
            UserLogin userLogin = null;
            if (isTestSite)
            {
                userLogin = dbContext.UserLogins
                .FirstOrDefault(ul => ul.TempGuid == guid && ul.UserTypeId == (int)UsersLoginType.Test);
            }
            else
            {
                userLogin = dbContext.UserLogins
                .FirstOrDefault(ul => ul.TempGuid == guid && ul.UserTypeId != (int)UsersLoginType.Test);
            }

            if (userLogin != null)
            {
                DateTime creationDate = userLogin.ModifiedDate != null ? (DateTime)userLogin.ModifiedDate : userLogin.CreatedDate;
                DateTime validTill = creationDate.AddHours(24);
                DateTime currenttime = DateTime.UtcNow;
                if (currenttime > validTill)
                {
                    userLogin = null;
                }
            }
            if (userLogin != null)
                return ToModel(userLogin);

            return null;
        }

        public bool updateTempGuid(Guid userid)
        {
            var user = dbContext.UserLogins.FirstOrDefault(x => x.Guid == userid);
            if (user != null)
            {
                user.Answer = null;
                user.SecurityQuestionId = null;
                user.TempGuid = Guid.NewGuid();
                SaveChanges();
                return true;
            }
            return false;
        }


        public IEnumerable<UserLoginViewModel> GetProjectAllUsers(Guid projectId)
        {
            List<UserLoginViewModel> resultModelList = new List<UserLoginViewModel>();
            UserLoginViewModel resultModel = new UserLoginViewModel();
            var projectUsers = this.dbContext.ProjectStaffMemberRoles
                .Where(u => u.FormDataEntry.Guid == projectId && u.UserLogin.Status == (int)Core.Enum.Status.Active && u.UserLogin.UserTypeId != (int)Core.Enum.UsersLoginType.Test)
                .OrderByDescending(d => d.Id);
            projectUsers.ToList().ForEach(prouser =>
            {
                if (prouser.IsActiveProjectUser == true)
                {
                    var userProjectJoinDate = Convert.ToDateTime(prouser.ProjectJoinedDate);
                    if (DateTime.Now.Date >= userProjectJoinDate.Date)
                    {
                        resultModel = new UserLoginViewModel
                        {
                            FirstName = prouser.UserLogin != null ? prouser.UserLogin.FirstName : "",
                            LastName = prouser.UserLogin != null ? prouser.UserLogin.LastName : "",
                            Guid = prouser.UserLogin != null ? prouser.UserLogin.Guid : Guid.Empty,
                            RoleId = prouser.Role.Guid,
                            RoleName = prouser.Role.Name,
                            UserTypeId = prouser.UserLogin.UserTypeId,
                        };
                        resultModelList.Add(resultModel);
                    }
                }
                else
                {
                    var userProjectJoinDate = Convert.ToDateTime(prouser.ProjectJoinedDate);
                    var userProjectLeftDate = Convert.ToDateTime(prouser.ProjectLeftDate);
                    if (DateTime.Now.Date >= userProjectJoinDate.Date && DateTime.Now.Date < userProjectLeftDate.Date)
                    {
                        resultModel = new UserLoginViewModel
                        {
                            FirstName = prouser.UserLogin != null ? prouser.UserLogin.FirstName : "",
                            LastName = prouser.UserLogin != null ? prouser.UserLogin.LastName : "",
                            Guid = prouser.UserLogin != null ? prouser.UserLogin.Guid : Guid.Empty,
                            RoleId = prouser.Role.Guid,
                            RoleName = prouser.Role.Name,
                            UserTypeId = prouser.UserLogin.UserTypeId,
                        };
                        resultModelList.Add(resultModel);
                    }
                }
            });

            #region All SA users
            try
            {
                var allSAUsers = dbContext.UserRoles.Where(x => x.Role.Name == RoleTypes.System_Admin.ToString().Replace("_", " ") && x.UserLogin.UserTypeId != (int)UsersLoginType.Test && x.UserLogin.Status == (int)Core.Enum.Status.Active).ToList();
                allSAUsers.ForEach(sa_user =>
                {
                    var isExest = resultModelList.FirstOrDefault(x => x.Guid == sa_user.UserLogin.Guid);
                    if (isExest == null && sa_user.UserLogin.Status == (int)Core.Enum.Status.Active)
                    {
                        resultModelList.Add(new UserLoginViewModel
                        {
                            FirstName = sa_user.UserLogin.FirstName,
                            LastName = sa_user.UserLogin.LastName,
                            Guid = sa_user.UserLogin.Guid,
                            RoleId = sa_user.Role.Guid,
                            RoleName = sa_user.Role.Name,
                            UserTypeId = sa_user.UserLogin.UserTypeId,
                        });
                    }
                });
            }
            catch (Exception exc) { }
            #endregion

            return resultModelList.OrderBy(x => x.FirstName);
        }

        public IEnumerable<UserLoginViewModel> TestEnvironment_GetProjectAllUsers(Guid projectId)
        {
            List<UserLoginViewModel> resultModelList = new List<UserLoginViewModel>();
            UserLoginViewModel resultModel = new UserLoginViewModel();

            var projectUsers = this.dbContext.ProjectStaffMemberRoles
                .Where(u => u.FormDataEntry.Guid == projectId && u.UserLogin.Status == (int)Core.Enum.Status.Active && u.UserLogin.UserTypeId == (int)UsersLoginType.Test)
                .OrderByDescending(d => d.Id);
            projectUsers.ToList().ForEach(prouser =>
            {
                if (prouser.IsActiveProjectUser == true)
                {
                    var userProjectJoinDate = Convert.ToDateTime(prouser.ProjectJoinedDate);
                    if (DateTime.Now.Date >= userProjectJoinDate.Date)
                    {
                        resultModel = new UserLoginViewModel
                        {
                            FirstName = prouser.UserLogin != null ? prouser.UserLogin.FirstName : "",
                            LastName = prouser.UserLogin != null ? prouser.UserLogin.LastName : "",
                            Guid = prouser.UserLogin != null ? prouser.UserLogin.Guid : Guid.Empty,
                            RoleId = prouser.Role.Guid,
                            RoleName = prouser.Role.Name,
                            UserTypeId = prouser.UserLogin.UserTypeId,
                        };
                        resultModelList.Add(resultModel);
                    }
                }
                else
                {
                    var userProjectJoinDate = Convert.ToDateTime(prouser.ProjectJoinedDate);
                    var userProjectLeftDate = Convert.ToDateTime(prouser.ProjectLeftDate);

                    if (DateTime.Now.Date >= userProjectJoinDate.Date && DateTime.Now.Date < userProjectLeftDate.Date)
                    {
                        resultModel = new UserLoginViewModel
                        {
                            FirstName = prouser.UserLogin != null ? prouser.UserLogin.FirstName : "",
                            LastName = prouser.UserLogin != null ? prouser.UserLogin.LastName : "",
                            Guid = prouser.UserLogin != null ? prouser.UserLogin.Guid : Guid.Empty,
                            RoleId = prouser.Role.Guid,
                            RoleName = prouser.Role.Name,
                            UserTypeId = prouser.UserLogin.UserTypeId,
                        };
                        resultModelList.Add(resultModel);
                    }
                }
            });

            #region All SA users
            try
            {
                var allSAUsers = dbContext.UserRoles.Where(x => x.Role.Name == RoleTypes.System_Admin.ToString().Replace("_", " ") && x.UserLogin.UserTypeId == (int)UsersLoginType.Test && x.UserLogin.Status == (int)Core.Enum.Status.Active).ToList();
                allSAUsers.ForEach(sa_user =>
                {
                    var isExest = resultModelList.FirstOrDefault(x => x.Guid == sa_user.UserLogin.Guid);
                    if (isExest == null && sa_user.UserLogin.Status == (int)Core.Enum.Status.Active)
                    {
                        resultModelList.Add(new UserLoginViewModel
                        {
                            FirstName = sa_user.UserLogin.FirstName,
                            LastName = sa_user.UserLogin.LastName,
                            Guid = sa_user.UserLogin.Guid,
                            RoleId = sa_user.Role.Guid,
                            RoleName = sa_user.Role.Name,
                            UserTypeId = sa_user.UserLogin.UserTypeId,
                        });
                    }
                });
            }
            catch (Exception exc) { }
            #endregion

            return resultModelList.OrderBy(x => x.FirstName);
        }


        public bool checkUsernameExist(string username, Guid authType, Guid? userid)
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

        public bool check_IsMailSend(bool isTestSite, string username, int? userid)
        {
            bool result = false;
            int usertype = isTestSite ? (int)Core.Enum.UsersLoginType.Test : (int)Core.Enum.UsersLoginType.Entity;

            UserLogin usrlogin = null;
            if (userid != null)
                usrlogin = dbContext.UserLogins.FirstOrDefault(x => x.Id == userid && x.UserTypeId == usertype && x.AuthTypeId == (int)Core.Enum.AuthenticationTypes.Local_Password);
            else
                usrlogin = dbContext.UserLogins.FirstOrDefault(x => x.UserName == username && x.UserTypeId == usertype && x.AuthTypeId == (int)Core.Enum.AuthenticationTypes.Local_Password);

            if (usrlogin != null)
                result = usrlogin.IsMailSend != null ? (bool)usrlogin.IsMailSend : false;

            return result;
        }
        public bool UpdateIsMailSend(int userid)
        {
            UserLogin userLogin = dbContext.UserLogins.FirstOrDefault(x => x.Id == userid);
            if (userLogin != null)
            {
                userLogin.IsMailSend = true;
                SaveChanges();
                return true;
            }
            return false;
        }

        public Guid UpdateTempGuid(int id)
        {
            UserLogin userlogin = dbContext.UserLogins.FirstOrDefault(x => x.Id == id);
            if (userlogin != null)
            {
                userlogin.TempGuid = Guid.NewGuid();
                SaveChanges();
                return (Guid)userlogin.TempGuid;
            }
            return Guid.Empty;
        }
        public string GetUserRoleByProjectId(Guid userId, Guid projectId)
        {
            string userRole = string.Empty;
            ProjectStaffMemberRole role = dbContext.ProjectStaffMemberRoles.FirstOrDefault(x => x.UserLogin.Guid == userId && x.FormDataEntry.Guid == projectId);
            if (role != null)
            {
                userRole = role.Role.Name;
            }
            else
            {
                UserRole systemRole = dbContext.UserRoles.FirstOrDefault(x => x.UserLogin.Guid == userId);
                if (systemRole != null)
                {
                    if (systemRole.Role.Name == Core.Enum.RoleTypes.System_Admin.ToString().Replace("_", " "))
                    {
                        userRole = systemRole.Role.Name;
                    }
                }
            }
            if (userRole == string.Empty)
            {
                var condition = MongoDB.Driver.Builders.Query<ProjectDeployViewModel>.EQ(q => q.ProjectGuid, projectId);
                var project = _mongoDBContext._database.GetCollection<ProjectDeployViewModel>("DeployedProjects").Find(condition).SetSortOrder("ProjectDeployDate").OrderByDescending(x => x.ProjectDeployDate).FirstOrDefault();
                if (project != null)
                {
                    ProjectStaffMongo staff = project.ProjectStaffListMongo.FirstOrDefault(x => x.StaffGuid == projectId);
                    userRole = staff != null ? staff.Role : string.Empty;
                }
            }
            return userRole;
        }



        public IEnumerable<SystemAdminToolsUserViewModel> GetAllSystemAdminToolsUser(Guid tenantId, Guid projectId)
        {
            IEnumerable<UserLogin> userList = this.dbContext.UserLogins
                .Where(u => u.Tenant.Guid == tenantId && u.UserTypeId != (int)Core.Enum.UsersLoginType.Test && u.Status == (int)Core.Enum.Status.Active)
                .OrderByDescending(d => d.Id);
            return ToSystemAdminToolsUserViewModel(userList, projectId);
        }
        public IEnumerable<SystemAdminToolsUserViewModel> ToSystemAdminToolsUserViewModel(IEnumerable<UserLogin> user, Guid projectId)
        {
            string[] defaultActivities = new string[] {
                EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration)
                ,EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration)
                ,EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration)
            };
            IQueryable<FormDataEntry> usersSQLList = dbContext.FormDataEntries.Where(x => defaultActivities.Contains(x.Activity.ActivityName) && x.ProjectDeployedId == null).AsQueryable();
            IQueryable<FormDataEntryMongo> usersMongoList = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindAll().AsQueryable().Where(x => defaultActivities.Contains(x.ActivityName));
            List<SystemAdminToolsUserViewModel> list = new List<SystemAdminToolsUserViewModel>();
            SystemAdminToolsUserViewModel model = new SystemAdminToolsUserViewModel();

            FormDataEntry userSQL = null;
            FormDataEntryMongo userMongo = null;
            user.ToList().ForEach(u =>
            {
                Guid formID = Guid.Empty;
                int? entityID = null;

                userSQL = usersSQLList.FirstOrDefault(x => x.ThisUserId == u.Id);
                if (userSQL != null)
                {
                    entityID = userSQL.EntityNumber;
                    formID = userSQL.Form.Guid;
                }
                else
                {
                    userMongo = usersMongoList.FirstOrDefault(x => x.ThisUserId == u.Id);
                    if (userMongo != null)
                    {
                        entityID = (int)userMongo.EntityNumber;
                        formID = userMongo.FormGuid;
                    }
                }
                model = new SystemAdminToolsUserViewModel();
                model.FirstName = u.FirstName;
                model.LastName = u.LastName;
                model.Email = u.Email;
                model.Mobile = u.Mobile;
                model.RoleName = u.UserRoles.FirstOrDefault().Role.Name == "System Admin" ? u.UserRoles.FirstOrDefault().Role.Name : this.GetUserRoleByProjectId(u);
                model.Status = u.Status != null ? (int)u.Status : (int)Core.Enum.Status.InActive;
                model.Address = u.Address;
                model.CreatedDate = u.CreatedDate;
                model.ModifiedDate = u.ModifiedDate;
                model.UserName = u.UserName;
                model.Guid = u.Guid;
                model.Id = u.Id;
                model.EntID = entityID == null ? " " : Convert.ToInt64(entityID).ToString("D7");
                model.FormId = formID;
                model.ProjectId = projectId;
                list.Add(model);
            });
            return list;
        }


        public SystemAdminToolsUserViewModel ToSystemAdminToolsUserViewModel(UserLogin user)
        {
            return new SystemAdminToolsUserViewModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Mobile = user.Mobile,
                RoleName = user.UserRoles.FirstOrDefault().Role.Name == "System Admin" ? user.UserRoles.FirstOrDefault().Role.Name : this.GetUserRoleByProjectId(user),
                Status = user.Status != null ? (int)user.Status : (int)Core.Enum.Status.InActive,
                Address = user.Address,
                CreatedDate = user.CreatedDate,
                ModifiedDate = user.ModifiedDate,
                UserName = user.UserName,
                Guid = user.Guid,
                Id = user.Id,
                EntID = user.Id.ToString("D7"),
            };
        }

        public string GetUserRoleByProjectId(UserLogin user)
        {
            string userRole = string.Empty;

            var entityCondition = Query<FormDataEntryMongo>.EQ(q => q.ThisUserGuid, user.Guid);
            var entity = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(entityCondition).FirstOrDefault();

            if (entity != null)
            {
                if (entity.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
                {
                    userRole = "Participant";
                }
                else
                {
                    var sysRole = entity.formDataEntryVariableMongoList.FirstOrDefault(x => x.VariableName == Core.Enum.DefaultsVariables.SysRole.ToString());
                    if (sysRole != null)
                    {
                        userRole = !string.IsNullOrEmpty(sysRole.SelectedValues) ? (sysRole.SelectedValues == "1" ? RoleTypes.System_Admin.ToString().Replace("_", " ") : "User") : "User";
                    }
                }
            }
            else
            {
                var sqlEntity = dbContext.FormDataEntries.FirstOrDefault(x => x.ThisUserId == user.Id);
                if (sqlEntity != null)
                {
                    if (sqlEntity.Form.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
                    {
                        userRole = "Participant";
                    }
                    else
                    {
                        var sysRole = sqlEntity.FormDataEntryVariables.FirstOrDefault(x => x.VariableId == 43);
                        if (sysRole != null)
                        {
                            userRole = !string.IsNullOrEmpty(sysRole.SelectedValues) ? (sysRole.SelectedValues == "1" ? RoleTypes.System_Admin.ToString().Replace("_", " ") : "User") : "User";
                        }
                    }
                }
            }
            return userRole;
        }

        public UserLoginViewModel UpdateMyProfile(UserLoginViewModel model, bool isTestSite = false)
        {
            if (isTestSite)
            {
                if (this.dbContext.UserLogins.Any(vc => vc.UserName.ToLower() == model.UserName.ToLower() && vc.LoginAuthTypeMaster.Id == model.AuthTypeId && vc.UserTypeId == (int)UsersLoginType.Test
                  && vc.Guid != model.Guid))
                {
                    throw new Core.AlreadyExistsException("Username already exist.", "Username");
                }
            }
            else
            {
                if (this.dbContext.UserLogins.Any(vc => vc.UserName.ToLower() == model.UserName.ToLower() && vc.LoginAuthTypeMaster.Id == model.AuthTypeId && vc.UserTypeId != (int)UsersLoginType.Test
              && vc.Guid != model.Guid))
                {
                    throw new Core.AlreadyExistsException("Username already exist.", "Username");
                }
            }

            var editUser = this.dbContext.UserLogins
                .FirstOrDefault(vc => vc.Guid == model.Guid);

            var modifieddBy = this.GetByGuid((Guid)model.ModifiedBy);

            editUser.FirstName = model.FirstName;
            editUser.LastName = model.LastName;
            editUser.Email = model.Email;
            editUser.Mobile = model.Mobile;
            editUser.Address = model.Address;
            editUser.Status = model.Status;
            editUser.ModifiedDate = DateTime.UtcNow;
            editUser.ModifiedBy = modifieddBy.Id;

            SaveChanges();

            var role = _roleProvider.GetByGuid(model.RoleId);
            this.dbContext.UserRoles.RemoveRange(editUser.UserRoles.ToList());
            this.dbContext.UserRoles.Add(new UserRole()
            {
                UserId = editUser.Id,
                RoleId = role.Id,
                Guid = Guid.NewGuid()
            });

            SaveChanges();

            if (isTestSite)
            {
                TestEnvironment_UpdateMyProfileDataInDataEntryMongo(editUser);
            }
            else
            {
                UpdateMyProfileDataInDataEntryMongo(editUser);
            }
            return ToModel(editUser);
        }


        public bool UpdateMyProfileDataInDataEntryMongo(UserLogin userlogin)
        {
            var checkInMongoEntityCondition = Query<FormDataEntryMongo>.EQ(q => q.ThisUserGuid, userlogin.Guid);
            IQueryable<FormDataEntryMongo> checkInMongo = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(checkInMongoEntityCondition).AsQueryable();

            if (checkInMongo != null)
            {
                string[] defaultFormNames = new string[]
                {
                    EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                    , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration)
                    , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                };

                var entityCurrentEntity = checkInMongo.Where(x => defaultFormNames.Contains(x.FormTitle)).FirstOrDefault();
                if (entityCurrentEntity != null)
                {
                    var updateEntityQuery = Query<FormDataEntryMongo>.EQ(p => p.Id, entityCurrentEntity.Id);
                    FormDataEntryMongo updateEntityModel = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindOne(updateEntityQuery);
                    if (updateEntityModel != null)
                    {

                        List<FormDataEntryVariableMongo> variableList = new List<FormDataEntryVariableMongo>();
                        FormDataEntryVariableMongo variableModel = new FormDataEntryVariableMongo();
                        var formVariables = updateEntityModel.formDataEntryVariableMongoList.AsQueryable();
                        var state = formVariables.FirstOrDefault(x => x.VariableName == DefaultsVariables.State.ToString());
                        var suburb = formVariables.FirstOrDefault(x => x.VariableName == DefaultsVariables.Suburb.ToString());

                        formVariables.ToList().ForEach(frmVrbl =>
                        {
                            variableModel = frmVrbl;

                            #region Person Registration Form
                            if (updateEntityModel.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration))
                            {
                                if (frmVrbl.VariableName == DefaultsVariables.FirstName.ToString())
                                {
                                    variableModel.SelectedValues = userlogin.FirstName;
                                }
                                else if (frmVrbl.VariableName == DefaultsVariables.Name.ToString())
                                {
                                    variableModel.SelectedValues = userlogin.LastName;
                                }
                                else if (frmVrbl.VariableName == DefaultsVariables.Phone.ToString())
                                {
                                    variableModel.SelectedValues = userlogin.Mobile;
                                }
                            }
                            #endregion

                            #region Participant Registration Form
                            if (updateEntityModel.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
                            {
                                if (frmVrbl.VariableName == DefaultsVariables.FirstName.ToString())
                                {
                                    variableModel.SelectedValues = userlogin.FirstName;
                                }
                                else if (frmVrbl.VariableName == DefaultsVariables.Name.ToString())
                                {
                                    variableModel.SelectedValues = userlogin.LastName;
                                }
                                else if (frmVrbl.VariableName == DefaultsVariables.Phone.ToString())
                                {
                                    variableModel.SelectedValues = userlogin.Mobile;
                                }
                            }
                            #endregion

                            #region Place/Group Registration Form
                            if (updateEntityModel.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
                            {
                                if (frmVrbl.VariableName == DefaultsVariables.Name.ToString())
                                {
                                    variableModel.SelectedValues = userlogin.FirstName;
                                }
                                else if (frmVrbl.VariableName == DefaultsVariables.Phone.ToString())
                                {
                                    variableModel.SelectedValues = userlogin.Mobile;
                                }
                            }
                            #endregion

                            variableList.Add(variableModel);
                        });
                        updateEntityModel.formDataEntryVariableMongoList = variableList;
                        updateEntityModel.ThisUserAddress = userlogin.Address;
                        updateEntityModel.ThisUserEmail = userlogin.Email;
                        updateEntityModel.ThisUserPhone = userlogin.Mobile;
                        updateEntityModel.ThisUserState = state != null ? state.SelectedValues : null;
                        updateEntityModel.ThisUserSuburb = suburb != null ? suburb.SelectedValues : null;

                        // Document Collections  
                        var collection = _mongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities");
                        // Document Update which need Id and Data to Update  
                        var result = collection.Update(updateEntityQuery, MongoDB.Driver.Builders.Update.Replace(updateEntityModel), MongoDB.Driver.UpdateFlags.None);
                        return true;
                    }
                }
            }
            else
            {
                var formDataEntry = dbContext.FormDataEntries.FirstOrDefault(c => c.ThisUserId == userlogin.Id);
                if (formDataEntry != null)
                {
                    var formVariables = formDataEntry.FormDataEntryVariables.ToList();
                    formVariables.ForEach(frmVrbl =>
                    {
                        #region Person Registration Form
                        if (formDataEntry.Form.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration))
                        {
                            if (frmVrbl.Variable.VariableName == DefaultsVariables.FirstName.ToString())
                            {
                                frmVrbl.SelectedValues = userlogin.FirstName;
                            }
                            else if (frmVrbl.Variable.VariableName == DefaultsVariables.Name.ToString())
                            {
                                frmVrbl.SelectedValues = userlogin.LastName;
                            }
                            else if (frmVrbl.Variable.VariableName == DefaultsVariables.Phone.ToString())
                            {
                                frmVrbl.SelectedValues = userlogin.Mobile;
                            }
                        }
                        #endregion

                        #region Participant Registration Form
                        if (formDataEntry.Form.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
                        {
                            if (frmVrbl.Variable.VariableName == DefaultsVariables.FirstName.ToString())
                            {
                                frmVrbl.SelectedValues = userlogin.FirstName;
                            }
                            else if (frmVrbl.Variable.VariableName == DefaultsVariables.Name.ToString())
                            {
                                frmVrbl.SelectedValues = userlogin.LastName;
                            }
                            else if (frmVrbl.Variable.VariableName == DefaultsVariables.Phone.ToString())
                            {
                                frmVrbl.SelectedValues = userlogin.Mobile;
                            }
                        }
                        #endregion

                        #region Place/Group Registration Form
                        if (formDataEntry.Form.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
                        {
                            if (frmVrbl.Variable.VariableName == DefaultsVariables.Name.ToString())
                            {
                                frmVrbl.SelectedValues = userlogin.FirstName;
                            }
                            else if (frmVrbl.Variable.VariableName == DefaultsVariables.Phone.ToString())
                            {
                                frmVrbl.SelectedValues = userlogin.Mobile;
                            }
                        }
                        #endregion

                    });
                    SaveChanges();
                    return true;
                }
            }
            return false;
        }


        public bool TestEnvironment_UpdateMyProfileDataInDataEntryMongo(UserLogin userlogin)
        {
            TestMongoDBContext _testMongoDBContext = new TestMongoDBContext();
            var checkInMongoEntityCondition = Query<FormDataEntryMongo>.EQ(q => q.ThisUserGuid, userlogin.Guid);
            IQueryable<FormDataEntryMongo> checkInMongo = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").Find(checkInMongoEntityCondition).AsQueryable();

            if (checkInMongo != null)
            {
                string[] defaultFormNames = new string[]
                {
                    EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Person_Registration)
                    , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Participant_Registration)
                    , EnumHelpers.GetEnumDescription((DefaultFormName)(int)DefaultFormName.Place__Group_Registration)
                };

                var entityCurrentEntity = checkInMongo.Where(x => defaultFormNames.Contains(x.FormTitle)).FirstOrDefault();
                if (entityCurrentEntity != null)
                {
                    var updateEntityQuery = Query<FormDataEntryMongo>.EQ(p => p.Id, entityCurrentEntity.Id);
                    FormDataEntryMongo updateEntityModel = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities").FindOne(updateEntityQuery);
                    if (updateEntityModel != null)
                    {

                        List<FormDataEntryVariableMongo> variableList = new List<FormDataEntryVariableMongo>();
                        FormDataEntryVariableMongo variableModel = new FormDataEntryVariableMongo();
                        var formVariables = updateEntityModel.formDataEntryVariableMongoList.AsQueryable();

                        var state = formVariables.FirstOrDefault(x => x.VariableName == DefaultsVariables.State.ToString());
                        var suburb = formVariables.FirstOrDefault(x => x.VariableName == DefaultsVariables.Suburb.ToString());

                        formVariables.ToList().ForEach(frmVrbl =>
                        {
                            variableModel = frmVrbl;

                            #region Person Registration Form
                            if (updateEntityModel.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration))
                            {
                                if (frmVrbl.VariableName == DefaultsVariables.FirstName.ToString())
                                {
                                    variableModel.SelectedValues = userlogin.FirstName;
                                }
                                else if (frmVrbl.VariableName == DefaultsVariables.Name.ToString())
                                {
                                    variableModel.SelectedValues = userlogin.LastName;
                                }
                                else if (frmVrbl.VariableName == DefaultsVariables.Phone.ToString())
                                {
                                    variableModel.SelectedValues = userlogin.Mobile;
                                }
                            }
                            #endregion

                            #region Participant Registration Form
                            if (updateEntityModel.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
                            {
                                if (frmVrbl.VariableName == DefaultsVariables.FirstName.ToString())
                                {
                                    variableModel.SelectedValues = userlogin.FirstName;
                                }
                                else if (frmVrbl.VariableName == DefaultsVariables.Name.ToString())
                                {
                                    variableModel.SelectedValues = userlogin.LastName;
                                }
                                else if (frmVrbl.VariableName == DefaultsVariables.Phone.ToString())
                                {
                                    variableModel.SelectedValues = userlogin.Mobile;
                                }
                            }
                            #endregion

                            #region Place/Group Registration Form
                            if (updateEntityModel.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
                            {
                                if (frmVrbl.VariableName == DefaultsVariables.Name.ToString())
                                {
                                    variableModel.SelectedValues = userlogin.FirstName;
                                }
                                else if (frmVrbl.VariableName == DefaultsVariables.Phone.ToString())
                                {
                                    variableModel.SelectedValues = userlogin.Mobile;
                                }
                            }
                            #endregion
                            variableList.Add(variableModel);
                        });
                        updateEntityModel.formDataEntryVariableMongoList = variableList;
                        updateEntityModel.ThisUserAddress = userlogin.Address;
                        updateEntityModel.ThisUserEmail = userlogin.Email;
                        updateEntityModel.ThisUserPhone = userlogin.Mobile;
                        updateEntityModel.ThisUserState = state != null ? state.SelectedValues : null;
                        updateEntityModel.ThisUserSuburb = suburb != null ? suburb.SelectedValues : null;
                        // Document Collections  
                        var collection = _testMongoDBContext._database.GetCollection<FormDataEntryMongo>("UserEntities");
                        // Document Update which need Id and Data to Update  
                        var result = collection.Update(updateEntityQuery, MongoDB.Driver.Builders.Update.Replace(updateEntityModel), MongoDB.Driver.UpdateFlags.None);
                        return true;
                    }
                }
            }
            else
            {
                var formDataEntry = dbContext.FormDataEntries.FirstOrDefault(c => c.ThisUserId == userlogin.Id);
                if (formDataEntry != null)
                {
                    var formVariables = formDataEntry.FormDataEntryVariables.ToList();
                    formVariables.ForEach(frmVrbl =>
                    {
                        #region Person Registration Form
                        if (formDataEntry.Form.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Person_Registration))
                        {
                            if (frmVrbl.Variable.VariableName == DefaultsVariables.FirstName.ToString())
                            {
                                frmVrbl.SelectedValues = userlogin.FirstName;
                            }
                            else if (frmVrbl.Variable.VariableName == DefaultsVariables.Name.ToString())
                            {
                                frmVrbl.SelectedValues = userlogin.LastName;
                            }
                            else if (frmVrbl.Variable.VariableName == DefaultsVariables.Phone.ToString())
                            {
                                frmVrbl.SelectedValues = userlogin.Mobile;
                            }
                        }
                        #endregion

                        #region Participant Registration Form
                        if (formDataEntry.Form.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Participant_Registration))
                        {
                            if (frmVrbl.Variable.VariableName == DefaultsVariables.FirstName.ToString())
                            {
                                frmVrbl.SelectedValues = userlogin.FirstName;
                            }
                            else if (frmVrbl.Variable.VariableName == DefaultsVariables.Name.ToString())
                            {
                                frmVrbl.SelectedValues = userlogin.LastName;
                            }
                            else if (frmVrbl.Variable.VariableName == DefaultsVariables.Phone.ToString())
                            {
                                frmVrbl.SelectedValues = userlogin.Mobile;
                            }
                        }
                        #endregion

                        #region Place/Group Registration Form
                        if (formDataEntry.Form.FormTitle == EnumHelpers.GetEnumDescription(DefaultFormName.Place__Group_Registration))
                        {
                            if (frmVrbl.Variable.VariableName == DefaultsVariables.Name.ToString())
                            {
                                frmVrbl.SelectedValues = userlogin.FirstName;
                            }
                            else if (frmVrbl.Variable.VariableName == DefaultsVariables.Phone.ToString())
                            {
                                frmVrbl.SelectedValues = userlogin.Mobile;
                            }
                        }
                        #endregion
                    });
                    SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public int GetUserStatus(string username
            , int authTypeId = (int)AuthenticationTypes.Local_Password
            , bool isTestSite = false)
        {
            UserLogin userInfo = new UserLogin();
            if (isTestSite)
                userInfo = dbContext.UserLogins
                    .FirstOrDefault(x => x.UserName.ToLower() == username.ToLower() && x.AuthTypeId == authTypeId && x.UserTypeId == (int)Core.Enum.UsersLoginType.Test && x.IsUserApprovedBySystemAdmin);
            else
                userInfo = dbContext.UserLogins
                    .FirstOrDefault(x => x.UserName.ToLower() == username.ToLower() && x.AuthTypeId == authTypeId && x.UserTypeId != (int)Core.Enum.UsersLoginType.Test && x.IsUserApprovedBySystemAdmin);
            if (userInfo != null)
            {
                return userInfo.Status ?? 0;
            }
            return 0;
        }
        public bool IsPassExist(string pass)
        {
            string plain, hash;
            byte[] temp;

            plain = pass;

            SHA1 sha = new SHA1CryptoServiceProvider();
            // This is one implementation of the abstract class SHA1.
            temp = sha.ComputeHash(Encoding.UTF8.GetBytes(plain));

            //storing hashed vale into byte data type
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < temp.Length; i++)
            {
                sb.Append(temp[i].ToString("x2"));
            }

            hash = sb.ToString();
            return dbContext.HashPasswords.Any(x => x.HashPassword1.ToLower().Contains(hash.ToLower()));
        }
    }


}
