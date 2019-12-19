using Aspree.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IActionListProvider
    {
        IQueryable<ActionListViewModel> GetAll(ActionListSearchParameters searchModel);
        IQueryable<ActionListViewModel> GetAllActionListActivities(Guid projectId, Guid loggedInUserGuid);
        int CountAllActionListRecord(Guid projectId, Guid loggedInUserGuid);
    }
}
