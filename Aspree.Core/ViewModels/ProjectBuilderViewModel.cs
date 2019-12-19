using Swashbuckle.Examples;
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

            //VariableCategory = new List<VariableCategoryViewModel>();
            //Role = new List<RoleModel>();
            //VariableType = new List<VariableTypeViewModel>();
            //ValidationRule = new List<ValidationRuleViewModel>();
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

    public class ProjectBuilderActivityViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ProjectBuilderActivityViewModel()
            {
                ActivityCategory = new List<ActivityCategoryViewModel>()
                {
                    new ActivityCategoryViewModel()
                    {
                        Guid = Guid.NewGuid(),
                        CategoryName = "asdasd",
                        Activities = new List<SubCategoryViewModel>()
                        {
                            new SubCategoryViewModel()
                            {
                                Guid = Guid.NewGuid(),
                                CreatedBy =1,
                                DeploymentStatus=1,
                                EndDate=DateTime.UtcNow,
                                Id=1,
                                IsAllVariableApprovedOfActivity=true,
                                Name ="Example",
                                StartDate=DateTime.UtcNow,                                
                                EntityType =new List<Guid>(),
                                IsApproved = true,
                                Type="Example-Type",
                                DateDeactivated=DateTime.UtcNow,
                                IsDefaultVariable=1,
                                IsPublished=true,
                                ProjectId=Guid.NewGuid(),
                                RepeatationCount=1,
                                RepeatationOffset=1,
                                ScheduleType=1,
                                Status="Active"





                            }
                        },
                        CreatedBy =Guid.NewGuid(),


                    }

                },
                EntityTypes = new List<EntityTypeViewModel>()
                 {
                     new EntityTypeViewModel()
                     {
                         Guid = Guid.NewGuid(),
                         Id=1,
                         Name="Text-Example",
                         TenantId=Guid.NewGuid()
                     }
                 },
                FormCategory = new List<FormCategoryViewModel>()
                  {
                      new FormCategoryViewModel()
                      {
                          Guid = Guid.NewGuid(),
                          TenantId=Guid.NewGuid(),
                          Id=1,
                          CategoryName="Example",
                           CreatedBy=Guid.NewGuid(),
                           CreatedDate=DateTime.UtcNow,
                           DateDeactivated=DateTime.UtcNow,
                           DeactivatedBy=Guid.NewGuid(),
                           Forms=new List<SubCategoryViewModel>()
                           {
                               new SubCategoryViewModel()
                               {
                                   DateDeactivated=DateTime.UtcNow,
                                   Guid=Guid.NewGuid(),
                                   CreatedBy=1,
                                   DeploymentStatus=1,
                                   EndDate=DateTime.UtcNow,
                                   EntityType=new List<Guid>(),
                                   Id=1,
                                   IsAllVariableApprovedOfActivity=true,
                                   IsApproved=true,
                                   IsDefaultVariable=1,
                                   IsPublished=true,
                                   Name="Example",
                                   ProjectId=Guid.NewGuid(),
                                   RepeatationCount=1,
                                   RepeatationOffset=1,
                                   ScheduleType=1,
                                   StartDate=DateTime.UtcNow,
                                  Status="Active",
                                  Type="Test type"


                               }
                           }

                      }
                  },
                Roles = new List<RoleModel>()
                  {
                      new RoleModel()
                      {
                          Guid = Guid.NewGuid(),
                          CreatedBy=Guid.NewGuid(),
                          CreatedDate=DateTime.UtcNow,
                          DateDeactivated=DateTime.UtcNow,
                          DeactivatedBy=Guid.NewGuid(),
                          Id=1,
                          IsSystemRole=true,
                          ModifiedBy=Guid.NewGuid(),
                          ModifiedDate=DateTime.UtcNow,
                          Name="Test",
                          Privileges=new  List<Guid>(),                          
                          TenantId=new  Nullable<Guid> ()



                      },
                  }
            };
        }
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

    public class ProjectBuilderFormsViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ProjectBuilderFormsViewModel
            {
                
            };
        }
    }

    public class ProjectBuilderVariablesViewModelExamples : IExamplesProvider
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
        public Guid EntityGroupId { get; set; }
        public string EntityGroupName { get; set; }
    }

}