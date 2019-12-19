using Aspree.Core.ViewModels;
using Aspree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IActivityProvider : IProviderCommon<ActivityViewModel, Activity>
    {
        IEnumerable<ActivityViewModel> GetAll(Guid tenantId);
        ProjectBuilderActivityViewModel GetProjectBuilderActivities(Guid tenantId, Guid LoggedInUserId, Guid projectId);
        ActivityViewModel UpdateActivityScheduling(ScheduleActivityViewModel model);
        ActivityViewModel RemoveScheduledActivity(Guid guid, Guid DeletedBy);
        ActivityViewModel SavePreviewScheduledActivity(string guid, Guid SavedBy);
        ActivityViewModel ScheduleActivity_New(NewScheduleActivityViewModel model);
        IEnumerable<SchedulingViewModel> GetAllActivityScheduling(Guid activityId);
        ActivityViewModel GetActivityByGuid(Guid guid, Guid logginuserId, Guid projectid);
        AddSummaryPageActivityViewModel AddSummaryPageActivity(AddSummaryPageActivityViewModel model);
        AddSummaryPageActivityViewModel TestEnvironment_AddSummaryPageActivity(AddSummaryPageActivityViewModel model);
        IEnumerable<AddSummaryPageActivityViewModel> GetAllSummaryPageActivity(string entId, Guid projectId, Guid loggedInUserId);
        IEnumerable<AddSummaryPageActivityViewModel> TestEnvironment_GetAllSummaryPageActivity(string entId, Guid projectId);
        AddSummaryPageActivityViewModel EditSummaryPageActivity(AddSummaryPageActivityViewModel model);
        AddSummaryPageActivityViewModel DeleteSummaryPageActivity(int id,Guid loggedinuser);
        AddSummaryPageActivityViewModel TestEnvironment_DeleteSummaryPageActivity(int id,Guid loggedinuser);
    }
}
