using Aspree.Core.ViewModels;
using Aspree.Core.ViewModels.MongoViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface.MongoProvider
{
    public interface IProjectDeployProvider
    {
        ProjectDeployViewModel Create(Guid projectId, List<Guid> activitiesList, int statusType);
        ProjectDeployViewModel CreateTestProject(Guid projectId, List<Guid> activitiesList, int statusType);

        ProjectDeployViewModel GetProjectByGuid(Guid projectId);
        ProjectDeployViewModel TestEnvironment_GetProjectByGuid(Guid projectId);

        IEnumerable<FormDataEntryProjectsViewModel> GetAllDeployedProject(Guid loggedInUserId);
        IEnumerable<FormDataEntryProjectsViewModel> TestEnvironment_GetAllDeployedProject(Guid loggedInUserId);

        ProjectStaffMemberRoleViewModel SQL_CheckEntityLinkedProject(Guid projectId, int entityId);
        ProjectStaffMemberRoleViewModel CheckEntityLinkedProject(Guid projectId, int entityId);
        ProjectStaffMemberRoleViewModel TestEnvironment_CheckEntityLinkedProject(Guid projectId, int entityId);
    }
}
