using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class ScheduleActivityViewModel
    {
        public ScheduleActivityViewModel()
        {
            ScheduleActivityList = new List<ViewModels.ActivityViewModel>();
        }
        public List<ActivityViewModel> ScheduleActivityList { get; set; }
        public Guid? TenantId { get; set; }
        public Guid Guid { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class NewScheduleActivityViewModel
    {
        public NewScheduleActivityViewModel()
        {
            ScheduleActivityList = new List<ViewModels.ActivityViewModel>();
            ActivitySchedulingViewModel = new List<ViewModels.SchedulingViewModel>();
        }
        public List<SchedulingViewModel> ActivitySchedulingViewModel { get; set; }
        public List<ActivityViewModel> ScheduleActivityList { get; set; }
        public Guid? TenantId { get; set; }
        public Guid Guid { get; set; }
        public Guid? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
