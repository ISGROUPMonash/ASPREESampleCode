using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public class ApiResponseViewModel<T>
    {

        public int ResponseCode { get; set; }

        public string ResponseMessage { get; set; }

        public T ResponseData { get; set; }
    }

    public class ApiResponseViewModel
    {
        public Guid UserGuid { get; set; }
    }

    public enum ResponseMessageType
    {
        Success,
        Error,
        ValidationError,
        Redirect,
        Refresh
    }

    public interface IResponseMessage
    {
    }

    public class ResponseMessage<T> : IResponseMessage
    {
        public ResponseMessage(T data, ResponseMessageType messageType = ResponseMessageType.Success)
        {
            this.MessageType = messageType.ToString();
            this.Data = data;
        }

        public string MessageType { get; set; }
        public T Data { get; set; }
    }
}
