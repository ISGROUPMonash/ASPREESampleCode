using Aspree.Core.ViewModels;
using Aspree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IUserLoginProvider : IProviderCommon<UserLoginViewModel, UserLogin>
    {
        List<string> GetUserRolesByEmail(string email);
        RoleViewModel IsValidUser(string UserName, string Password);
        bool ChangePassword(Guid guid, ChangePassword changePassword);
        bool ResetPassword(ResetPassword resetPassword, bool isTestSite=false);
        bool ResetNewPassword(ResetPassword resetPassword, bool isTestSite = false);
        bool EditSecurityQuestion(UpdateUserSecurityQuestion securityQuestion);
        bool ChangeStatus(Guid userGuid, int newStatus);
        IEnumerable<UserLoginViewModel> GetAll(Guid tenantId);
        RoleViewModel CheckLoginCredentials(string username, string password);
        UserLoginViewModel GetUserByEmailId(string EmailId);
        UserLoginViewModel GetByTempGuid(Guid guid,bool isTestSite=false);
        bool updateTempGuid(Guid userid);
        IEnumerable<UserLoginViewModel> GetProjectAllUsers(Guid projectId);
        IEnumerable<UserLoginViewModel> TestEnvironment_GetProjectAllUsers(Guid projectId);
        bool checkUsernameExist(string username, Guid authType, Guid? userid);
        bool IsPassExist(string password);
        SecurityQuestionViewModel GetUserByUsername(string username);
        bool check_IsMailSend(bool isTestSite, string username, int? userid);
        bool UpdateIsMailSend(int userid);
        Guid UpdateTempGuid(int id);
        string GetUserRoleByProjectId(Guid userId, Guid projectId);

        IEnumerable<SystemAdminToolsUserViewModel> GetAllSystemAdminToolsUser(Guid tenantId, Guid projectId);
        UserLoginViewModel UpdateMyProfile(UserLoginViewModel model, bool isTest = false);

        int GetUserStatus(string username
            , int authTypeId = (int)Core.Enum.AuthenticationTypes.Local_Password
            , bool isTestSite = false);
    }
}
