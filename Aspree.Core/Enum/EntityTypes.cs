using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.Enum
{
    public enum EntityTypes
    {
        Person = 1,
        NonPerson = 2,
        Participant = 3,
        Place__Group = 4,
        Project = 5,
    }


    public enum EntityTypesListInDB
    {
        /// <summary>
        /// Person
        /// </summary>
        Person = 1,
        /// <summary>
        /// Hospital
        /// </summary>
        Hospital = 2,
        /// <summary>
        /// Practice/Clinic
        /// </summary>
        Practice__Clinic = 3,
        /// <summary>
        /// Laboratory
        /// </summary>
        Laboratory = 4,
        /// <summary>
        /// Medical Imaging
        /// </summary>
        Medical_Imaging = 5,
        /// <summary>
        /// Research facility/University
        /// </summary>
        Research_facility__University = 6,
        /// <summary>
        /// Project
        /// </summary>
        Project = 7,
        /// <summary>
        /// Healthcare Group
        /// </summary>
        Healthcare_Group = 8,
        /// <summary>
        /// Government Organisation
        /// </summary>
        Government_Organisation = 9,
        /// <summary>
        /// Industry Group
        /// </summary>
        Industry_Group = 10,
        /// <summary>
        /// Consumer Group
        /// </summary>
        Consumer_Group = 11,
        /// <summary>
        /// Activity Venue
        /// </summary>
        Activity_Venue = 12,
        /// <summary>
        /// Vehicle
        /// </summary>
        Vehicle = 13,
        /// <summary>
        /// MAC
        /// </summary>
        MAC = 14,
        /// <summary>
        /// Ethics Committee
        /// </summary>
        Ethics_Committee = 15,
        /// <summary>
        /// Participant
        /// </summary>
        Participant = 16,
        /// <summary>
        /// Place/Group
        /// </summary>
        Place__Group = 17,
    }


    public enum EntitySubTypesListInDB
    {
        [System.ComponentModel.Description("Medical Practitioner/Allied Health")]
        Medical_Practitioner__Allied_Healt = 1,
        [System.ComponentModel.Description("Non-Medical Practitioner")]
        Non_Medical__Practitioner = 2,
        [System.ComponentModel.Description("Public (Overnight Admissions)")]
        Public_Overnight_Admissions = 3,
        [System.ComponentModel.Description("Public (Day Admissions Only)")]
        Public_Day_Admissions_Only = 4,
        [System.ComponentModel.Description("Private (Overnight Admissions)")]
        Private_Overnight_Admissions = 5,
        [System.ComponentModel.Description("Private (Day Admissions Only)")]
        Private_Day_Admissions_Only = 6,
        [System.ComponentModel.Description("Specialist Clinic")]
        Specialist_Clinic = 7,
        [System.ComponentModel.Description("General Practice")]
        General_Practice = 8,
        [System.ComponentModel.Description("Allied Health Clinic")]
        Allied_Health_Clinic = 9,
        [System.ComponentModel.Description("General Laboratory")]
        General_Laboratory = 10,
        [System.ComponentModel.Description("Genetics Laboratory")]
        Genetics_Laboratory = 11,
        [System.ComponentModel.Description("Registry")]
        Registry = 12,
        [System.ComponentModel.Description("Clinical Trial ")]
        Clinical_Trial = 13,
        [System.ComponentModel.Description("Cohort Study")]
        Cohort_Study = 14,
        [System.ComponentModel.Description("State Health Network")]
        State_Health_Network = 15,
        [System.ComponentModel.Description("National Health Network")]
        National_Health_Network = 16,
        [System.ComponentModel.Description("Regulatory Body (TGA)")]
        Regulatory_Body_TGA = 17,
        [System.ComponentModel.Description("Industry Peak Body")]
        Industry_Peak_Body = 18,
        [System.ComponentModel.Description("Device Manufacturer")]
        Device_Manufacturer = 19,
        [System.ComponentModel.Description("Clinical Craft Group/ Society ")]
        Clinical_Craft_Group_Society = 20,
        [System.ComponentModel.Description("Other")]
        Other = 21,
    }





    public enum EntityTypesListInDBSummary
    {
        /// <summary>
        /// Person
        /// </summary>
        Person = 1,
        /// <summary>
        /// Hospital
        /// </summary>
        Hospital = 2,
        /// <summary>
        /// Practice/Clinic
        /// </summary>
        Practice__Clinic = 3,
        /// <summary>
        /// Laboratory
        /// </summary>
        Laboratory = 4,
        /// <summary>
        /// Medical Imaging
        /// </summary>
        Medical_Imaging = 5,
        /// <summary>
        /// Research facility/University
        /// </summary>
        Research_facility__University = 6,
        ///// <summary>
        ///// Project
        ///// </summary>
        //Project = 7,
        /// <summary>
        /// Healthcare Group
        /// </summary>
        Healthcare_Group = 7,
        /// <summary>
        /// Government Organisation
        /// </summary>
        Government_Organisation = 8,
        /// <summary>
        /// Industry Group
        /// </summary>
        Industry_Group = 9,
        /// <summary>
        /// Consumer Group
        /// </summary>
        Consumer_Group = 10,
        /// <summary>
        /// Activity Venue
        /// </summary>
        Activity_Venue = 11,
        /// <summary>
        /// Vehicle
        /// </summary>
        Vehicle = 12,
        /// <summary>
        /// MAC
        /// </summary>
        MAC = 13,
        /// <summary>
        /// Ethics Committee
        /// </summary>
        Ethics_Committee = 14,
        /// <summary>
        /// Participant
        /// </summary>
        Participant = 15,
        ///// <summary>
        ///// Place/Group
        ///// </summary>
        //Place__Group = 17,
    }

}
