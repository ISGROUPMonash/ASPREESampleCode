﻿using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;

namespace Aspree.WebApi.ExceptionHander
{
    public class GlobalExceptionHandler : ExceptionHandler
    {
        public async override Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            // Access Exception using context.Exception;  
            const string errorMessage = "An unexpected error occured";
            var response = context.Request.CreateResponse(HttpStatusCode.InternalServerError,
                new
                {  
                    Message = errorMessage
                });
            response.Headers.Add("X-Error", errorMessage);
            context.Result = new ResponseMessageResult(response);
        }
    }
}