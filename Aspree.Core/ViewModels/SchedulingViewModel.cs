using System;
using System.Collections.Generic;
using Swashbuckle.Examples;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class SchedulingViewModel
    {
        public int Id { get; set; }
        /// <summary>
        /// ActivityId of scheduled activity 
        /// </summary>
        public Guid ActivityId { get; set; }
        /// <summary>
        /// No. of activity scheduling to be completed
        /// </summary>
        public int ScheduledToBeCompleted { get; set; }
        /// <summary>
        /// Available activity for creation
        /// </summary>
        public int ActivityAvailableForCreation { get; set; }
        /// <summary>
        /// RolesToCreateActivity
        /// </summary>
        public List<Guid> RolesToCreateActivity { get; set; }
        /// <summary>
        /// RolesToCreateActivity_Name
        /// </summary>
        public List<string> RolesToCreateActivity_Name { get; set; }
        /// <summary>
        /// RoleToCreateActivityRegardlessScheduled
        /// </summary>
        public List<Guid> RoleToCreateActivityRegardlessScheduled { get; set; }
        /// <summary>
        /// RoleToCreateActivityRegardlessScheduled_Name
        /// </summary>
        public List<string> RoleToCreateActivityRegardlessScheduled_Name { get; set; }
        /// <summary>
        /// OtherActivity
        /// </summary>
        public Nullable<Guid> OtherActivity { get; set; }
        /// <summary>
        /// OffsetCount
        /// </summary>
        public Nullable<int> OffsetCount { get; set; }
        /// <summary>
        /// OffsetType
        /// </summary>
        public Nullable<int> OffsetType { get; set; }
        /// <summary>
        /// SpecifiedActivity
        /// </summary>
        public Nullable<Guid> SpecifiedActivity { get; set; }
        /// <summary>
        /// CreationWindowOpens
        /// </summary>
        public Nullable<int> CreationWindowOpens { get; set; }
        /// <summary>
        /// CreationWindowClose
        /// </summary>
        public Nullable<int> CreationWindowClose { get; set; }
        /// <summary>
        /// Guid
        /// </summary>
        public System.Guid Guid { get; set; }
        /// <summary>
        /// Activity Is Scheduled true/false
        /// </summary>
        public Nullable<bool> IsScheduled { get; set; }
        /// <summary>
        /// Schedule date of activity
        /// </summary>
        public Nullable<System.DateTime> ScheduleDate { get; set; }
        /// <summary>
        /// List of Entity type
        /// </summary>
        public List<Guid> EntityTypes { get; set; }
        /// <summary>
        /// Name of Activity
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// Status of Scheduled activity
        /// </summary>
        public int? Status { get; set; }
        /// <summary>
        /// ProjectId
        /// </summary>
        public Guid ProjectId { get; set; }
        /// <summary>
        /// IsDefaultActivity
        /// </summary>
        public int IsDefaultActivity { get; set; }
        /// <summary>
        /// Activity can be created multiple times true/false
        /// </summary>
        public bool CanCreatedMultipleTime { get; set; }





        /// <summary>
        /// Guid of activities that used other activities
        /// </summary>
        public List<Guid> UsedAsOtherActivitiesList { get; set; }

        /// <summary>
        /// Guid of activities that used other activities
        /// </summary>
        public List<Guid> UsedAsSpecifiedActivitiesList { get; set; }


        /// <summary>
        /// status id of activity  i.e. published
        /// </summary>
        public int ActivityStatusId { get; set; }
    }

    public class NewSchedulingViewModel
    {
        [Required(ErrorMessage = "This field is required.")]
        public Guid ActivityId { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public int ScheduledToBeCompleted { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public int ActivityAvailableForCreation { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public List<Guid> RolesToCreateActivity { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public List<Guid> RoleToCreateActivityRegardlessScheduled { get; set; }
        public Nullable<Guid> OtherActivity { get; set; }
        public Nullable<int> OffsetCount { get; set; }
        public Nullable<int> OffsetType { get; set; }
        public Nullable<Guid> SpecifiedActivity { get; set; }
        public Nullable<int> CreationWindowOpens { get; set; }
        public Nullable<int> CreationWindowClose { get; set; }
        public Guid ProjectId { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public bool CanCreatedMultipleTime { get; set; }

    }

    public class EditSchedulingViewModel
    {
        [Required(ErrorMessage = "This field is required.")]
        public Guid ActivityId { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public int ScheduledToBeCompleted { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public int ActivityAvailableForCreation { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public List<Guid> RolesToCreateActivity { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public List<Guid> RoleToCreateActivityRegardlessScheduled { get; set; }
        public Nullable<Guid> OtherActivity { get; set; }
        public Nullable<int> OffsetCount { get; set; }
        public Nullable<int> OffsetType { get; set; }
        public Nullable<Guid> SpecifiedActivity { get; set; }
        public Nullable<int> CreationWindowOpens { get; set; }
        public Nullable<int> CreationWindowClose { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public bool CanCreatedMultipleTime { get; set; }
    }

    public class GetAllSchedulingViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
             return new List<SchedulingViewModel>()
            {
                new SchedulingViewModel
                {
                Id = 1,
                ActivityId = Guid.NewGuid(),
                ScheduledToBeCompleted = 1,
                ActivityAvailableForCreation = 1,
                RolesToCreateActivity = new List<Guid>(),
                RolesToCreateActivity_Name = new List<string>(),
                RoleToCreateActivityRegardlessScheduled = new List<Guid>(),
                RoleToCreateActivityRegardlessScheduled_Name = new List<string>(),
                OtherActivity = null,
                OffsetCount = 3,
                OffsetType = 1,
                SpecifiedActivity = Guid.NewGuid(),
                CreationWindowOpens = 1,
                CreationWindowClose = 2,
                Guid = Guid.NewGuid(),
                IsScheduled = true,
                ScheduleDate = DateTime.UtcNow,
                EntityTypes = new List<Guid>(),
                ActivityName = "Example Activity Name",
                Status = 1,
                ProjectId = Guid.NewGuid(),
                IsDefaultActivity = 1,
                CanCreatedMultipleTime = true
                }


            };
        }
    }
    public class SchedulingViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {

            return new SchedulingViewModel()
            {
                Id = 1,
                ActivityId = Guid.NewGuid(),
                ScheduledToBeCompleted = 1,
                ActivityAvailableForCreation = 1,
                RolesToCreateActivity = new List<Guid>(),
                RolesToCreateActivity_Name = new List<string>(),
                RoleToCreateActivityRegardlessScheduled = new List<Guid>(),
                RoleToCreateActivityRegardlessScheduled_Name = new List<string>(),
                OtherActivity = null,
                OffsetCount = 3,
                OffsetType = 1,
                SpecifiedActivity = Guid.NewGuid(),
                CreationWindowOpens = 1,
                CreationWindowClose = 2,
                Guid = Guid.NewGuid(),
                IsScheduled = true,
                ScheduleDate = DateTime.UtcNow,
                EntityTypes = new List<Guid>(),
                ActivityName = "Example Activity Name",
                Status = 1,
                ProjectId = Guid.NewGuid(),
                IsDefaultActivity = 1,
                CanCreatedMultipleTime = true

            };
        }
    }

    public class NewSchedulingViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {

            return new NewSchedulingViewModel()
            {
                ActivityId = Guid.NewGuid(),
                ScheduledToBeCompleted = 1,
                ActivityAvailableForCreation = 1,
                RolesToCreateActivity = new List<Guid>(),
                RoleToCreateActivityRegardlessScheduled = new List<Guid>(),
                OtherActivity = null,
                OffsetCount = 3,
                OffsetType = 1,
                SpecifiedActivity = Guid.NewGuid(),
                CreationWindowOpens = 1,
                CreationWindowClose = 2,
                ProjectId = Guid.NewGuid(),
                CanCreatedMultipleTime = true

            };
        }
    }

    public class EditSchedulingViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {

            return new EditSchedulingViewModel()
            {
                ActivityId = Guid.NewGuid(),
                ScheduledToBeCompleted = 1,
                ActivityAvailableForCreation = 1,
                RolesToCreateActivity = new List<Guid>(),
                RoleToCreateActivityRegardlessScheduled = new List<Guid>(),
                OtherActivity = null,
                OffsetCount = 3,
                OffsetType = 1,
                SpecifiedActivity = Guid.NewGuid(),
                CreationWindowOpens = 1,
                CreationWindowClose = 2,
                CanCreatedMultipleTime = true

            };
        }
    }
}
