using Aspree.Core.ViewModels;
using Aspree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IReportProvider
    {
        ReportViewModel GetAll(Guid loggedInUser, string ProjectName, string ActivityName = null, string FormName = null);
    }
}
