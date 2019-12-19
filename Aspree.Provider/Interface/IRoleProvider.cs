using Aspree.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IRoleProvider
    {

        RoleModel Create(RoleModel model);
        RoleModel Update(RoleModel model);
        RoleModel ToModel(Aspree.Data.Role entity);
        IEnumerable<RoleModel> GetAll();
        RoleModel GetById(int id);
        RoleModel GetByGuid(Guid guid);
        RoleModel DeleteById(int id, Guid DeletedBy);
        RoleModel DeleteByGuid(Guid guid, Guid DeletedBy);
        bool ChangeStatus(Guid roleGuid, int newStatus);
        IEnumerable<RoleModel> GetAll(Guid tenantId);
        void SaveChanges();
    }
}
