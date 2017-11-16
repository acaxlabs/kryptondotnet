using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using XenonExtensions;

namespace KryptonDotNet
{
    public class ErrorResult : ResponseMessageResult
    {
        public ErrorResult(HttpStatusCode statusCode, string type, string message, object content, Exception ex) : base(new HttpResponseMessage(statusCode))
        {
            message = string.IsNullOrEmpty(message) ? ex.AllMessages().Replace(Environment.NewLine, " ") : message;
            this.Response.ReasonPhrase = ex.AllMessages().Replace(Environment.NewLine, " ");
            this.Response.Content = new ObjectContent(typeof(Error), new Error(type, message, content), new JsonMediaTypeFormatter());
        }

        public ErrorResult(HttpStatusCode statusCode, Error error, Exception ex) : base(new HttpResponseMessage(statusCode))
        {
            error.Message = string.IsNullOrEmpty(error.Message) ? ex.AllMessages().Replace(Environment.NewLine, " ") : error.Message;
            this.Response.ReasonPhrase = ex.AllMessages().Replace(Environment.NewLine, " ");
            this.Response.Content = new ObjectContent(typeof(Error), error, new JsonMediaTypeFormatter());
        }

        public ErrorResult(Exception ex) : base(new HttpResponseMessage(HttpStatusCode.InternalServerError))
        {
            string message = ex.AllMessages().Replace(Environment.NewLine, " ");
            this.Response.ReasonPhrase = message;
            this.Response.Content = new ObjectContent(typeof(Error), new Error("error", message, message), new JsonMediaTypeFormatter());
        }
    }

    

}
