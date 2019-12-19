using Aspree.Core.ViewModels;
using Aspree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IActivityCategoryProvider
    {
        ActivityCategoryViewModel Create(ActivityCategoryViewModel model);
        ActivityCategoryViewModel Update(ActivityCategoryViewModel model);
        ActivityCategoryViewModel ToModel(ActivityCategory entity);
        IEnumerable<ActivityCategoryViewModel> GetAll();
        ActivityCategoryViewModel GetById(int id);
        ActivityCategoryViewModel GetByGuid(Guid guid);
        ActivityCategoryViewModel DeleteById(int id, Guid DeletedBy);
        ActivityCategoryViewModel DeleteByGuid(Guid guid,Guid DeletedBy);
        IEnumerable<ActivityCategoryViewModel> GetAll(Guid tenantId);
        void SaveChanges();
    }
}
