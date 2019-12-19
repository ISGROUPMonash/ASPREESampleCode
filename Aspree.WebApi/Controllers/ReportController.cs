using Aspree.Core.ViewModels;
using Aspree.Provider.Interface;
using Swashbuckle.Examples;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Aspree.WebApi.Controllers
{
    /// <summary>
    /// Report Controller
    /// </summary>
    public class ReportController : BaseController
    {
        private readonly IReportProvider _reportProvider;
        private readonly IUserLoginProvider _userLoginProvider;
        /// <summary>
        /// Reportcontroller constructor
        /// </summary>
        /// <param name="reportProvider"></param>
        /// <param name="userLoginProvider"></param>
        public ReportController(IReportProvider reportProvider, IUserLoginProvider userLoginProvider)
        {
            this._reportProvider = reportProvider;
            this._userLoginProvider = userLoginProvider;
        }

        /// <summary>
        /// Generate Report
        /// </summary>
        /// <remarks>
        /// Generate Report<br></br>
        /// <strong>Purpose.</strong>
        /// - The purpose of this api is to export list of entity in json format.
        /// - This api provides the functionality to filter entity data by project, activity and form name, which takes paging parameters(page size, page number) to support pagination.
        /// - The api returns activity list of related entity, form list of activity and variable list of relevant form.
        /// <br></br>
        /// <strong>Authentication.</strong>
        /// - HTTP Basic Authentication.
        /// </remarks>
        ///// <param name="pagingparametermodel"></param>        
        /// <param name="projectName">Project Name(required)</param>
        /// <param name="activityName">Activity Name(optional)</param>
        /// <param name="formName">Form Name(optional)</param>
        /// <param name="pageNumber">Page Number(optional)</param>
        /// <param name="pageSize">The number of form responses per page; default is 20 (optional)</param>
        /// <returns>Report Data</returns>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "This error is returned by the server when we pass wrong datatype value in request parameter")]
        [SwaggerResponse(HttpStatusCode.Unauthorized, Description = "The user does not exist or does not have local password authentication")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "No records matching the provided filters are found")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        [SwaggerResponseExample(HttpStatusCode.OK, typeof(ReportViewModelExamples))]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(ReportViewModel))]
        [Route("api/v1/Report/DataExport")]
        [HttpGet]
        public ReportViewModel DataExport(
            string projectName,
            string activityName = null,
            string formName = null,
            int pageNumber = 1,
            int pageSize = 20)
        {
            ReportPagingParameterModel pagingparametermodel = new ReportPagingParameterModel() { pageSize = pageSize, pageNumber = pageNumber };
            if (Request.Headers.Authorization == null)
            {
                var msg = new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login.")
                };
                throw new HttpResponseException(msg);
            }

            #region check login

            if (this.LoggedInUserId == Guid.Empty)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login."),
                });
            }
            #endregion

            if (pagingparametermodel == null)
                pagingparametermodel = new ReportPagingParameterModel();
            var source = _reportProvider.GetAll(this.LoggedInUserId, projectName, activityName, formName);

            // Get's No of Rows Count   
            int count = source.results.Count();

            // Parameter is passed from Query string if it is null then it default Value will be pageNumber:1  
            int CurrentPage = pagingparametermodel.pageNumber;

            // Parameter is passed from Query string if it is null then it default Value will be pageSize:20  
            int PageSize = pagingparametermodel.pageSize;

            // Display TotalCount to Records to User  
            int TotalCount = count;

            // Calculating Totalpage by Dividing (No of Records / Pagesize)  
            int TotalPages = (int)Math.Ceiling(count / (double)PageSize);

            // Returns List of Customer after applying Paging   
            //var items = source.results.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            var items1 = source.results.Skip((CurrentPage - 1) * PageSize).Take(PageSize).ToList();
            ReportViewModel items = new ReportViewModel();
            items.results = items1;
            items.metadata = source.metadata;

            // if CurrentPage is greater than 1 means it has previousPage  
            var previousPage = CurrentPage > 1 ? "Yes" : "No";

            // if TotalPages is greater than CurrentPage means it has nextPage  
            var nextPage = CurrentPage < TotalPages ? "Yes" : "No";


            int nextPageNo = CurrentPage + 1;
            int previousPageNo = CurrentPage - 1;
            string basePath = Utilities.ConfigSettings.ReportAPIUrl + "Report/DataExport?projectName=" + projectName + "";

            if (!string.IsNullOrEmpty(activityName))
                basePath = basePath + "&activityName=" + activityName;
            if (!string.IsNullOrEmpty(formName))
                basePath = basePath + "&formName=" + formName;

            items.metadata.next = nextPage == "Yes" ? basePath + "&pageNumber=" + nextPageNo + "&pageSize=" + pageSize : string.Empty;
            items.metadata.previous = previousPage == "Yes" ? basePath + "&pageNumber=" + previousPageNo + "&pageSize=" + pageSize : string.Empty;

            // Object which we are going to send in header   
            var paginationMetadata = new
            {
                totalCount = TotalCount,
                pageSize = PageSize,
                currentPage = CurrentPage,
                totalPages = TotalPages,
                previousPage,
                nextPage
            };

            // Setting Header  
            System.Web.HttpContext.Current.Response.Headers.Add("Paging-Headers", Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
            // Returing List of Customers Collections  
            if (items.results.Count == 0)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Record not found."),
                    ReasonPhrase = "Record not found."
                });
            }
            return items;
        }
    }
}
