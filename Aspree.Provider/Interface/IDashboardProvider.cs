using Aspree.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IDashboardProvider
    {
        DashboardStatus GetDashboardStatus(DashboardFilter filter);
    }
}
