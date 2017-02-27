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
        public static PaginatedResult Paginate(this ApiController controller, IQueryable<object> items)
        {
            return new PaginatedResult(items, controller.ActionContext);
        }
    }
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

    public class PaginationAttribute : ActionFilterAttribute
    {
        PageInfo PageInfo { get; set; }


        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            base.OnActionExecuting(actionContext);
            var query = actionContext.Request.RequestUri.ParseQueryString();
            PageInfo pageInfo = new PageInfo();
            pageInfo.Page.TryParseExtended(query["page"], pageInfo.Page);
            pageInfo.Total.TryParseExtended(query["total"], pageInfo.Total);
            pageInfo.PageSize.TryParseExtended(query["pageSize"], pageInfo.PageSize);
            this.PageInfo = pageInfo;
        }
        public override async void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
            var formatter = actionExecutedContext.ActionContext.ControllerContext.Configuration.Formatters.JsonFormatter;
            //var content = await actionExecutedContext.Response.Content.ReadAsStringAsync();
            var contentStream = await actionExecutedContext.Response.Content.ReadAsStreamAsync();
            var items = (IList<object>)new JsonSerializer().Deserialize(new StreamReader(contentStream), typeof(IList<object>));
            this.PageInfo.Total = this.PageInfo.Total == 0 ? items.Count() : this.PageInfo.Total;
            this.PageInfo.Items = items.Skip((this.PageInfo.Page - 1) * this.PageInfo.PageSize).Take(this.PageInfo.PageSize).ToList();
            actionExecutedContext.Response.Content = new ObjectContent(this.PageInfo.GetType(), this.PageInfo, formatter);
        }


    }


    public class PageInfo
    {
        public int Total = 0;
        public int Page = 1;
        public int PageSize = 100;
        public List<object> Items { get; set; }

        public PageInfo() { }
        public PageInfo(int page, int total, int pageSize)
        {
            Page = page == 0 ? 1 : page;
            Total = total;
            PageSize = pageSize == 0 ? 100 : pageSize;
        }
    }
}
