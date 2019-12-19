using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class ActivitySchedulingViewModel
    {
        public int Id { get; set; }
        public Guid ActivityId { get; set; }
        public int ScheduleType { get; set; }
        public string ScheduleValue { get; set; }
        public Guid? ParentActivityId { get; set; }
        public string Offset { get; set; }
        public int? EndsType { get; set; }
        public string EndsValue { get; set; }
        public string BeforeSchedule { get; set; }
        public string AfterSchedule { get; set; }
        public DateTime? StartDate{ get; set; }
        public DateTime? EndDate { get; set; }
    }
}
