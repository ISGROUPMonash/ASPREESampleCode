using Aspree.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aspree.Controllers.Mongo
{
    public class SummaryController : BaseController
    {
        private readonly WebApiHandler _webApi;
        public SummaryController()
        {
            _webApi = new WebApiHandler();
        }
        // GET: Summary
        [Route("Mongo/Summary/Index/{projectId}/{participantID}")]
        public ActionResult Index(Guid? projectId = null, Int64? participantID = null)
        {
            //WriteLog("Web.SummaryController: start.");

            #region store last search result
            if (projectId == null  && participantID == null)
            {
                participantID = TempData.Peek("SummaryPageParticipantId") as Int64?;
                projectId = TempData.Peek("SummaryPageGuid") as Guid?;
            }
            TempData["SummaryPageParticipantId"] = participantID;
            TempData["SummaryPageGuid"] = projectId;

            TempData.Keep("SummaryPageParticipantId");
            TempData.Keep("SummaryPageGuid");
            #endregion

            #region url checking
            string absolutePath = string.Empty;
            try
            {
                /////--------local host test url.
                //absolutePath = (Request.Url.Segments.Count() > 1 ? Request.Url.Segments[1].ToLower() : string.Empty);

                ////---------live domain test url.
                bool isTesturipath = Request.Url.AbsoluteUri.ToLower().Contains("uds-test");
                absolutePath = isTesturipath ? "test/" : "";
            }
            catch (Exception exc)
            { }
            #endregion

            #region API- get project information
            if (projectId != null && participantID!=null)
            {
                Session["ProjectId"] = projectId.ToString();
                Session["ProjectName"] = ViewBag.ProjectName;
                ViewBag.ProjectId = projectId.ToString();

                var projectname = _webApi.Get("MongoDB_Summary/GetSummaryDetails/" + projectId + "/" + participantID);
                if (projectname.MessageType == "Success")
                {
                    Core.ViewModels.MongoViewModels.SummaryViewModel project = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.MongoViewModels.SummaryViewModel>(projectname.Content);
                    return View(project);
                }
            }
            #endregion

            #region sql-code
            //List<Core.ViewModels.SchedulingViewModel> UserActivity = new List<Core.ViewModels.SchedulingViewModel>();
            //List<Core.ViewModels.ActivityViewModel> allActivity = new List<Core.ViewModels.ActivityViewModel>();

            ////var allActivityResponse = _webApi.Get("Activity");
            //var allActivityResponse = _webApi.Get("Scheduling/GetAllScheduledActivityByProjectId/" + guid);
            //if (allActivityResponse.MessageType == "Success")
            //{
            //    List<Core.ViewModels.SchedulingViewModel> allActivity1 = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.SchedulingViewModel>>(allActivityResponse.Content);
            //    //UserActivity = allActivity.Where(x => x.ActivityName.Contains(formName) & x.ProjectId.ToString() == ViewBag.ProjectId).ToList();
            //    //UserActivity = allActivity.Where(x => x.EntityTypes.Contains(EntityTypeGuid) & x.ProjectId.ToString() == ViewBag.ProjectId).ToList();
            //    UserActivity = allActivity1.Where(x => x.EntityTypes.Contains(EntityTypeGuid)).ToList();

            //    if (absolutePath == "test/")
            //    {
            //        UserActivity = UserActivity.Where(x => x.Status == (int)Core.Enum.ActivityDeploymentStatus.Pushed || x.Status == (int)Core.Enum.ActivityDeploymentStatus.Deployed).ToList();
            //    }
            //    else
            //    {
            //        UserActivity = UserActivity.Where(x => x.Status == (int)Core.Enum.ActivityDeploymentStatus.Deployed).ToList();
            //    }
            //}
            //var allSummaryPageActivityResponse = new Utility.ResponseMessage();
            //if (absolutePath == "test/")
            //{
            //    WriteLog("Web.SummaryController: gat all test site summary page activity.");
            //    allSummaryPageActivityResponse = _webApi.Get("Test/Activity/GetAllSummaryPageActivity/" + participant + "/" + guid);
            //}
            //else
            //{
            //    WriteLog("Web.SummaryController: gat all live site summary page activity.");
            //    allSummaryPageActivityResponse = _webApi.Get("Activity/GetAllSummaryPageActivity/" + participant + "/" + guid);
            //}

            //if (allSummaryPageActivityResponse.MessageType == "Success")
            //{
            //    List<Core.ViewModels.AddSummaryPageActivityViewModel> sumarypageactivitylist = new List<Core.ViewModels.AddSummaryPageActivityViewModel>();
            //    sumarypageactivitylist = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.AddSummaryPageActivityViewModel>>(allSummaryPageActivityResponse.Content);
            //    ViewBag.SummaryPageActivityList = sumarypageactivitylist.OrderByDescending(x => x.ActivityDate).ToList();// Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.AddSummaryPageActivityViewModel>>(allSummaryPageActivityResponse.Content);
            //    List<Guid> removeList = new List<Guid>();

            //    foreach (Core.ViewModels.SchedulingViewModel activities in UserActivity)
            //    {
            //        DateTime offsetDate = entityCreationDate;
            //        if (activities.ScheduledToBeCompleted == (int)Core.Enum.ScheduledToBeCompleted.Offset_from_another_activity)
            //        {
            //            Int16 count = 0;

            //            if (activities.OffsetCount != null)
            //                count = Convert.ToInt16(activities.OffsetCount);

            //            switch (activities.OffsetType)
            //            {
            //                case (int)Core.Enum.SchedulingOffsetType.Day:
            //                    offsetDate = offsetDate.AddDays(count);
            //                    break;
            //                case (int)Core.Enum.SchedulingOffsetType.Weeks:
            //                    offsetDate = offsetDate.AddDays(count * 7);
            //                    break;
            //                case (int)Core.Enum.SchedulingOffsetType.Month:
            //                    offsetDate = offsetDate.AddMonths(count);
            //                    break;
            //                case (int)Core.Enum.SchedulingOffsetType.Year:
            //                    offsetDate = offsetDate.AddYears(count);
            //                    break;
            //                default:
            //                    break;
            //            }

            //            if (offsetDate > DateTime.UtcNow)
            //            {
            //                if (activities.OtherActivity == _thisActivityGuid)
            //                {
            //                    removeList.Add(activities.ActivityId);
            //                }
            //            }
            //        }
            //        if (activities.ActivityAvailableForCreation == (int)Core.Enum.SchedulingActivityAvailableForCreation.Only_if_specified_activity_had_already_been_created)
            //        {
            //            Core.ViewModels.AddSummaryPageActivityViewModel isAvaliable = sumarypageactivitylist.FirstOrDefault(x => x.ActivityId == activities.SpecifiedActivity && x.ProjectId == guid);
            //            if (isAvaliable == null)
            //            {
            //                removeList.Add(activities.ActivityId);
            //            }
            //        }


            //        if (activities.ActivityAvailableForCreation == (int)Core.Enum.SchedulingActivityAvailableForCreation.Based_on_calendar_month_before_or_after_scheduled_date)
            //        {
            //            if (activities.ScheduledToBeCompleted == (int)Core.Enum.ScheduledToBeCompleted.Offset_from_another_activity)
            //            {
            //                int? start = activities.CreationWindowOpens;
            //                int? ends = activities.CreationWindowClose;
            //                try
            //                {
            //                    DateTime acDate = offsetDate;
            //                    DateTime edate = acDate.AddMonths((int)ends);
            //                    DateTime sdate = acDate.AddMonths(-(int)start);

            //                    if (sdate > DateTime.UtcNow || edate < DateTime.UtcNow)
            //                    {
            //                        removeList.Add(activities.ActivityId);
            //                    }
            //                    else
            //                    {
            //                        removeList.Remove(activities.ActivityId);
            //                    }
            //                }
            //                catch (Exception exc) { }
            //            }
            //            else
            //            {
            //                int? start = activities.CreationWindowOpens;
            //                int? ends = activities.CreationWindowClose;
            //                try
            //                {
            //                    DateTime acDate = (DateTime)activities.ScheduleDate;
            //                    DateTime edate = acDate.AddMonths((int)ends);
            //                    DateTime sdate = acDate.AddMonths(-(int)start);

            //                    if (sdate > DateTime.UtcNow || edate < DateTime.UtcNow)
            //                    {
            //                        removeList.Add(activities.ActivityId);
            //                    }
            //                }
            //                catch (Exception exc) { }
            //            }
            //        }

            //        if (activities.ActivityAvailableForCreation == (int)Core.Enum.SchedulingActivityAvailableForCreation.Based_on_days_before_or_after_scheduled_date)
            //        {
            //            if (activities.ScheduledToBeCompleted == (int)Core.Enum.ScheduledToBeCompleted.Offset_from_another_activity)
            //            {
            //                int? start = activities.CreationWindowOpens;
            //                int? ends = activities.CreationWindowClose;
            //                try
            //                {
            //                    DateTime acDate = offsetDate;
            //                    DateTime edate = acDate.AddDays((int)ends);
            //                    DateTime sdate = acDate.AddDays(-(int)start);

            //                    if (sdate > DateTime.UtcNow || edate < DateTime.UtcNow)
            //                    {
            //                        removeList.Add(activities.ActivityId);
            //                    }
            //                    else
            //                    {
            //                        removeList.Remove(activities.ActivityId);
            //                    }
            //                }
            //                catch (Exception exc) { }
            //            }
            //            else
            //            {
            //                int? start = activities.CreationWindowOpens;
            //                int? ends = activities.CreationWindowClose;
            //                try
            //                {
            //                    DateTime acDate = (DateTime)activities.ScheduleDate;
            //                    DateTime edate = acDate.AddDays((int)ends);
            //                    DateTime sdate = acDate.AddDays(-(int)start);

            //                    if (sdate > DateTime.UtcNow || edate < DateTime.UtcNow)
            //                    {
            //                        removeList.Add(activities.ActivityId);
            //                    }
            //                }
            //                catch (Exception exc) { }
            //            }
            //        }

            //    }


            //    //remove activities based on scheduling options
            //    if (removeList.Count() > 0)
            //        UserActivity = UserActivity.Where(x => !removeList.Contains(x.ActivityId)).ToList();



            //    //foreach (var remove in removeList)
            //    //{
            //    //    var item = UserActivity.SingleOrDefault(x => x.ActivityId == remove);

            //    //    if (item != null)
            //    //        UserActivity.Remove(item);
            //    //}
            //}
            #endregion

            return View();
        }
    }
}