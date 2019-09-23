using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class ProjectBuilderViewModel
    {
    }
    public class ProjectBuilderVariablesViewModel
    {
        public ProjectBuilderVariablesViewModel()
        {
            
        }
        /// <summary>
        /// List of variable category
        /// </summary>
        public IEnumerable<VariableCategoryViewModel> VariableCategory { get; set; }
        /// <summary>
        /// List of roles
        /// </summary>
        public IEnumerable<RoleModel> Role { get; set; }
        /// <summary>
        /// List of variable type
        /// </summary>
        public IEnumerable<VariableTypeViewModel> VariableType { get; set; }
        /// <summary>
        /// List of ValidationRule
        /// </summary>
        public IEnumerable<ValidationRuleViewModel> ValidationRule { get; set; }

        public IEnumerable<LookupVariablesPreviewViewModel> LookupVariablesPreviewViewModelList { get; set; }
    }

    public class ProjectBuilderFormsViewModel
    {
        /// <summary>
        /// List of Form Category
        /// </summary>
        public IEnumerable<FormCategoryViewModel> FormCategory { get; set; }
        /// <summary>
        /// List of Variable Categories
        /// </summary>
        public IEnumerable<VariableCategoryViewModel> VariableCategories { get; set; }
        /// <summary>
        /// List of roles
        /// </summary>
        public IEnumerable<RoleModel> Roles { get; set; }
        /// <summary>
        /// List of Entity type
        /// </summary>
        public IEnumerable<EntityTypeViewModel> EntityTypes { get; set; }
    }

    public class ProjectBuilderActivityViewModel
    {
        /// <summary>
        /// List of Form Category
        /// </summary>
        public IEnumerable<FormCategoryViewModel> FormCategory { get; set; }
        /// <summary>
        /// List of Form Activity Category
        /// </summary>
        public IEnumerable<ActivityCategoryViewModel> ActivityCategory { get; set; }
        /// <summary>
        /// List of Form Activity Roles
        /// </summary>
        public IEnumerable<RoleModel> Roles { get; set; }
        /// <summary>
        ///  List of Entity Types
        /// </summary>
        public IEnumerable<EntityTypeViewModel> EntityTypes { get; set; }
    }

   
    public class ProjectBuilderFormViewModelViewModel
    {
        public int FormId { get; set; }
        public int? ProjectId { get; set; }
        public List<FormVariableViewModel> FormVariables { get; set; }
        public bool VariableStatus { get; set; }
        public string VariableName { get; set; }
        public string FormName { get; set; }
        public int FormStatusId { get; set; }
    }

    public class ProjectBuilderFormsViewModelExamples //: IExamplesProvider
    {
        public object GetExamples()
        {
            return new ProjectBuilderFormsViewModel
            {
                
            };
        }
    }

    public class ProjectBuilderVariablesViewModelExamples //: IExamplesProvider
    {
        public object GetExamples()
        {
            return new ProjectBuilderVariablesViewModel
            {

            };
        }
    }






    public class LookupVariablesPreviewViewModel
    {
        public int Id { get; set; }
        public Guid EntityTypeId { get; set; }
        public Guid EntitySubtypeId { get; set; }
        public string EntityName { get; set; }
    }

}