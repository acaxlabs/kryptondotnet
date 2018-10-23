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
using System.Web.Http;
using XenonExtensions;

namespace KryptonDotNet
{
    public class ErrorResult : ResponseMessageResult
    {
        public const string DEFAULT_ERROR_MESSAGE = "An error has occurred";
        public ErrorResult(HttpStatusCode statusCode, string type, string message, object content, HttpActionContext actionContext) : base(new HttpResponseMessage(statusCode))
        {
            message = string.IsNullOrEmpty(message) ? DEFAULT_ERROR_MESSAGE : message;
            this.Response.ReasonPhrase = DEFAULT_ERROR_MESSAGE;
            this.Response.Content = new ObjectContent(typeof(Error), new Error(type, message, content), actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
        }
        public ErrorResult(HttpStatusCode statusCode, string type, string message, object content, Exception ex, HttpActionContext actionContext) : base(new HttpResponseMessage(statusCode))
        {
            message = string.IsNullOrEmpty(message) ? GetMessageWithoutNewLines(ex) : message;
            this.Response.ReasonPhrase = GetMessageWithoutNewLines(ex);
            this.Response.Content = new ObjectContent(typeof(Error), new Error(type, message, content), actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
        }

        public ErrorResult(HttpStatusCode statusCode, Error error, Exception ex, HttpActionContext actionContext) : base(new HttpResponseMessage(statusCode))
        {
            error.Message = string.IsNullOrEmpty(error.Message) ? GetMessageWithoutNewLines(ex) : error.Message;
            this.Response.ReasonPhrase = GetMessageWithoutNewLines(ex);
            this.Response.Content = new ObjectContent(typeof(Error), error, actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
        }

        public ErrorResult(Exception ex, HttpActionContext actionContext) : base(new HttpResponseMessage(HttpStatusCode.InternalServerError))
        {
            string message = GetMessageWithoutNewLines(ex);
            this.Response.ReasonPhrase = message;
            this.Response.Content = new ObjectContent(typeof(Error), new Error("error", message, message), actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
        }

        private string GetMessageWithoutNewLines(Exception ex)
        {
            if (ex == null)
            {
                return string.Empty;
            }

            string msg = ex
                .AllMessages()
                .Replace(Environment.NewLine, " ")  // \r\n
                .Replace("\n", " ");

            return msg;
        }
    }

    

}
