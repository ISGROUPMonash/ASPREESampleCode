using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class ReportViewModel
    {
        public ReportViewModel()
        {
            results = new List<results>();
        }
        /// <summary>
        /// Pagination links
        /// </summary>
        public metadata metadata { get; set; }
        /// <summary>
        /// Results of query
        /// </summary>
        public List<results> results { get; set; }
    }
    public class Report_ActivityReportViewModel
    {
        public Report_ActivityReportViewModel()
        {
            forms = new List<Report_FormReportViewModel>();
        }
        /// <summary>
        /// The GUID of the created activity
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// Name of the activity
        /// </summary>
        /// <example>Test Activity</example>
        /// <value>AAAAAAAAAA</value>
        /// <remarks>REMARKSSSS</remarks>
        [System.ComponentModel.DefaultValue("My Default String Value")]
        public string name { get; set; }
        /// <summary>
        /// Timestamp of creation in ISO-8601 format
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd'T'HH:mm:ss.fffK}")]
        public DateTime timestamp { get; set; }
        /// <summary>
        /// GUID of entity that created the activity
        /// </summary>
        public Guid createdById { get; set; }
        /// <summary>
        /// A list of forms entered for the activity
        /// </summary>
        public List<Report_FormReportViewModel> forms { get; set; }
    }
    public class Report_FormReportViewModel
    {
        public Report_FormReportViewModel()
        {
            variables = new List<Report_VariableReportViewModel>();
        }
        /// <summary>
        /// The GUID of the entered form data
        /// </summary>
        public Guid id { get; set; }
        /// <summary>
        /// Name of the form
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Version number of the form
        /// </summary>
        public int version { get; set; }
        /// <summary>
        /// Timestamp of submission in ISO-8601 format
        /// </summary>
        public DateTime timestamp { get; set; }
        /// <summary>
        /// GUID of the entity submitting the form
        /// </summary>
        public Guid submittedById { get; set; }
        /// <summary>
        /// List of variables entered on the form
        /// </summary>
        public List<Report_VariableReportViewModel> variables { get; set; }
    }
    public class Report_VariableReportViewModel
    {
        /// <summary>
        /// Name of variable
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Value entered (use null for blank)
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// Data type
        /// </summary>
        public string type { get; set; }
    }


    public class metadata
    {
        /// <summary>
        /// Link to next page of results
        /// </summary>
        public string next { get; set; }
        /// <summary>
        /// Link to previous page of results
        /// </summary>
        public string previous { get; set; }
    }
    public class results
    {
        /// <summary>
        /// Entity id
        /// </summary>
        public Int64 id { get; set; }

        /// <summary>
        /// A list of activities for an entity
        /// </summary>
        public List<Report_ActivityReportViewModel> activities { get; set; }
    }
    public class data
    {
        public metadata metadata { get; set; }
        public List<results> results { get; set; }
    }



    public class ReportPagingParameterModel
    {
        const int maxPageSize = 20;
        /// <summary>
        /// Page Number(optional)
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Page Number")]
        public int pageNumber { get; set; } = 1;

        /// <summary>
        /// not usable
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        public int _pageSize { get; set; } = 20;

        /// <summary>
        /// The number of form responses per page; default is 20 (optional)
        /// </summary>
        [Newtonsoft.Json.JsonProperty(PropertyName = "Page Size")]
        public int pageSize
        {

            get { return _pageSize; }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
    }




    public class ReportViewModelExamples : IExamplesProvider
    {
        public object GetExamples()
        {
            return new ReportViewModel
            {
                metadata = new metadata()
                {
                    next = "/api/v1/Report/DataExport?projectName=Example Project&pageNumber=3&pageSize=10",
                    previous = "/api/v1/Report/DataExport?projectName=Example Project&pageNumber=1&pageSize=10",
                },
                results = new List<results>()
                {
                    new results
                    {
                        id = 1234,
                        activities = new List<Report_ActivityReportViewModel>()
                        {
                            new Report_ActivityReportViewModel ()
                            {
                                id = Guid.NewGuid(),
                                createdById = Guid.NewGuid(),
                                name = "Example Activity",
                                timestamp = DateTime.UtcNow,
                                forms = new List<Report_FormReportViewModel>()
                                {
                                    new Report_FormReportViewModel()
                                    {
                                        id = Guid.NewGuid(),
                                        name = "Example Form",
                                        version = 1,
                                        timestamp =DateTime.UtcNow,
                                        submittedById = Guid.NewGuid   (),
                                        variables =  new List<Report_VariableReportViewModel>()
                                        {
                                            new Report_VariableReportViewModel() { name = "Example Variable", type = "Text Box", value = "ABCD" },
                                        },
                                    },
                                }
                            }
                        }
                    }
                }


            };
        }
    }
    //    public class Example : IExamplesProvider
    //{
    //    public virtual object GetExamples()
    //    {
    //        var value = new Report_ActivityReportViewModel()
    //        {
    //            id = Guid.NewGuid(),
    //            name = "value name",
    //            createdById = Guid.NewGuid(),
    //            forms = new List<Report_FormReportViewModel>(),
    //            timestamp = DateTime.UtcNow,

    //        };
    //        var model = new Report_ActivityReportViewModel() { Value = value };

    //        return model;
    //    }
    //}

}
