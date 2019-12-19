using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aspree.Core.ViewModels
{
    public enum MessageTypes
    {
        Success,
        Error,
        ValidationErrors,
        Refresh,
        Redirect,
        Modal,
        Unauthorized,
        NotFound
    }
    public class MessageContainer<T>
    {
        public MessageContainer(T data, string message = "", MessageTypes messageType = MessageTypes.Success)
        {
            this.Content = data;
            this.Message = message;
            this.MessageType = messageType.ToString();
        }

        public string Message { get; set; }
        public string MessageType { get; set; }
        public T Content { get; set; }
    }
}
