using Aspree.Core.ViewModels;
using Aspree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IVariableCategoryProvider : IProviderCommon<VariableCategoryViewModel, VariableCategory>
    {
        IEnumerable<VariableCategoryViewModel> GetAll(Guid tenantId);
    }
}
