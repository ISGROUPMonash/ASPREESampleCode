using Aspree.Core.ViewModels;
using Aspree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IVariableProvider : IProviderCommon<VariableViewModel, Variable>
    {
        IEnumerable<VariableViewModel> GetAll(Guid tenantId);
        ProjectBuilderVariablesViewModel GetProjectBuilderVariables(Guid tenantId, Guid LoggedInUserId, Guid projectId);
        IEnumerable<ProjectBuilderFormViewModelViewModel> GetFormVariableByProjectId(Guid projectId);
        IEnumerable<VariableViewModel> GetAllVariables(Guid tenantId, Guid projectId);
        VariableViewModel GetVariablesByGuid(Guid guid, Guid logginuserId, Guid projectid);
    }
}
