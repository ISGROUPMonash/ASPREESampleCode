using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class AddSummaryPageActivityViewModel
    {
        /// <summary>
        /// Id of Page Activity
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Activity Id of PageActivity
        /// </summary>
        public Guid ActivityId { get; set; }
        /// <summary>
        /// Activity completed by user
        /// </summary>
        public Guid ActivityCompletedByUser { get; set; }
        /// <summary>
        /// Activity completed by 
        /// </summary>
        public string ActivityCompletedByUserName { get; set; }
        /// <summary>
        /// Date of activity
        /// </summary>
        public System.DateTime ActivityDate { get; set; }
        /// <summary>
        /// Is activity added
        /// </summary>
        public Nullable<bool> IsActivityAdded { get; set; }
        /// <summary>
        /// Project Id
        /// </summary>
        public Guid ProjectId { get; set; }
        /// <summary>
        /// Created By
        /// </summary>
        public Guid CreatedBy { get; set; }
        /// <summary>
        /// Created Date
        /// </summary>
        public System.DateTime CreatedDate { get; set; }
        /// <summary>
        /// Modified By
        /// </summary>
        public Nullable<Guid> ModifiedBy { get; set; }
        /// <summary>
        ///  Modified Date
        /// </summary>
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        /// <summary>
        /// Deactivated By
        /// </summary>
        public Nullable<Guid> DeactivatedBy { get; set; }
        /// <summary>
        /// Date of deactivation
        /// </summary>
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        /// <summary>
        /// Guid
        /// </summary>
        public System.Guid Guid { get; set; }
        public ActivityViewModel activityViewModel { get; set; }
        /// <summary>
        /// List of forms
        /// </summary>
        public List<FormActivityViewModel> Forms { get; set; }
        /// <summary>
        /// Name of activity
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// Person Entity Id
        /// </summary>
        public string PersonEntityId { get; set; }
        /// <summary>
        /// Guid of Linked Project from Project Linkage 
        /// </summary>
        public Guid LinkedProjectGuid { get; set; }
        /// <summary>
        /// Name of Linked Project from Project Linkage 
        /// </summary>
        public string LinkedProjectName { get; set; }
    }

    public class NewAddSummaryPageActivityViewModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ActivityId of Page Activity
        /// </summary>
        public Guid ActivityId { get; set; }
        /// <summary>
        /// Activity completed by
        /// </summary>
        public Guid ActivityCompletedByUser { get; set; }
        /// <summary>
        /// Activity date
        /// </summary>
        public System.DateTime ActivityDate { get; set; }
        /// <summary>
        /// Is activity added
        /// </summary>
        public Nullable<bool> IsActivityAdded { get; set; }
        /// <summary>
        /// Project Id
        /// </summary>
        public Guid ProjectId { get; set; }
        /// <summary>
        /// Created By
        /// </summary>
        public Guid CreatedBy { get; set; }
        /// <summary>
        /// Created date
        /// </summary>
        public System.DateTime CreatedDate { get; set; }
        /// <summary>
        /// Modified By
        /// </summary>
        public Nullable<Guid> ModifiedBy { get; set; }
        /// <summary>
        /// Modified date
        /// </summary>
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        /// <summary>
        /// Deactivited by
        /// </summary>
        public Nullable<Guid> DeactivatedBy { get; set; }
        /// <summary>
        /// Deactivation date
        /// </summary>
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        /// <summary>
        /// Guid
        /// </summary>
        public System.Guid Guid { get; set; }
        /// <summary>
        /// Person Entity Id
        /// </summary>
        public string PersonEntityId { get; set; }
    }


    public class NewAddSummaryPageActivityViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new NewAddSummaryPageActivityViewModel()
            {
               ActivityCompletedByUser=Guid.NewGuid(),
               CreatedDate=DateTime.UtcNow,
               DateDeactivated=DateTime.UtcNow,
               CreatedBy=Guid.NewGuid(),
               ActivityDate=DateTime.UtcNow,
               ActivityId=Guid.NewGuid(),
               DeactivatedBy=Guid.NewGuid(),
               Guid=Guid.NewGuid(),
               Id=1,
               IsActivityAdded=true,
               ModifiedBy=Guid.NewGuid(),
               ModifiedDate=DateTime.UtcNow,
               PersonEntityId="Test",
               ProjectId=Guid.NewGuid(),


                
               

            };
        }
    }

    public class NewAddSummaryPageActivityViewModelExamples1 : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<NewAddSummaryPageActivityViewModel>()
            {
                new NewAddSummaryPageActivityViewModel()
                {
                      ActivityCompletedByUser = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                DateDeactivated = DateTime.UtcNow,
                CreatedBy = Guid.NewGuid(),
                ActivityDate = DateTime.UtcNow,
                ActivityId = Guid.NewGuid(),
                DeactivatedBy = Guid.NewGuid(),
                Guid = Guid.NewGuid(),
                Id = 1,
                IsActivityAdded = true,
                ModifiedBy = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow,
                PersonEntityId = "Test",
                ProjectId = Guid.NewGuid(),

                }
              





            };
        }
    }

    public class GetAllAddSummaryPageActivityViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<AddSummaryPageActivityViewModel>()
            { 
              new AddSummaryPageActivityViewModel
              {
                ActivityCompletedByUser = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                DateDeactivated = DateTime.UtcNow,
                CreatedBy = Guid.NewGuid(),
                ActivityDate = DateTime.UtcNow,
                ActivityId = Guid.NewGuid(),
                DeactivatedBy = Guid.NewGuid(),
                Guid = Guid.NewGuid(),
                Id = 1,
                IsActivityAdded = true,
                ModifiedBy = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow,
                PersonEntityId = "Test",
                ProjectId = Guid.NewGuid(),
                ActivityCompletedByUserName = "Test",
                ActivityName = "Activity-Test",
                activityViewModel = new ActivityViewModel()
                {
                    ActivityCategoryId = Guid.NewGuid(),
                    EntityTypes = new List<Guid>(),
                    Guid = Guid.NewGuid(),
                    ActivityName = "Test",
                    ActivityRoleNames = new List<string> { "System Admin", "Project Admin" },
                    ActivityRoles = new List<Guid>(),
                    ActivityStatusId = Guid.NewGuid(),
                    ActivityStatusName = "Test",
                    CreatedBy = Guid.NewGuid(),
                    CreatedDate = DateTime.UtcNow,
                    DateDeactivated = DateTime.UtcNow,
                    DeactivatedBy = Guid.NewGuid(),
                    DependentActivityId = Guid.NewGuid(),
                    EndDate = DateTime.UtcNow,
                    Forms = new List<FormActivityViewModel>()
                    {
                        new FormActivityViewModel()
                        {
                            FormTitle="Test",
                            Id=Guid.NewGuid(),
                            Roles=new List<Guid>(),
                            Status="Active",
                        }
                    },
                    Id = 1,
                    FormViewModelList = new List<FormViewModel>()
                    {
                        new FormViewModel()
                        {
                            FormStatusId=Guid.NewGuid(),
                            Guid=Guid.NewGuid(),
                            FormState=1,
                            FormCategoryId=Guid.NewGuid(),
                            FormDataEntryGuid=Guid.NewGuid(),
                            ApprovedBy=1,
                            ApprovedDate=DateTime.UtcNow,
                            CreatedBy=Guid.NewGuid(),
                            CreatedDate=DateTime.UtcNow,
                            DateDeactivated=DateTime.UtcNow,
                            DeactivatedBy=Guid.NewGuid(),
                            EntityTypes=new List<Guid>(),
                            FormTitle="Test",
                            FormUsedInActivityList=new List<Guid>(),
                            Id=1,
                            IsDefaultForm=1,
                            IsNewForm=true,
                            IsPublished=true,
                            IsTemplate=true,
                            ModifiedBy=Guid.NewGuid(),
                            ModifiedDate=DateTime.UtcNow,
                            PreviousVersion=1,
                            ProjectId=Guid.NewGuid(),
                            TenantId=Guid.NewGuid(),
                            UsedVariablesNameList=new List<string>(),
                            Variables=new List<FormVariableViewModel>()
                            {
                                new FormVariableViewModel()
                                {
                                    DependentVariableId=Guid.NewGuid(),
                                    FormVariableIsApprovedStatus=true,
                                    FormVariableRoles=new List<Guid>(),
                                    formVariableRoleViewModel=new List<FormVariableRoleViewModel>()
                                    {
                                        new FormVariableRoleViewModel()
                                        {
                                            CanCreate=true,
                                            CanDelete=true,
                                            CanEdit=true,
                                            CanView=true,
                                            FormVariableId=1,
                                            Guid=Guid.NewGuid(),
                                            RoleGuidId=Guid.NewGuid(),
                                        }
                                    }

                                }
                            },
                        }
                    }
                }
                }
            };
        }
    }

    public class AddSummaryPageActivityViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new AddSummaryPageActivityViewModel()
            {
                ActivityCompletedByUser = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                DateDeactivated = DateTime.UtcNow,
                CreatedBy = Guid.NewGuid(),
                ActivityDate = DateTime.UtcNow,
                ActivityId = Guid.NewGuid(),
                DeactivatedBy = Guid.NewGuid(),
                Guid = Guid.NewGuid(),
                Id = 1,
                IsActivityAdded = true,
                ModifiedBy = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow,
                PersonEntityId = "Test",
                ProjectId = Guid.NewGuid(),
                ActivityCompletedByUserName="Test",
                ActivityName="Activity-Test",
                activityViewModel=new ActivityViewModel()
                {
                    ActivityCategoryId=Guid.NewGuid(),
                    EntityTypes=new List<Guid>(),
                    Guid=Guid.NewGuid(),
                    ActivityName="Test",
                    ActivityRoleNames = new List<string> { "System Admin", "Project Admin" },
                    ActivityRoles=new List<Guid>(),
                    ActivityStatusId=Guid.NewGuid(),
                    ActivityStatusName="Test",
                    CreatedBy=Guid.NewGuid(),
                    CreatedDate=DateTime.UtcNow,
                    DateDeactivated=DateTime.UtcNow,
                    DeactivatedBy=Guid.NewGuid(),
                    DependentActivityId=Guid.NewGuid(),
                    EndDate=DateTime.UtcNow,
                    Forms=new List<FormActivityViewModel>()
                    {
                        new FormActivityViewModel()
                        {
                            FormTitle="Test",
                            Id=Guid.NewGuid(),
                            Roles=new List<Guid>(),
                            Status="Active",
                        }
                    },
                    Id=1,
                    FormViewModelList=new List<FormViewModel>()
                    {
                        new FormViewModel()
                        {
                            FormStatusId=Guid.NewGuid(),
                            Guid=Guid.NewGuid(),
                            FormState=1,
                            FormCategoryId=Guid.NewGuid(),
                            FormDataEntryGuid=Guid.NewGuid(),
                            ApprovedBy=1,
                            ApprovedDate=DateTime.UtcNow,
                            CreatedBy=Guid.NewGuid(),
                            CreatedDate=DateTime.UtcNow,
                            DateDeactivated=DateTime.UtcNow,
                            DeactivatedBy=Guid.NewGuid(),
                            EntityTypes=new List<Guid>(),
                            FormTitle="Test",
                            FormUsedInActivityList=new List<Guid>(),
                            Id=1,
                            IsDefaultForm=1,
                            IsNewForm=true,
                            IsPublished=true,
                            IsTemplate=true,
                            ModifiedBy=Guid.NewGuid(),
                            ModifiedDate=DateTime.UtcNow,
                            PreviousVersion=1,
                            ProjectId=Guid.NewGuid(),
                            TenantId=Guid.NewGuid(),
                            UsedVariablesNameList=new List<string>(),
                            Variables=new List<FormVariableViewModel>()
                            {
                                new FormVariableViewModel()
                                {
                                    DependentVariableId=Guid.NewGuid(),
                                    FormVariableIsApprovedStatus=true,
                                    FormVariableRoles=new List<Guid>(),
                                    formVariableRoleViewModel=new List<FormVariableRoleViewModel>()
                                    {
                                        new FormVariableRoleViewModel()
                                        {
                                            CanCreate=true,
                                            CanDelete=true,
                                            CanEdit=true,
                                            CanView=true,
                                            FormVariableId=1,
                                            Guid=Guid.NewGuid(),
                                            RoleGuidId=Guid.NewGuid(),
                                        }
                                    }

                                }
                            },                                                       
                        }                        
                    }                    
                }
            };
        }
    }

}
