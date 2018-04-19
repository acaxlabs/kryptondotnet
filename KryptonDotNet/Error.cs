using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KryptonDotNet
{
    public class Error
    {
        public string Message { get; set; }
        public object Content { get; set; }
        public string Type { get; set; } = "Error";

        public Error(string type, string msg, object content)
        {
            Type = type;
            Message = msg;
            Content = content;
        }

        public Error(Error error)
        {
            Type = error.Type;
            Message = error.Message;
            Content = error.Content;
        }
    }
}
