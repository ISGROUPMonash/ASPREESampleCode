using log4net;
using log4net.Core;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Aspree.WebApi.ExceptionHander
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            string exceptionMessage = string.Empty;

            if (actionExecutedContext.Exception.InnerException == null)
            {
                exceptionMessage = actionExecutedContext.Exception.Message;
            }
            else
            {
                exceptionMessage = actionExecutedContext.Exception.InnerException.Message;
            }
            
            if (actionExecutedContext.Exception is Core.BadRequestException)
            {
                if (actionExecutedContext.Exception.Data.Contains("ValidationError"))
                {
                    var errorObject = new List<object>();

                    errorObject.Add(new
                    {
                        Key = actionExecutedContext.Exception.Data["ValidationError"],
                        Message = exceptionMessage
                    });

                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.BadRequest, errorObject);

                    return;
                }
                

                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Message = exceptionMessage
                });
               
                return;
            }

            if (actionExecutedContext.Exception is Core.NotFoundException)
            {

                if (actionExecutedContext.Exception.Data.Contains("ValidationError"))
                {
                    var errorObject = new List<object>();

                    errorObject.Add(new
                    {
                        Key = actionExecutedContext.Exception.Data["ValidationError"],
                        Message = exceptionMessage
                    });

                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.BadRequest, errorObject);

                    return;
                }

                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Message = exceptionMessage
                });
                return;
            }

            if (actionExecutedContext.Exception is Core.AlreadyExistsException)
            {
                if (actionExecutedContext.Exception.Data.Contains("ValidationError"))
                {
                    var errorObject = new List<object>();

                    errorObject.Add(new
                    {
                        Key = actionExecutedContext.Exception.Data["ValidationError"],
                        Message = exceptionMessage
                    });

                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.Ambiguous, errorObject);

                    return;
                }


                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.Ambiguous, new
                {
                    Message = exceptionMessage
                });

                return;
            }

        
            if (actionExecutedContext.Exception is Core.UnauthorizedException || ((System.Web.Http.HttpResponseException)actionExecutedContext.Exception).Response.StatusCode == HttpStatusCode.Unauthorized)
            {
                if (actionExecutedContext.Exception.Data.Contains("ValidationError"))
                {
                    var errorObject = new List<object>();

                    errorObject.Add(new
                    {
                        Key = actionExecutedContext.Exception.Data["ValidationError"],
                        Message = exceptionMessage
                    });

                    actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.Unauthorized, errorObject);

                    return;
                }


                actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    Message = exceptionMessage
                });

                return;
            }

            _logger.Error(exceptionMessage, actionExecutedContext.Exception);

#if DEBUG
            actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.InternalServerError, new
            {
                Message = Core.Exceptions.ExceptionResource.UnExpectedError,
                StackTrace = exceptionMessage + " " + actionExecutedContext.Exception.StackTrace
            });
#else
             actionExecutedContext.Response = actionExecutedContext.Request.CreateResponse(HttpStatusCode.InternalServerError, new
            {
                Message = Core.Exceptions.ExceptionResource.UnExpectedError
            });
#endif
        }
    }
}