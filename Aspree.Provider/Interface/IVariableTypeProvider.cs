using Aspree.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    /// <summary>
    /// Handles form variable type & related operations
    /// </summary>
    public interface IVariableTypeProvider
    {
        IEnumerable<VariableTypeViewModel> GetAll();
    }
}
