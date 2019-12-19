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
        Heading = 1,
        OtherText = 2,
        EntID = 3,
        EntGrp = 4,
        EntType = 5,
        PerSType = 6,
        HospSType = 7,
        PracSType = 8,
        LabSType = 9,
        ProSType = 10,
        GovSType = 11,
        IndSType = 12,
        ConSType = 13,
        Title = 14,
        Name = 15,
        FirstName = 16,
        MiddleName = 17,
        NoMidNm = 18,
        PrefName = 19,
        Unit = 20,
        NoUnit = 21,
        StrtNum = 22,
        StrtNme = 23,
        StrtType = 24,
        Suburb = 25,
        Country = 26,
        State = 27,
        Postcode = 28,
        DifAddress = 29,
        StrtNum2 = 30,
        StrtNme2 = 31,
        StrtType2 = 32,
        Suburb2 = 33,
        Country2 = 34,
        State2 = 35,
        Postcode2 = 36,
        NoAddress = 37,
        Email = 38,
        Phone = 39,
        Username = 40,
        SysAppr = 41,
        Active = 42,
        SysRole = 43,
        DOB = 44,
        Gender = 45,
        ConfData = 46,
        CnstModel = 47,
        Ethics = 48,
        DataStore = 49,
        ProDt = 50,
        AuthenticationMethod = 51,
        LnkPro = 52,
        Join = 53,
        Actv = 54,
        End = 55,
        ProRole = 56,
        ProjectLogo = 57,
        ProjectColor = 58,
        ProjectDisplayName = 59,
        ProjectDisplayNameTextColour = 60,
        ApiAccessEnabled = 61,
        RecruitStart = 62,
        RecruitEnd = 63,
        PlaceProfilePicture = 147,
        Fax = 0,
    }

    public enum ProjectSubTypeEnum
    {
        Registry = 1,
        Clinical_Trial = 2,
        Cohort_Study = 3,
        Other = 4,
    }
}
