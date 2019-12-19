﻿using Aspree.Core.ViewModels;
using Aspree.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Provider.Interface
{
    public interface IFormCategoryProvider: IProviderCommon<FormCategoryViewModel, FormCategory>
    {
        IEnumerable<FormCategoryViewModel> GetAll(Guid tenantId);
    }
}
