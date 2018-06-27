using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;

namespace KryptonDotNet
{
    /// <summary>
    /// Creates a KryptonDotNet.KrptonListResult, encapsulates functionality from
    /// FilteredResult, SortedResult, and PaginatedResult
    /// </summary>
    public class KryptonListResult<T> : ResponseMessageResult
    {
        public IQueryable<T> Items { get; }

        public KryptonListResult(HttpResponseMessage message)
            : base(message)  { }

        public KryptonListResult(IQueryable<T> items, HttpActionContext actionContext)
            :base(new HttpResponseMessage(System.Net.HttpStatusCode.OK))
        {
            var filterRes = new FilteredResult<T>(items, actionContext);
            var sortRes = new SortedResult<T>(filterRes.Items, actionContext);
            var pagedRes = new PaginatedResult<T>(sortRes.Items, actionContext);
            Items = pagedRes.Items;
            foreach (var item in pagedRes.Response.Headers)
            {
                this.Response.Headers.Add(item.Key, item.Value);
            }
            this.Response.Content = new ObjectContent(Items.GetType(), Items, actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
        }
    }


    
}
