using Aspree.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    /// <summary>
    /// To get the all the details about Role
    /// </summary>
    public interface IRoleProvider
    {
        IEnumerable<RoleModel> GetAll(Guid tenantId);
    }
}
