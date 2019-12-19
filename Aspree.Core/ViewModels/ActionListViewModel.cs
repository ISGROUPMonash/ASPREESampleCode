using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    /// <summary>
    /// ActionList Model
    /// </summary>
    public class ActionListViewModel
    {
        /// <summary>
        /// Entity Number
        /// </summary>
        public string EntityNumber { get; set; }

        /// <summary>
        /// Id of an Entity Type
        /// </summary>
        public int EntityTypeId { get; set; }
        /// <summary>
        /// Guid of an Entity Type
        /// </summary>
        public Guid EntityTypeGuid { get; set; }
        /// <summary>
        /// Name of an Entity Type
        /// </summary>
        public string EntityTypeName { get; set; }

        /// <summary>
        /// Id of an Activity
        /// </summary>
        public int ActivityId { get; set; }
        /// <summary>
        /// Guid of an Activity
        /// </summary>
        public Guid ActivityGuid { get; set; }
        /// <summary>
        /// Name of an Activity
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// Id of a form
        /// </summary>
        public int FormId { get; set; }
        /// <summary>
        /// Guid of a form
        /// </summary>
        public Guid FormGuid { get; set; }
        /// <summary>
        /// Name of a form
        /// </summary>
        public string FormName { get; set; }
        /// <summary>
        /// Status id of a form
        /// </summary>
        public int FormStatusId { get; set; }
        /// <summary>
        /// Status Name of a form
        /// </summary>
        public string FormStatusName { get; set; }
        /// <summary>
        /// Created Data
        /// </summary>
        public DateTime CreatedDate { get; set; }
        public int filteredCount { get; set; }
        public int totleRecordCount { get; set; }
        public string SummaryPageActivityId { get; set; }
        public Guid? EntityProjectGuid { get; set; }
        public int ProjectVersion { get; set; }
    }

    /// <summary>
    /// Action list search filter model
    /// </summary>
    public class ActionListSearchParameters
    {
        //default parameters
        /// <summary>
        /// 
        /// </summary>
        public int Draw { get; set; }

        /// <summary>
        /// number of records that the table can display in the current draw.
        /// </summary>
        public int Length { get; set; }
        /// <summary>
        /// start position from where to count
        /// </summary>
        public int Start { get; set; }


        //required parameters
        /// <summary>
        /// Guid of project
        /// </summary>
        public Guid projectId { get; set; }

        //optional parameters
        /// <summary>
        /// Entity type
        /// </summary>
        public string EntityType { get; set; }
        /// <summary>
        /// Entity number
        /// </summary>
        public string EntityNumber { get; set; }
        /// <summary>
        /// Activity name
        /// </summary>
        public string Activity { get; set; }
        /// <summary>
        /// Form name
        /// </summary>
        public string Form { get; set; }
        /// <summary>
        /// Status of form
        /// </summary>
        public string FormStatus { get; set; }

        /// <summary>
        /// Guid of loggedin user
        /// </summary>
        public Guid LoggedInUserGuid { get; set; }
    }

    public class ActionListSearchParametersExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ActionListSearchParameters
            {
                EntityNumber = "1234",
                EntityType = "Person",
                projectId = Guid.NewGuid(),
                FormStatus = "Draft",
                Activity = "Example Activity",
                Form = "Example Form",
                Draw = 1,
                Length = 10,
                Start = 0,
                LoggedInUserGuid = Guid.NewGuid(),
            };
        }
    }
    public class GetAllActionListViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<ActionListViewModel>()
            {
                new ActionListViewModel()
                {
                    EntityNumber = "1234",
                    ActivityId = 1,
                    ActivityGuid = Guid.NewGuid(),
                    ActivityName = "Example Activity",
                    FormId = 1,
                    FormGuid = Guid.NewGuid(),
                    FormName = "Example Form",
                    FormStatusId = (int)Core.Enum.FormStatusTypes.Draft,
                    FormStatusName = Core.Enum.FormStatusTypes.Draft.ToString(),
                    EntityTypeId = 1,
                    EntityTypeGuid = Guid.NewGuid(),
                    EntityTypeName = "Person",
                    CreatedDate = DateTime.UtcNow,
                    filteredCount = 10,
                }
            };
        }
    }
    public class ActionListViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ActionListViewModel()
            {
                EntityNumber = "1234",
                ActivityId = 1,
                ActivityGuid = Guid.NewGuid(),
                ActivityName = "Example Activity",
                FormId = 1,
                FormGuid = Guid.NewGuid(),
                FormName = "Example Form",
                FormStatusId = (int)Core.Enum.FormStatusTypes.Draft,
                FormStatusName = Core.Enum.FormStatusTypes.Draft.ToString(),
                EntityTypeId = 1,
                EntityTypeGuid = Guid.NewGuid(),
                EntityTypeName = "Person",
                CreatedDate = DateTime.UtcNow,
                filteredCount = 10,
            };
            
        }
    }
}
