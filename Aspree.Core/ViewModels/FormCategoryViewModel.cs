using System;
using Swashbuckle.Examples;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class FormCategoryViewModel
    {
        [IgnoreDataMember]
        public int Id { get; set; }
        /// <summary>
        /// Name of Form Category
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// Form Category created by
        /// </summary>
        public Guid CreatedBy { get; set; }
        /// <summary>
        /// Form Category created date
        /// </summary>
        public System.DateTime CreatedDate { get; set; }
        /// <summary>
        /// Form category modified by
        /// </summary>
        public Nullable<Guid> ModifiedBy { get; set; }
        /// <summary>
        ///  Form category Modified Date
        /// </summary>
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<Guid> DeactivatedBy { get; set; }
        /// <summary>
        /// Form category deactivated date
        /// </summary>
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        /// <summary>
        /// Guid
        /// </summary>
        public System.Guid Guid { get; set; }
        /// <summary>
        /// list of subcategory
        /// </summary>
        public IList<SubCategoryViewModel> Forms { get; set; }
        /// <summary>
        /// Tenat Id
        /// </summary>
        public Guid? TenantId { get; set; }
        /// <summary>
        /// IsDefaultFormCategory
        /// </summary>
        public int IsDefaultFormCategory { get; set; }
    }

    public class GetAllFormCategoryViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
           return new List<FormCategoryViewModel>()
           { 
            new FormCategoryViewModel
            {
                CategoryName = "Example Category Name",
                CreatedBy = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow,
                DeactivatedBy = null,
                DateDeactivated = DateTime.UtcNow,
                Guid = Guid.NewGuid(),
                Forms = null,
                //Forms = (c => new Core.ViewModels.SubCategoryViewModel
                //{
                //    Guid = c.Guid,
                //    Id = c.Id,
                //    Name = c.ActivityName,
                //    Status = c.ActivityStatu.Status,
                //    ProjectId = c.FormDataEntry.Guid,
                //    DateDeactivated = c.DateDeactivated,
                //    RepeatationCount = c.RepeatationCount,
                //    ScheduleType = c.ScheduleType,
                //    StartDate = c.StartDate,
                //    EndDate = c.EndDate,
                //    RepeatationOffset = c.RepeatationOffset,
                //    IsDefaultVariable = c.IsDefaultActivity,
                //    DeploymentStatus = c.ActivitySchedulings.FirstOrDefault(x => x.ActivityId == c.Id) != null ? c.ActivitySchedulings.FirstOrDefault(x => x.ActivityId == c.Id).Status : (int?)null,
                //    IsAllVariableApprovedOfActivity = CheckAllVariableApproval(c),
                //}).OrderBy(x => x.Name).ToList()
                TenantId = Guid.NewGuid(),
                IsDefaultFormCategory = 1
            }
            };
        }
    }

    public class FormCategoryViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new FormCategoryViewModel
            {
                CategoryName = "Example Category Name",
                CreatedBy = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = Guid.NewGuid(),
                ModifiedDate = DateTime.UtcNow,
                DeactivatedBy = null,
                DateDeactivated = DateTime.UtcNow,
                Guid = Guid.NewGuid(),
                Forms = null,
                //Forms = (c => new Core.ViewModels.SubCategoryViewModel
                //{
                //    Guid = c.Guid,
                //    Id = c.Id,
                //    Name = c.ActivityName,
                //    Status = c.ActivityStatu.Status,
                //    ProjectId = c.FormDataEntry.Guid,
                //    DateDeactivated = c.DateDeactivated,
                //    RepeatationCount = c.RepeatationCount,
                //    ScheduleType = c.ScheduleType,
                //    StartDate = c.StartDate,
                //    EndDate = c.EndDate,
                //    RepeatationOffset = c.RepeatationOffset,
                //    IsDefaultVariable = c.IsDefaultActivity,
                //    DeploymentStatus = c.ActivitySchedulings.FirstOrDefault(x => x.ActivityId == c.Id) != null ? c.ActivitySchedulings.FirstOrDefault(x => x.ActivityId == c.Id).Status : (int?)null,
                //    IsAllVariableApprovedOfActivity = CheckAllVariableApproval(c),
                //}).OrderBy(x => x.Name).ToList()
                TenantId = Guid.NewGuid(),
                IsDefaultFormCategory = 1
            };
        }
    }
}
