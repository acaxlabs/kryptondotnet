using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using XenonExtensions;

namespace KryptonDotNet
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            string message =  context.Exception.AllMessages().Replace(Environment.NewLine, " ");
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            response.ReasonPhrase = message;
            response.Content = new ObjectContent(typeof(Error), new Error("error", message, message), new JsonMediaTypeFormatter());
            context.Response = response; 
        }
    }
}