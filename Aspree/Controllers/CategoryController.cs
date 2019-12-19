using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Aspree.Controllers
{
    public class CategoryController : BaseController
    {
        // GET: Category
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Activities()
        {
            return View();

        }
        public ActionResult Forms()
        {
            return View();
        }
        public ActionResult Variables()
        {
            return View();
        }
        public ActionResult EntityType()
        {
            return View();
        }
        public ActionResult EntitySubType()
        { 
            return View();
        }
    }
}