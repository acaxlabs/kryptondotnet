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
    /// <summary>
    ///  Represents a sorted list of items as content and sort info in header value
    ///  : -H krypton-sort
    /// </summary>
    public class SortedResult<T> : ResponseMessageResult
    {
        public IQueryable<T> Items { get; }
        public SortedResult(IQueryable<T> items, HttpActionContext actionContext) 
            : base(new HttpResponseMessage(System.Net.HttpStatusCode.OK))
        {
            var sort = HeaderUtil.ResolveSortHeader(actionContext.Request.Headers);
            if (!string.IsNullOrEmpty(sort))
            {
                var sortClause = sort.Replace("+","").Replace("-", " DESC");
                items = items.OrderBy(sortClause);
            }
            this.Items = items;
            this.Response.Content = new ObjectContent(Items.GetType(), Items, actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
        }
    }
}
