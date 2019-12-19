using Aspree.Core.ViewModels;
using Aspree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IPrivilegeProvider : IProviderCommon<PrivilegeSmallViewModel, Privilege>
    {
        void SetDefaults();
        bool CreateDefaultFormsForProject(int projectId, int createdBy, int tenantId);
        bool CreateDefaultActivitiesForProject(int projectId, int createdBy, int tenantId);
    }
}
