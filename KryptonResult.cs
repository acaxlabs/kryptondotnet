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
   
    
    public class KryptonListResult : ResponseMessageResult
    {
        public KryptonListResult(IQueryable<object> items, HttpActionContext actionContext)
            : base(new HttpResponseMessage(System.Net.HttpStatusCode.OK))
        {
            var filterRes = new FilteredResult(items, actionContext);
            items = filterRes.Response.Content.ReadAsAsync<IQueryable<object>>().Result;
            var sortRes = new SortedResult(items, actionContext);
            items = sortRes.Response.Content.ReadAsAsync<IQueryable<object>>().Result;
            var pagedRes = new PaginatedResult(items, actionContext);
            this.Response.Content = pagedRes.Response.Content;

        }
    }
}
