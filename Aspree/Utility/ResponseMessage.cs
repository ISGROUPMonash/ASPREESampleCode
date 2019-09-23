using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Aspree.Utility
{
    /// <summary>
    /// Response common data model
    /// </summary>
    public class ResponseMessage
    {
        public string MessageType { get; set; }
        public string Content { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}