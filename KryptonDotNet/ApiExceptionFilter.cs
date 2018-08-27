using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using XenonExtensions;

namespace KryptonDotNet
{
    public class ApiExceptionFilter : ExceptionFilterAttribute
    {
        private string DEFAULT_ERROR_MESSAGE= "";

        public override void OnException(HttpActionExecutedContext context)
        { 
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            if(CanReturnDetailedErrors())
            {
                Exception ex = context.Exception;
                string message = Regex.Replace(ex.AllMessages(), @"\r\n?|\n", "-");
                response.ReasonPhrase = message;
                response.Content = new ObjectContent(typeof(Error), new Error("error", message, ex), new JsonMediaTypeFormatter());
            }
            else
            {
                response.ReasonPhrase = DEFAULT_ERROR_MESSAGE;
                response.Content = new ObjectContent(typeof(Error), new Error("Error", DEFAULT_ERROR_MESSAGE, new { }), new JsonMediaTypeFormatter());
            }

            context.Response = response;
        }


        private bool CanReturnDetailedErrors()
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains("DisableDetailedErrorResults"))
            {
                return !bool.Parse(ConfigurationManager.AppSettings["DisableDetailedErrorResults"]);
            }
            else
            {
                return true;
            }
        }
    }
}