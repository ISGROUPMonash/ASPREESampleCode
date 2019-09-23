using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.Enum
{
    public enum FormStatusTypes
    {
        Active = 1,
        InActive = 2,
        Draft = 3,
        Published = 4,
        Submitted_for_review = 5,
        Not_entered = 6,
    }

    public enum FormStateTypes
    {
        Active = 1,
        InActive = 2,
        Draft = 3,
        Published = 4
    }

    public enum FormCategories
    {
        System = 1,
    }

    public enum DefaultFormType
    {
        Custom = 1,
        Default = 0,

    }
    public enum DefaultFormName
    {
        [System.ComponentModel.Description("Person Registration")]
        Person_Registration = 1,

        [System.ComponentModel.Description("Participant Registration")]
        Participant_Registration = 2,

        [System.ComponentModel.Description("Place/Group Registration")]
        Place__Group_Registration = 3,

        [System.ComponentModel.Description("Project Registration")]
        Project_Registration = 4,

        [System.ComponentModel.Description("Project Linkage")]
        Project_Linkage = 5,
    }
}
