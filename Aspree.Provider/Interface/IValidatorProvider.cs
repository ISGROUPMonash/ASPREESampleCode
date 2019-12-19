using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IValidatorProvider
    {
        bool CheckExist(Core.ViewModels.ValidatorViewModal validator);
    }
}
