using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Results;

namespace KryptonDotNet
{
    public class SortedResult : ResponseMessageResult
    {
        public SortedResult(IQueryable<object> items, HttpActionContext actionContext) 
            : base(new HttpResponseMessage(System.Net.HttpStatusCode.OK))
        {
            var sort = HeaderUtil.ResolveSortHeader(actionContext.Request.Headers);
            if (!string.IsNullOrEmpty(sort))
            {
                var sortby = Regex.Replace(sort, "[-+]", "");
                var sortClause = sort.StartsWith("-") ? $"{sortby} DESC" : sortby;
                items = items.OrderBy(sortClause);
            }
            this.Response.Content = new ObjectContent(items.GetType(), items, actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
        }
    }
}
