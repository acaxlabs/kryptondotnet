using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Results;

namespace KryptonDotNet
{
    public class FilteredResult : ResponseMessageResult
    {
        public FilteredResult(IQueryable<object> items, HttpActionContext actionContext) 
            : base(new HttpResponseMessage(System.Net.HttpStatusCode.OK))
        {
            var dict = HeaderUtil.ResolveFilterInfoHeader(actionContext.Request.Headers);
            if (dict != null)
            {
                foreach (var property in dict.Keys)
                {
                    var value = dict[property];
                    var filterClause = $"{property} = {value}";
                    items = items.Where(filterClause);
                }
            }
            
            this.Response.Content = new ObjectContent(items.GetType(), items, actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
        }
    }
    
}
