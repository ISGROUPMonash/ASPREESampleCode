using Aspree.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataTables.Mvc;
using Aspree.Core.ViewModels;
using Aspree.ExtensionClasses;

namespace Aspree.Controllers
{
    [CustomAuthorizeAttribute]
    public class ActionListController : BaseController
    {
        private readonly WebApiHandler _webApi;

        public ActionListController()
        {
            _webApi = new WebApiHandler();
        }

        // GET: ActionList
        public ActionResult Index(Guid? guid = null)
        {
            if (guid == null)
            {
                if (Session["ProjectId"] != null)
                {
                    guid = new Guid(Session["ProjectId"].ToString());
                }
                else
                {
                    Session["ProjectId"] = null;
                    SetErrorMessage("Please select project.");
                    return Redirect("/Home/ListOfProject");
                }
            }

            Session["ProjectId"] = guid.ToString();

            //get project name by guid
            var roles = _webApi.Get("Role");
            if (roles.MessageType == "Success")
            {
                var role = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Core.ViewModels.RoleModel>>(roles.Content);
                var crnRole = role.FirstOrDefault(c => c.Name == this.LoggedInUser.Roles.FirstOrDefault());
                ViewBag.LoggedInUserRoleGuid = crnRole != null ? crnRole.Guid : Guid.Empty;
            }
            ViewBag.ProjectGuid = guid;
            return View();
        }

        public ActionResult GetActionList([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel,
            Guid projectId,
            string EntityType = null,
            int ?EntityNumber = null ,
            string Activity = null,
            string Form = null,
            string FormStatus = null
            )
        {
            ActionListSearchParameters searchmodel = new ActionListSearchParameters();
            searchmodel.Draw = requestModel.Draw;
            searchmodel.Length = requestModel.Length;
            searchmodel.Start = requestModel.Start;
            searchmodel.projectId = projectId;
            searchmodel.EntityType = EntityType;
            searchmodel.EntityNumber = EntityNumber?.ToString();
            searchmodel.Activity = Activity;
            searchmodel.Form = Form;
            searchmodel.FormStatus = FormStatus;
            int totalCount = 0;
            //var totalCountResponse = _webApi.Get("ActionList/CountAllActionList/" + projectId);
            //if (totalCountResponse.MessageType == "Success")
            //{
            //    try { totalCount = Convert.ToInt32(totalCountResponse.Content); } catch (Exception exc) { };
            //}
            var actionlistAPIResponse = _webApi.Post("ActionList/AllActionListData", searchmodel);
            if (actionlistAPIResponse.MessageType == "Success")
            {
                List<ActionListViewModel> resultData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ActionListViewModel>>(actionlistAPIResponse.Content);
                ActionListViewModel filtered = resultData.FirstOrDefault();
                int filteredCount = filtered != null ? filtered.filteredCount : 0;
                totalCount = filtered != null ? filtered.totleRecordCount : 0;
                return Json(new DataTablesResponse(requestModel.Draw, resultData, filteredCount, totalCount), JsonRequestBehavior.AllowGet);
            }
            return Json(new DataTablesResponse(requestModel.Draw, null, 0, 0), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult UpdateProjectId(Guid newProjectId)
        {
            Session["ProjectId"] = newProjectId;
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}