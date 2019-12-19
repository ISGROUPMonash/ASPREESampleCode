using Aspree.Core.ViewModels;
using Aspree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface ISchedulingProvider : IProviderCommon<SchedulingViewModel, ActivityScheduling>
    {
        SchedulingViewModel GetByActivityGuid(Guid guid);
        IEnumerable<SchedulingViewModel> GetAllScheduledActivityByProjectId(Guid projectId);
        bool PushScheduledActivities(List<Guid> activitiesList, int statusType, Guid loggedInUser);
        bool InactivateActivity(List<Guid> activityIdList, int statusType, Guid LoggedInUserId, Guid projectId);
    }
}
