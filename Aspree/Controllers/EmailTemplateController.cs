using Aspree.ExtensionClasses;
using Aspree.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aspree.Controllers
{
    [CustomAuthorizeAttribute(Roles = "Definition Admin")]
    public class EmailTemplateController : BaseController
    {
        private readonly WebApiHandler _webApi;

        public EmailTemplateController()
        {
            _webApi = new WebApiHandler();
        }
        // GET: EmailTemplate
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Edit(Guid guid)
        {
            var roles = new List<Core.ViewModels.PrivilegeSmallViewModel>();
            var role = new Core.ViewModels.EmailTemplateViewModel();
            ViewBag.Guid = guid;
            var roleResponse = _webApi.Get("EmailTemplate/" +guid.ToString());

            if (roleResponse.MessageType == "Success")
            {
                var editRole = Newtonsoft.Json.JsonConvert.DeserializeObject<Core.ViewModels.EmailTemplateViewModel>(roleResponse.Content);
                role.Guid = editRole.Guid;
                role.MailBody = editRole.MailBody;
                role.ModifiedBy = editRole.ModifiedBy;
                role.ModifiedDate = editRole.ModifiedDate;
                role.EventName = editRole.EventName;
                role.PushEmailEventID = editRole.PushEmailEventID;
                role.Subject = editRole.Subject;
                role.EventList = editRole.EventList;
                role.EMailKeywords = editRole.EMailKeywords;
            }
            return View(role);
        }
    }
}