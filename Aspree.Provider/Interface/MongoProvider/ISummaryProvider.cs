using Aspree.Core.ViewModels.MongoViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface.MongoProvider
{
    public interface ISummaryProvider
    {
        SummaryViewModel GetSummaryDetails(Guid projectId, Int64 entityId, Guid LoggedInUser);
        SummaryViewModel TestEnvironment_GetSummaryDetails(Guid projectId, Int64 entityId, Guid LoggedInUser);

        SummaryPageActivityViewModel AddSummaryPageActivity(SummaryPageActivityViewModel model, Guid loggedInUser);
        SummaryPageActivityViewModel TestEnvironment_AddSummaryPageActivity(SummaryPageActivityViewModel model, Guid loggedInUser);

        SummaryPageActivityViewModel EditSummaryPageActivity(SummaryPageActivityViewModel model, Guid loggedInUser);
        SummaryPageActivityViewModel TestEnvironment_EditSummaryPageActivity(SummaryPageActivityViewModel model, Guid loggedInUser);

        SummaryPageActivityViewModel DeleteSummaryPageActivity(string id, Guid loggedInUser);
        SummaryPageActivityViewModel TestEnvironment_DeleteSummaryPageActivity(string id, Guid loggedInUser);

        FormsMongo GetSummaryPageForm(Int64 entId, Guid formId, Guid activityId, Guid projectId, int p_Version, string summaryPageActivityId, Guid loggedInUserId, Guid currentProjectId);
        FormsMongo TestEnvironment_GetSummaryPageForm(Int64 entId, Guid formId, Guid activityId, Guid projectId, int p_Version, string summaryPageActivityId, Guid loggedInUserId, Guid currentProjectId);
        Core.ViewModels.SummaryPageLeftPanelViewModel UpdateLeftPanelSummaryPage(Guid projectId, Int64 entityId,bool isTestSite=false);

    }
}
