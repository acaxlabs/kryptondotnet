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
using System.Configuration;

namespace KryptonDotNet
{
    public class ErrorResult : ResponseMessageResult
    {
        public const string DEFAULT_ERROR_MESSAGE = "An error has occurred";
        public ErrorResult(HttpStatusCode statusCode, string type, string message, object content, HttpActionContext actionContext) : base(new HttpResponseMessage(statusCode))
        {
            if(CanReturnDetailedErrors())
            {
                message = string.IsNullOrEmpty(message) ? DEFAULT_ERROR_MESSAGE : message;
                this.Response.ReasonPhrase = DEFAULT_ERROR_MESSAGE;
                this.Response.Content = new ObjectContent(typeof(Error), new Error(type, message, content), actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
            }
            else
            {
                SetupUndetailedResult(actionContext);
            }
        }
        public ErrorResult(HttpStatusCode statusCode, string type, string message, object content, Exception ex, HttpActionContext actionContext) : base(new HttpResponseMessage(statusCode))
        {
            if(CanReturnDetailedErrors())
            {
                message = string.IsNullOrEmpty(message) ? ex.AllMessages().Replace(Environment.NewLine, " ") : message;
                this.Response.ReasonPhrase = ex.AllMessages().Replace(Environment.NewLine, " ");
                this.Response.Content = new ObjectContent(typeof(Error), new Error(type, message, content), actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
            }
            else
            {
                SetupUndetailedResult(actionContext);
            }
        }

        public ErrorResult(HttpStatusCode statusCode, Error error, Exception ex, HttpActionContext actionContext) : base(new HttpResponseMessage(statusCode))
        {
            if(CanReturnDetailedErrors())
            {
                error.Message = string.IsNullOrEmpty(error.Message) ? ex.AllMessages().Replace(Environment.NewLine, " ") : error.Message;
                this.Response.ReasonPhrase = ex?.AllMessages().Replace(Environment.NewLine, " ") ?? "";
                this.Response.Content = new ObjectContent(typeof(Error), error, actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
            }
            else
            {
                SetupUndetailedResult(actionContext);
            }
        }

        public ErrorResult(Exception ex, HttpActionContext actionContext) : base(new HttpResponseMessage(HttpStatusCode.InternalServerError))
        {
            if (CanReturnDetailedErrors())
            {
                string message = ex.AllMessages().Replace(Environment.NewLine, " ");
                this.Response.ReasonPhrase = message;
                this.Response.Content = new ObjectContent(typeof(Error), new Error("error", message, message), actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
            }
            else
            {
                SetupUndetailedResult(actionContext);
            }
        }

        private bool CanReturnDetailedErrors()
        {
            if(ConfigurationManager.AppSettings.AllKeys.Contains("DisableDetailedErrorResults"))
            {
                return !bool.Parse( ConfigurationManager.AppSettings["DisableDetailedErrorResults"]);
            }
            else
            {
                return true;
            }
        }
        private void SetupUndetailedResult(HttpActionContext actionContext)
        {
            var message = DEFAULT_ERROR_MESSAGE;
            this.Response.ReasonPhrase = DEFAULT_ERROR_MESSAGE + " and you have disabled detailed error results. Check your Web.Config for the setting DisableDetailedErrorResults.";
            this.Response.Content = new ObjectContent(typeof(Error), new Error("error", message, new { }), actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
        }

    }

    

}
