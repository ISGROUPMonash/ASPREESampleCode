using Aspree.Core.ViewModels;
using Aspree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IAuthenticationTypeProvider : IProviderCommon<AuthenticationTypeViewModel, LoginAuthTypeMaster>
    {
        IEnumerable<AuthenticationTypeViewModel> GetAll(Guid tenantId);
        AuthenticationTypeViewModel GetAuthenticationTypeByState(string state);
        AuthenticationTypeViewModel InActiveByGuid(Guid guid, Guid DeletedBy);
    }
}
