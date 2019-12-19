using Aspree.Core.ViewModels;
using Aspree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IFormProvider : IProviderCommon<FormViewModel, Form>
    {
        IEnumerable<FormViewModel> GetAll(Guid tenantId);
        ProjectBuilderFormsViewModel GetProjectBuilderForms(Guid tenantId, Guid LoggedInUserId, Guid projectId);
        IEnumerable<FormViewModel> GetAllDefaultForms(Guid tenantId);
        IEnumerable<FormViewModel> GetProjectDefaultForms(Guid tenantId, Guid projectId);
        IEnumerable<FormViewModel> GetFormsByGuidList(List<Guid> guids);
        FormViewModel GetActivityFormBySearchedEntity(int entId, Guid formId, Guid activityId, int summarypageActivityId);
        IEnumerable<FormViewModel> GetAllForms(Guid tenantId, Guid projectId);
        FormViewModel TestEnvironment_GetActivityFormBySearchedEntity(int entId, Guid formId, Guid activityId);
        FormViewModel GetUSerFormByGuid(Guid guid, Guid logginuserId, Guid projectid);
    }
}
