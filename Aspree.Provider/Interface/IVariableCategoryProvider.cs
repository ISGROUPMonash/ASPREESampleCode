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
    /// <summary>
   /// Handles category of variables & related operations
   /// </summary>
    public interface IVariableCategoryProvider
    {
        IEnumerable<VariableCategoryViewModel> GetAll(Guid tenantId);
    }
}
