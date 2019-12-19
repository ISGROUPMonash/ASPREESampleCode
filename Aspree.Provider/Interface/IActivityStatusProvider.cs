using Aspree.Core.ViewModels;
using Aspree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IActivityStatusProvider 
    {
        ActivityStatusViewModel Create(ActivityStatusViewModel model);
        ActivityStatusViewModel Update(ActivityStatusViewModel model);
        ActivityStatusViewModel ToModel(ActivityStatu entity);
        IEnumerable<ActivityStatusViewModel> GetAll();
        ActivityStatusViewModel GetById(int id);
        ActivityStatusViewModel GetByGuid(Guid guid);
        ActivityStatusViewModel DeleteById(int id, Guid DeletedBy);
        ActivityStatusViewModel DeleteByGuid(Guid guid, Guid DeletedBy);
        void SaveChanges();
    }
}
