using Aspree.Core.ViewModels;
using Aspree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IEntityProvider : IProviderCommon<EntityViewModel, Entity>
    {
        IEnumerable<EntityViewModel> GetAll(Guid tenantId);
        EntityViewModel GetByTenantGuid(Guid guid);
        EntityViewModel GetByEntityTypeAndSubTypeGuid(Guid tenantGuid, Guid typeGuid, Guid? subTypeGuid = null);
        IEnumerable<EntityViewModel> GetAllEntitiesCreatedBySearch();
    }
}
