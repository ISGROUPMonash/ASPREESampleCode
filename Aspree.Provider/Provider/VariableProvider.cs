using Aspree.Provider.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspree.Core.ViewModels;
using Aspree.Core.Enum;

namespace Aspree.Provider.Provider
{
    public class VariableProvider : IVariableProvider
    {
        private readonly IVariableCategoryProvider _VariableCategoryProvider;
        private readonly IValidationRuleProvider _ValidationRuleProvider;
        private readonly IRoleProvider _RoleProvider;
        private readonly IVariableTypeProvider _VariableTypeProvider;

        public VariableProvider(  IVariableCategoryProvider variableCategoryProvider, IValidationRuleProvider validationRuleProvider, IRoleProvider roleProvider, IVariableTypeProvider variableTypeProvider)
        {
            this._VariableCategoryProvider = variableCategoryProvider;
            this._ValidationRuleProvider = validationRuleProvider;
            this._RoleProvider = roleProvider;
            this._VariableTypeProvider = variableTypeProvider;
        }

        /// <summary>
        /// Method to get project builder variables on the basis of tenantId.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns>List of projectBuilderVariablesViewModel model.</returns>
        public ProjectBuilderVariablesViewModel GetProjectBuilderVariables(Guid tenantId)
        {
           
            ProjectBuilderVariablesViewModel projectBuilderVariablesViewModel = new ProjectBuilderVariablesViewModel();

            projectBuilderVariablesViewModel.VariableCategory = _VariableCategoryProvider.GetAll(tenantId);
            projectBuilderVariablesViewModel.Role = _RoleProvider.GetAll(tenantId);
            projectBuilderVariablesViewModel.VariableType = _VariableTypeProvider.GetAll();
            projectBuilderVariablesViewModel.ValidationRule = _ValidationRuleProvider.GetAll();
           
            return projectBuilderVariablesViewModel;
        }
       
    }
}