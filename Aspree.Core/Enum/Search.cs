using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.Enum
{
    public enum Search
    {
    }

    public enum SearchParticipantVariable
    {
        Participant_Id = 1,
        First_Name = 14,
        Middle_Name = 15,
        Surname = 13,
        Date_of_birth = 40,
        Gender = 41,
    }

    public enum SearchStaffPhysicianContactPersonVariable
    {
        Participant_Id = 1,
        First_Name = 14,
        Surname = 13,
        Type = 3,
        Sponsor__Organisation = 0,
        Address = 26,
        Suburb = 22,
        State = 24,
        PostCode = 25,
        Country = 23,
        Email = 35,
        Phone = 0,
        Fax = 0,
    }

    public enum SearchPlaceVariable
    {
        Participant_Id = 1,
        Name = 13,
        Type = 3,
        Sponsor__Organisation = 0,
        Address = 26,
        Suburb = 22,
        State = 24,
        PostCode = 25,
        Country = 23,
        Email = 35,
        Phone = 0,
        Fax = 0,
    }


    public enum SearchForms
    {
        Person_Registration = 1,
        Participant_Registration = 2,
        Place__Group_Registration = 3,
        Project_Registration = 4,
        Unique_Identifier = 5,
    }
}