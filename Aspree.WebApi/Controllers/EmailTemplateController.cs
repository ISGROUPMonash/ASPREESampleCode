using Aspree.Core.ViewModels;
using Aspree.Provider.Provider;
using Aspree.Provider.Interface;
using Aspree.WebApi.Utilities;
using Swagger.Net.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Aspree.WebApi.Controllers
{
    [System.Web.Http.Description.ApiExplorerSettings(IgnoreApi = true)]
    public class EmailTemplateController : BaseController
    {
        private readonly IEmailTemplateProvider _emailTemplateProvider;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="emailTemplateProvider"></param>
        public EmailTemplateController(IEmailTemplateProvider emailTemplateProvider)//, IPushEmailEventProvider pushEmailEventProvider
        {
            _emailTemplateProvider = emailTemplateProvider;

        }


        /// <summary>
        /// Get All Email Templates
        /// </summary>
        /// <remarks>
        /// Get All Email Templates
        /// </remarks>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IEnumerable<EmailTemplateViewModel>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        //[SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
         public IEnumerable<EmailTemplateViewModel> GetAll()
        {
            #region check login
            if (this.LoggedInUserId == Guid.Empty)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login."),
                });
            }
            #endregion
            return _emailTemplateProvider.GetAll();
        }

        /// <summary>
        /// Get Email Template By Guid
        /// </summary>
        /// <remarks>
        /// Get Email Template By Guid
        /// </remarks>
        /// <param name="guid">Guid of Email Templated</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(RoleModel))]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        public EmailTemplateViewModel Get(Guid guid)
        {
            #region check login
            if (this.LoggedInUserId == Guid.Empty)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login."),
                });
            }
            #endregion
            var email = _emailTemplateProvider.GetByGuid(guid);
            if (email == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Email was not found."),
                });
            }

            return email;
        }

        /// <summary>
        /// Edit e-mail template By guid
        /// </summary>
        /// <remarks>
        /// Edit e-mail template By guid
        /// </remarks>
        /// <param name="editEmailModel">EmailTemplate Model</param>
        /// <param name="guid">guid of role to be edited</param>
        [SwaggerResponse(HttpStatusCode.BadRequest, Description = "Bad Request")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(RoleModel))]
        [SwaggerResponse(HttpStatusCode.Ambiguous, Description = "Ambiguous")]
        [SwaggerResponse(HttpStatusCode.InternalServerError, Description = "Internal Server Error")]
        [SwaggerResponse(HttpStatusCode.NotFound, Description = "Not Found")]
        //[SwaggerResponse(HttpStatusCode.Unauthorized, Description = "Unauthorized")]
        //[SwaggerResponse(HttpStatusCode.Forbidden, Description = "Forbidden")]
        public HttpResponseMessage Put(Guid guid, [FromBody]EmailTemplateViewModel editEmailModel)
        {
            #region check login
            if (this.LoggedInUserId == Guid.Empty)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Unauthorized access. Please login."),
                });
            }
            #endregion
            if (!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ModelState.AllErrors());
            }

            var EmailEdit = _emailTemplateProvider.Update(new EmailTemplateViewModel()
            {
                Guid = editEmailModel.Guid,
                MailBody = editEmailModel.MailBody,
                ModifiedBy = LoggedInUserId,
                ModifiedDate = DateTime.UtcNow,
                Subject=editEmailModel.Subject
            
                
            });

            if (EmailEdit == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Email template was not found.");
            }

            _emailTemplateProvider.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, EmailEdit);
        }

    }
}