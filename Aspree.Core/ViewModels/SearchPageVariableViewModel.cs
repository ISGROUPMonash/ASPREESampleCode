using System;
using System.Collections.Generic;
using Swashbuckle.Examples;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class SearchPageVariableViewModel
    {
        /// <summary>
        /// List of search variables
        /// </summary>
        public List<KeyValuePair<int, string>> SearchVariables { get; set; }
        /// <summary>
        /// ProjectId
        /// </summary>
        public Guid ProjectId { get; set; }
        /// <summary>
        /// FormId
        /// </summary>
        public Guid FormId { get; set; }
        /// <summary>
        /// Form Title
        /// </summary>
        public string FormTitle { get; set; }
        //public KeyValuePair<int, string> FirstName { get; set; }
        //public KeyValuePair<int, string> Surname { get; set; }
        //public string Type { get; set; }
        //public string SponsorOrganisation { get; set; }
        //public string Address { get; set; }
        //public string Suburb { get; set; }
        //public string State { get; set; }
        //public string PostCode { get; set; }
        //public string Country { get; set; }
        //public string Email { get; set; }
        //public string Phone { get; set; }
        //public string Fax { get; set; }
        //public string Dateofbirth { get; set; }
        //public string Gender { get; set; }
        //public string MiddleName { get; set; }
    }

    public class SearchPageVariableViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new SearchPageVariableViewModel()
            {
                SearchVariables = new List<KeyValuePair<int, string>>() { new KeyValuePair<int, string>(1, "EntID"), new KeyValuePair<int, string>(2, "Name") },
                ProjectId = Guid.NewGuid(),
                FormId = Guid.NewGuid(),
                FormTitle = "Example Title"

            };
        }
    }
}
