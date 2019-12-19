using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.Enum
{
    public enum ScheduleTypes
    {
        FixedDate = 1,
        ParticipantsJoining = 2,
        ProjectStart = 3
    }
    public enum ScheduleEndsTypes
    {
        Never = 1,
        On = 2,
        After = 3,
    }

    public enum SchedulingTypeQuestions
    {
        When_is_the_activity_scheduled_to_be_completed__ = 1,
        When_should_the_activity_be_available_for_creation_on_the_summary_page__ = 2,
        Which_roles_should_be_able_to_create_the_activity_as_scheduled__ = 3,
        Which_roles_should_be_able_to_create_the_activity_regardless_of_schedule__ = 4,

    }

    public enum SchedulingOffsetType
    {
        Day = 1,
        Weeks = 2,
        Month = 3,
        Year = 4,
    }

    public enum ScheduledToBeCompleted
    {
        Unscheduled = 1,
        Offset_from_another_activity = 2,
    }
    public enum SchedulingActivityAvailableForCreation
    {
        Always_available = 1,
        Only_if_specified_activity_had_already_been_created = 2,
        Based_on_calendar_month_before_or_after_scheduled_date = 3,
        Based_on_days_before_or_after_scheduled_date = 4,
    }
}

