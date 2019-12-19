using Aspree.Core.Validators;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels.MongoViewModels
{
    [BsonIgnoreExtraElements]
    public class SummaryViewModel
    {
        /// <summary>
        /// List of activity type to add in summary page
        /// </summary>
        public List<SummaryPageActivityTypes> SummaryPageActivityTypeList { get; set; }
        /// <summary>
        /// List of activities added on summary page
        /// </summary>
        public List<SummaryPageActivityViewModel> SummaryPageActivitiesList { get; set; }
        /// <summary>
        /// List of project users
        /// </summary>
        public List<SummaryPageProjectUser> SummaryPageProjectUsersList { get; set; }
        /// <summary>
        /// Guid of project
        /// </summary>
        public Guid ProjectGuid { get; set; }
        /// <summary>
        /// Name of project
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// Entity number of an entity
        /// </summary>
        public string EntityNumber { get; set; }
        /// <summary>
        /// Name of entity
        /// </summary>
        public string EntityUserName { get; set; }
        /// <summary>
        /// Surname of entity
        /// </summary>
        public string EntityUserSurname { get; set; }
        /// <summary>
        /// Date of birth of entity
        /// </summary>
        public string DOB { get; set; }
        /// <summary>
        /// Gender of entity
        /// </summary>
        public string Gender { get; set; }
        /// <summary>
        /// Address of entity
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// Suburb of entity
        /// </summary>
        public string Suburb { get; set; }
        /// <summary>
        /// State of entity
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// Phone no of entity
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// Email of entity
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Entity Type of Entity
        /// </summary>
        public string EntityType { get; set; }
        /// <summary>
        /// Creation date of entity
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Profession of person entity
        /// </summary>
        public string Profession { get; set; }


        /// <summary>
        /// Profile image of entity
        /// </summary>
        public string EntityProfileImage { get; set; }


        public string Postcode { get; set; }
        public string Fax { get; set; }
        public string StrtNum { get; set; }
        public string StrtNum2 { get; set; }
        public string StrtNme { get; set; }
        public string StrtNme2 { get; set; }

    }
    [BsonIgnoreExtraElements]
    public class SummaryPageActivityViewModel
    {
        /// <summary>
        /// Id of summary page activity
        /// </summary>
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId Id { get; set; }

        /// <summary>
        /// version of project
        /// </summary>
        public int ProjectVersion { get; set; }

        /// <summary>
        /// Date of activity on summary page
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ActivityDate { get; set; }

        /// <summary>
        /// Id of activity
        /// </summary>
        public int ActivityId { get; set; }
        /// <summary>
        /// Name of activity
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// Guid of activity
        /// </summary>
        public Guid ActivityGuid { get; set; }
        /// <summary>
        /// id of activity completed by
        /// </summary>
        public int ActivityCompletedById { get; set; }
        /// <summary>
        /// Guid of activity completed by
        /// </summary>
        public Guid ActivityCompletedByGuid { get; set; }
        /// <summary>
        /// Name of activity completed by
        /// </summary>
        public string ActivityCompletedByName { get; set; }
        /// <summary>
        /// Entity number  of entity
        /// </summary>
        public Int64 PersonEntityId { get; set; }
        /// <summary>
        /// Guid of project
        /// </summary>
        public Guid ProjectGuid { get; set; }
        /// <summary>
        /// Name of project
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// activity added by name of user
        /// </summary>
        public string CreatedByName { get; set; }
        /// <summary>
        /// activity added date
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// summary page activity modified by guid
        /// </summary>
        public Nullable<Guid> ModifiedBy { get; set; }
        /// <summary>
        /// summary page activity modification date
        /// </summary>
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        /// <summary>
        /// Summary page activity modified by name
        /// </summary>
        public string ModifiedByName { get; set; }
        /// <summary>
        /// summary page activity deleted by name
        /// </summary>
        public Nullable<Guid> DeactivatedBy { get; set; }
        /// <summary>
        /// summary page activity deletion date
        /// </summary>
        public Nullable<System.DateTime> DateDeactivated { get; set; }
        /// <summary>
        /// summary page activity deleted by name
        /// </summary>
        public string DeactivatedByName { get; set; }

        /// <summary>
        /// list of summary page activity forms
        /// </summary>
        public List<SummaryPageActivityForms> SummaryPageActivityFormsList { get; set; }

        /// <summary>
        /// name of linked project name
        /// </summary>
        public string LinkedProjectName { get; set; }
        /// <summary>
        /// guid of linked project
        /// </summary>
        public Guid? LinkedProjectGuid { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class SummaryPageActivityForms
    {
        /// <summary>
        /// id of form
        /// </summary>
        public int FormId { get; set; }
        /// <summary>
        /// Guid of form
        /// </summary>
        public Guid FormGuid { get; set; }
        /// <summary>
        /// Name of Form
        /// </summary>
        public string FormTitle { get; set; }
        /// <summary>
        /// Status id of form
        /// </summary>
        public int FormStatusId { get; set; }
        /// <summary>
        /// Status name of form
        /// </summary>
        public string FormStatusName { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class SummaryPageActivityTypes
    {
        /// <summary>
        /// id of activity
        /// </summary>
        public int ActivityId { get; set; }
        /// <summary>
        /// name of activity
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// Guid of activity
        /// </summary>
        public Guid ActivityGuid { get; set; }
        /// <summary>
        /// activity type id i.e.(default/custom)
        /// </summary>
        public int IsDefaultActivity { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class SummaryPageProjectUser
    {
        /// <summary>
        /// id of user
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// guid of user
        /// </summary>
        public Guid UserGuid { get; set; }
        /// <summary>
        /// name of user
        /// </summary>
        public string UserName { get; set; }
    }




    public class SummaryViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new SummaryViewModel
            {
                SummaryPageActivityTypeList = new List<SummaryPageActivityTypes>()
                {
                    new SummaryPageActivityTypes()
                    {
                        ActivityId = 1,
                        ActivityGuid = Guid.NewGuid(),
                        ActivityName = "Example Activity",
                        IsDefaultActivity = (int)Core.Enum.DefaultActivityType.Custom,
                    }
                },
                SummaryPageActivitiesList = new List<SummaryPageActivityViewModel>()
                {
                    new SummaryPageActivityViewModel()
                    {
                        Id = new ObjectId(),
                        ActivityId = 1,
                        ActivityGuid =Guid.NewGuid(),
                        ActivityName = "Example Activity",
                        ActivityDate=DateTime.UtcNow,
                        ActivityCompletedById = 1,
                        ActivityCompletedByGuid =Guid.NewGuid(),
                        ActivityCompletedByName = "Example User",
                        CreatedByName = "SA User",
                        CreatedDate = DateTime.UtcNow,
                        DateDeactivated = null,
                        DeactivatedBy = null,
                        DeactivatedByName = null,
                        LinkedProjectGuid = Guid.NewGuid(),
                        LinkedProjectName = "Example Project",
                        ModifiedBy = Guid.NewGuid(),
                        ModifiedByName = "SA User",
                        ModifiedDate = DateTime.UtcNow,
                        PersonEntityId =1,
                        ProjectGuid=Guid.NewGuid(),
                        ProjectName="Example Project",
                        ProjectVersion = 1,
                        SummaryPageActivityFormsList= new List<SummaryPageActivityForms>()
                        {
                            new SummaryPageActivityForms()
                            {FormId = 1,
                                FormGuid = Guid.NewGuid(),
                                FormTitle = "Example Form",
                                FormStatusId = (int)Core.Enum.FormStatusTypes.Draft,
                                FormStatusName = Core.Enum.FormStatusTypes.Draft.ToString(),
                            }
                        }
                    }
                },
                SummaryPageProjectUsersList = new List<SummaryPageProjectUser>()
                {
                    new SummaryPageProjectUser()
                    {
                        UserId =1,
                        UserGuid = Guid.Empty,
                        UserName = "Example User",
                    }
                },
                ProjectGuid = Guid.NewGuid(),
                ProjectName = "Example Project",

                EntityNumber = "1000",
                EntityUserName = "Example Entity",
                EntityUserSurname = "ExampleUserName",
                DOB = "01-01-1990",
                Gender = "Male",
                Address = "",
                Suburb = "",
                State = "",
                Phone = "1234567890",
                Email = "email@example.com",
                EntityType = Core.Enum.EntityTypes.Person.ToString(),
                CreatedDate = DateTime.UtcNow,
            };
        }
    }

    public class SummaryPageActivityViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new SummaryPageActivityViewModel
            {
                Id = ObjectId.GenerateNewId(),
                ActivityId = 1,
                ActivityGuid = Guid.NewGuid(),
                ActivityName = "Example Activity",
                ActivityDate = DateTime.UtcNow,
                ActivityCompletedById = 1,
                ActivityCompletedByGuid = Guid.NewGuid(),
                ActivityCompletedByName = "Example User",
                CreatedByName = "SA User",
                CreatedDate = DateTime.UtcNow,
                DateDeactivated = null,
                DeactivatedBy = null,
                DeactivatedByName = null,
                LinkedProjectGuid = Guid.NewGuid(),
                LinkedProjectName = "Example Project",
                ModifiedBy = Guid.NewGuid(),
                ModifiedByName = "SA User",
                ModifiedDate = DateTime.UtcNow,
                PersonEntityId = 1,
                ProjectGuid = Guid.NewGuid(),
                ProjectName = "Example Project",
                ProjectVersion = 1,
                SummaryPageActivityFormsList = new List<SummaryPageActivityForms>()
                {
                    new SummaryPageActivityForms()
                    {
                        FormId = 1,
                        FormGuid = Guid.NewGuid(),
                        FormTitle = "Example Form",
                        FormStatusId = (int)Core.Enum.FormStatusTypes.Draft,
                        FormStatusName = Core.Enum.FormStatusTypes.Draft.ToString(),
                    }
                }
            };
        }
    }
}
