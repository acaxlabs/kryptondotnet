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

        public Error(string msg, object content)
        {
            Message = msg;
            Content = content;
        }

        public Error(Error error)
        {
            Message = error.Message;
            Content = error.Content;
        }
    }
}
