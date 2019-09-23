using Aspree.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Aspree.Controllers
{
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Common class which is used to call web api 
        /// </summary>
        private readonly Services.WebApiHandler _webApi;
        public BaseController()
        {
            _webApi = new Services.WebApiHandler();
        }
        /// <summary>
        /// Global variable to set error message
        /// </summary>
        /// <param name="message">Error Message</param>
        protected void SetErrorMessage(string message)
        {
            TempData["Error"] = message;
        }
        /// <summary>
        /// Global variable to set sucess message for any operations
        /// </summary>
        /// <param name="message">Sucess Message</param>
        protected void SetSuccessMessage(string message)
        {
            TempData["Success"] = message;
        }
        /// <summary>
        /// Global variable to set base/host variable value
        /// </summary>
        /// <param name="message">Sucess Message</param>
        protected string BaseUrl
        {
            get
            {
                return $"{Request.Url.Scheme}://{Request.Url.Authority}";
            }
        }
        
    }
}