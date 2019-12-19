using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class ActivityViewModel
    {
        /// <summary>
        /// Id of activity
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Name of activity
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// DependentActivityId of activity
        /// </summary>
        public Nullable<Guid> DependentActivityId { get; set; }
        /// <summary>
        /// Category id of activity
        /// </summary>
        public Nullable<Guid> ActivityCategoryId { get; set; }
        /// <summary>
        /// Start date of activity
        /// </summary>
        public Nullable<System.DateTime> StartDate { get; set; }
        /// <summary>
        /// End date of activity
        /// </summary>
        public Nullable<System.DateTime> EndDate { get; set; }
        /// <summary>
        /// Schedult type of activity
        /// </summary>
        public int ScheduleType { get; set; }
        /// <summary>
        /// Status id of activity
        /// </summary>
        public Guid ActivityStatusId { get; set; }
        /// <summary>
        /// status name of activity
        /// </summary>
        public string ActivityStatusName { get; set; }


        /// <summary>
        /// UserTypeRole of activity
        /// </summary>
        public string UserTypeRole { get; set; }
        /// <summary>
        /// IsFormContaindData of activity
        /// </summary>
        public bool IsFormContaindData { get; set; }



        /// <summary>
        /// GUID of activity creator
        /// </summary>
        public Guid CreatedBy { get; set; }
        /// <summary>
        /// Date of activity created
        /// </summary>
        public System.DateTime CreatedDate { get; set; }
        /// <summary>
        /// GUID of activity modifier
        /// </summary>
        public Nullable<Guid> ModifiedBy { get; set; }
        /// <summary>
        /// Date of activity Modified
        /// </summary>
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        /// <summary>
        /// GUID of activity deactivator
        /// </summary>
        public Nullable<Guid> DeactivatedBy { get; set; }
        /// <summary>
        /// Date of activity deactivation
        /// </summary>
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        /// <summary>
        /// GUID of activity
        /// </summary>
        public System.Guid Guid { get; set; }
        /// <summary>
        /// GUID of project
        /// </summary>
        public System.Guid ProjectId { get; set; }
        /// <summary>
        /// GUID of tenant
        /// </summary>
        public Nullable<Guid> TenantId { get; set; }
        /// <summary>
        /// List of activity forms
        /// </summary>
        public List<FormActivityViewModel> Forms { get; set; }
        /// <summary>
        /// List of Entity type GUID
        /// </summary>
        public List<Guid> EntityTypes { get; set; }
        /// <summary>
        /// List of roles GUID
        /// </summary>
        public List<Guid> ActivityRoles { get; set; }
        /// <summary>
        /// List of roles names
        /// </summary>
        public List<string> ActivityRoleNames { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public bool PreviewSchedulingDone { get; set; }
        /// <summary>
        /// Type of activity (system generated or custom)
        /// </summary>
        public int IsDefaultActivity { get; set; }
        /// <summary>
        /// List of forms view model
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public List<FormViewModel> FormViewModelList { get; set; }
        /// <summary>
        /// is activity depends on another activity
        /// </summary>
        public bool? IsActivityRequireAnEntity { get; set; }
        /// <summary>
        /// Repeatation Type of activity
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public Nullable<int> RepeatationType { get; set; }
        /// <summary>
        /// Repeatation Count of activity
        /// </summary>
        public Nullable<int> RepeatationCount { get; set; }
        /// <summary>
        /// Repeatation Offset activity
        /// </summary>
        public Nullable<int> RepeatationOffset { get; set; }

        /// <summary>
        /// status id of activity
        /// </summary>
        public int ActivityStatusIdInt { get; set; }
    }

    public class NewActivityViewModel
    {
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Activity Name")]
        public string ActivityName { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Repeatation Type")]
        public Nullable<int> RepeatationType { get; set; }
        public Nullable<int> RepeatationCount { get; set; }
        public Nullable<int> RepeatationOffset { get; set; }
        public Nullable<Guid> DependentActivityId { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Activity Category")]
        public Nullable<Guid> ActivityCategoryId { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public int ScheduleType { get; set; }
        public Guid ActivityStatusId { get; set; }
        public Nullable<Guid> TenantId { get; set; }
        public List<FormActivityViewModel> Forms { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Entity Types")]
        public List<Guid> EntityTypes { get; set; }

        //[Required(ErrorMessage = "Required")]
        [Display(Name = "Activity Roles")]
        public List<Guid> ActivityRoles { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Project")]
        public System.Guid ProjectId { get; set; }
        public bool? IsActivityRequireAnEntity { get; set; }
    }


    public class EditActivityViewModel
    {
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Activity Name")]
        public string ActivityName { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Repeatation Type")]
        public Nullable<int> RepeatationType { get; set; }
        public Nullable<int> RepeatationCount { get; set; }
        public Nullable<int> RepeatationOffset { get; set; }
        public Nullable<Guid> DependentActivityId { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Activity Category")]
        public Nullable<Guid> ActivityCategoryId { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public int ScheduleType { get; set; }
        public Guid ActivityStatusId { get; set; }
        public System.Guid Guid { get; set; }
        public Nullable<Guid> TenantId { get; set; }
        public List<FormActivityViewModel> Forms { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Entity Types")]
        public List<Guid> EntityTypes { get; set; }
        //[Required(ErrorMessage = "Required")]
        [Display(Name = "Activity Roles")]
        public List<Guid> ActivityRoles { get; set; }
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Project")]
        public System.Guid ProjectId { get; set; }
        public bool? IsActivityRequireAnEntity { get; set; }
    }

    public class FormActivityViewModel
    {
        /// <summary>
        /// Id of form
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Title of the form
        /// </summary>
        public string FormTitle { get; set; }
        /// <summary>
        /// Status of the form
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// GUID list of roles 
        /// </summary>
        public List<Guid> Roles { get; set; }
    }

    public class GetAllActivityViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<ActivityViewModel>()
            {
                new ActivityViewModel
                {
                    ActivityName = "Example Activity",
                    ActivityCategoryId = Guid.NewGuid(),
                    ActivityRoleNames = new List<string> { "System Admin", "Project Admin" },
                    ActivityRoles = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                    ActivityStatusId = Guid.NewGuid(),
                    ActivityStatusName = "Active",
                    CreatedBy = Guid.NewGuid(),
                    CreatedDate = DateTime.UtcNow,
                    DateDeactivated = DateTime.UtcNow,
                    DeactivatedBy = Guid.NewGuid(),
                    DependentActivityId = Guid.NewGuid(),
                    EndDate = DateTime.UtcNow.AddDays(5),
                    EntityTypes = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                    Forms = new List<FormActivityViewModel>() {
                        new FormActivityViewModel
                        {
                            Id = Guid.NewGuid(),
                            FormTitle = "Example Form",
                            Status = "Draft",
                            Roles = new List<Guid> {Guid.NewGuid(), }
                        }
                    },
                    FormViewModelList = null,
                    Guid = Guid.NewGuid(),
                    Id = 1,
                    IsActivityRequireAnEntity = false,
                    IsDefaultActivity = 1,
                    ModifiedBy = Guid.NewGuid(),
                    ModifiedDate = DateTime.UtcNow,
                    PreviewSchedulingDone = false,
                    ProjectId = Guid.NewGuid(),
                    RepeatationCount = 1,
                    RepeatationOffset = 1,
                    RepeatationType = 1,
                    ScheduleType = 1,
                    StartDate = DateTime.UtcNow,
                    TenantId = Guid.NewGuid(),
                }
            };
        }
    }

    public class ActivityViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ActivityViewModel
            {
                ActivityName = "Example Activity",
                ActivityCategoryId = Guid.NewGuid(),
                ActivityRoleNames = new List<string> { "System Admin", "Project Admin" },
                ActivityRoles = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                ActivityStatusId = Guid.NewGuid(),
                ActivityStatusName = "Active",
                CreatedBy = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                DateDeactivated = DateTime.UtcNow,
                DeactivatedBy = Guid.NewGuid(),
                DependentActivityId = Guid.NewGuid(),
                EndDate = DateTime.UtcNow.AddDays(5),
                EntityTypes = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                Forms = new List<FormActivityViewModel>() {
                    new FormActivityViewModel
                    {
                        Id = Guid.NewGuid(),
                        FormTitle = "Example Form",
                        Status = "Draft",
                        Roles = new List<Guid> {Guid.NewGuid(), }
                    }
                },
                FormViewModelList = null,
                Guid = Guid.NewGuid(),
                Id = 1,
                IsActivityRequireAnEntity = false,
                IsDefaultActivity = 1,
                ModifiedBy = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow,
                PreviewSchedulingDone = false,
                ProjectId = Guid.NewGuid(),
                RepeatationCount = 1,
                RepeatationOffset = 1,
                RepeatationType = 1,
                ScheduleType = 1,
                StartDate = DateTime.UtcNow,
                TenantId = Guid.NewGuid(),

            };
        }
    }
    public class EditActivityViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new EditActivityViewModel()
            {
                Guid = Guid.Empty,
                ActivityName = "Example Activity",
                ActivityCategoryId = Guid.NewGuid(),
                ActivityRoles = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                ActivityStatusId = Guid.NewGuid(),
                DependentActivityId = Guid.NewGuid(),
                EndDate = DateTime.UtcNow.AddDays(5),
                EntityTypes = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                Forms = new List<FormActivityViewModel>() {
                    new FormActivityViewModel
                    {
                        Id = Guid.NewGuid(),
                        FormTitle = "Example Form",
                        Status = "Draft",
                    }
                },
                IsActivityRequireAnEntity = false,
                ProjectId = Guid.NewGuid(),
                RepeatationCount = null,
                RepeatationOffset = null,
                RepeatationType = null,
                ScheduleType = 1,
                StartDate = DateTime.UtcNow,
                TenantId = Guid.NewGuid(),
            };
        }
    }
    public class NewActivityViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new NewActivityViewModel()
            {
                ActivityName = "Example Activity",
                ActivityCategoryId = Guid.NewGuid(),
                ActivityRoles = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                ActivityStatusId = Guid.NewGuid(),
                DependentActivityId = Guid.NewGuid(),
                EndDate = DateTime.UtcNow.AddDays(5),
                EntityTypes = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
                Forms = new List<FormActivityViewModel>() {
                    new FormActivityViewModel
                    {
                        Id = Guid.NewGuid(),
                        FormTitle = "Example Form",
                        Status = "Draft",
                    }
                },
                IsActivityRequireAnEntity = false,
                ProjectId = Guid.NewGuid(),
                RepeatationCount = null,
                RepeatationOffset = null,
                RepeatationType = null,
                ScheduleType = 1,
                StartDate = DateTime.UtcNow,
                TenantId = Guid.NewGuid(),
            };
        }
    }
}
