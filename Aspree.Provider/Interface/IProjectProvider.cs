using Aspree.Core.ViewModels;
using Aspree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IProjectProvider : IProviderCommon<ProjectViewModel, Project>
    {
        IEnumerable<ProjectViewModel> GetAll(Guid tenantId);
        ProjectViewModel PublishProject(ProjectViewModel model);
        FormDataEntryProjectsViewModel GetProjectByGuid_New(Guid guid);
        IEnumerable<ProjectViewModel> GetAllProjectByUserId(Guid userId);
        ProjectBasicDetailsViewModel ProjectBasicDetail(Guid guid);
    }
}
