using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Results;
using XenonExtensions;

namespace KryptonDotNet
{
    /// <summary>
    ///  Represents a paged list of items as content and paging info in header values
    ///  : -H krypton-page, krypton-total, krypton-pageSize
    /// </summary>
    public class PaginatedResult : ResponseMessageResult
    {
        public IQueryable<object> Items { get; }

        public PaginatedResult(IQueryable<object> items, HttpActionContext actionContext)
            : base(new HttpResponseMessage(System.Net.HttpStatusCode.OK))
        {
            PageInfo pageInfo = HeaderUtil.ResolvePageInfoHeaders(actionContext.Request.Headers);

            pageInfo.Total = pageInfo.Total == 0 ? items.Count() : pageInfo.Total;
            this.Items = items.Skip((pageInfo.Page - 1) * pageInfo.PageSize).Take(pageInfo.PageSize);
            this.Response.Content = new ObjectContent(this.Items.GetType(), this.Items, actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);

            this.Response.Headers.Remove(HeaderValues.KRYPTON_PAGE);
            this.Response.Headers.Add(HeaderValues.KRYPTON_PAGE, pageInfo.Page.ToString());

            this.Response.Headers.Remove(HeaderValues.KRYPTON_TOTAL);
            this.Response.Headers.Add(HeaderValues.KRYPTON_TOTAL, pageInfo.Total.ToString());

            this.Response.Headers.Remove(HeaderValues.KRYPTON_PAGE_SIZE);
            this.Response.Headers.Add(HeaderValues.KRYPTON_PAGE_SIZE, pageInfo.PageSize.ToString());
        }

    }
    
    public class PageInfo
    {
        public int Total = 0;
        public int Page = 1;
        public int PageSize = 100;
    }
}
