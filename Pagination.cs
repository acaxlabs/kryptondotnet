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
    public static class PaginationApiController
    {
        /// <summary>
        /// Creates a KryptonDotNet.PaginatedResult
        /// </summary>
        /// <param name="controller">the current Web Api controller</param>
        /// <param name="items">the list of objects to page</param>
        /// <returns>KryptonDotNet.PaginatedResult</returns>
        public static PaginatedResult Paginate(this ApiController controller, IQueryable<object> items)
        {
            return new PaginatedResult(items, controller.ActionContext);
        }
    }
    /// <summary>
    ///  Represents a paged list of items as content and paging info in header values
    ///  : -h krypton-page, -h krypton-total, -h krypton-pageSize
    /// </summary>
    public class PaginatedResult : ResponseMessageResult
    {
        public PaginatedResult(IQueryable<object> items, HttpActionContext actionContext)
            : base(new HttpResponseMessage(System.Net.HttpStatusCode.OK))
        {
            var query = actionContext.Request.RequestUri.ParseQueryString();
            PageInfo pageInfo = new PageInfo();
            pageInfo.Page = pageInfo.Page.TryParseExtended( query["page"], pageInfo.Page);
            pageInfo.Total = pageInfo.Total.TryParseExtended(query["total"], pageInfo.Total);
            pageInfo.PageSize = pageInfo.PageSize.TryParseExtended(query["pageSize"], pageInfo.PageSize);
            pageInfo.Total = pageInfo.Total == 0 ? items.Count() : pageInfo.Total;
            pageInfo.Items = items.Skip((pageInfo.Page - 1) * pageInfo.PageSize).Take(pageInfo.PageSize).ToList();
            this.Response.Content = new ObjectContent(pageInfo.Items.GetType(), pageInfo.Items, actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);

            this.Response.Headers.Remove("krypton-page");
            this.Response.Headers.Add("krypton-page", pageInfo.Page.ToString());

            this.Response.Headers.Remove("krypton-total");
            this.Response.Headers.Add("krypton-total", pageInfo.Total.ToString());

            this.Response.Headers.Remove("krypton-pageSize");
            this.Response.Headers.Add("krypton-pageSize", pageInfo.PageSize.ToString());
        }

    }
    
    public class PageInfo
    {
        public int Total = 0;
        public int Page = 1;
        public int PageSize = 100;
        public List<object> Items { get; set; }
    }
}
