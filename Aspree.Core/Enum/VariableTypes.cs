using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.Enum
{
    public enum VariableTypes
    {
        Text_Box = 1,
        Free_Text = 2,
        Dropdown = 3,
        Numeric__Integer___ = 4,
        Numeric__Decimal___ = 5,
        Date = 6,
        LKUP = 7,
        Checkbox = 8,
        Heading = 9,
        Other_Text = 10,
        ColorPicker = 11,
        FileType = 12

    }

    public enum VariableStatusTypes
    {
        Active = 1,
        Inactive = 2,
        Draft = 3,
        Published = 4,
    }

    public enum DefaultVariableType
    {
        Heading = 2,
        Custom = 1,
        Default = 0,
    }

    public enum VariableCategories
    {
        System = 1,
        Default = 2,
    }

    public enum Variables
    {
        Email = 1,
        UserName = 2,
        Phone = 3,
        Numeric = 4,
    }
    public enum DateFormat
    {
        DD_MM_YYYY = 1,
        MM_DD_YYYY = 2,
        MM_YYYY = 3,
        YYYY = 4,
        DD_MMM_YYYY = 5,
        MMM_YYYY = 6
    }
    public enum DefaultsVariables
    {
        EntID = 3,
        EntGrp = 4,
        EntType = 5,
        PerSType = 6,
        Name = 15,
        FirstName = 16,
        MiddleName = 17,
        NoMidNm = 18,
        Email = 38,
        Username = 40,
        DOB = 44,
        Gender = 45,
        AuthenticationMethod = 51,
        ProRole = 56,
        LnkPro = 52,
        SysAppr = 41,
        Active = 42,
        ProjectDisplayName = 319,
        SysRole = 43,

        RecruitStart = 664,
        RecruitEnd = 665,

        HospSType =7,
        PracSType=8,
        LabSType=9,
        ProSType=10,
        GovSType = 11,

        IndSType=12,
        ConSType=13,

    }
}
