using Aspree.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    /// <summary>
    /// Handles form variables & related operations
    /// </summary>
    public interface IVariableProvider 
    {
        ProjectBuilderVariablesViewModel GetProjectBuilderVariables(Guid tenantId);
    }
}
