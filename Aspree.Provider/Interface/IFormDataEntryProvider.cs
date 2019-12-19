using Aspree.Core.ViewModels;
using Aspree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IFormDataEntryProvider : IProviderCommon<FormDataEntryViewModel, FormDataEntry>
    {
        List<List<FormDataEntryVariableViewModel>> SearchVariables(SearchPageVariableViewModel model, string source = null);
        IEnumerable<FormDataEntryVariableViewModel> GetFormDataEntryByEntId(Guid projectId, Guid formId, string entId);
        UserLoginViewModel CheckDuplicateUsername(string userName, string emial, Guid authType, Guid? userguid);
        Guid? LocalPasswordGuid();
        Guid? AuthTypeLocalMailsend(Guid dataentryguid);
        IEnumerable<FormDataEntryProjectsViewModel> GetAllFormDataEntryProjects(Guid projectId, Guid formId);
        bool checkEmailExistUserLogin(string email, Guid? userid);
        bool checkUsernameExistUserLogin(string username, Guid authType, Guid? userid);
        IEnumerable<FormDataEntryProjectsViewModel> GetAllDataEntryProjectList();
        IEnumerable<FormDataEntryProjectsViewModel> TestEnvironment_GetAllDataEntryProjectList(Guid loggedInUser);
        FormDataEntryViewModel TestEnvironment_Create(FormDataEntryViewModel model);
        FormDataEntryViewModel TestEnvironment_Update(FormDataEntryViewModel model);
        List<List<FormDataEntryVariableViewModel>> TestEnvironment_SearchVariables(SearchPageVariableViewModel model);
        IEnumerable<FormDataEntryVariableViewModel> TestEnvironment_GetFormDataEntryByEntId(Guid projectId, Guid formId, string entId);
    }
}
