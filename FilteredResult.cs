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
    /// <summary>
    ///  Represents a filtered list of items as content and filter info in header values
    ///  : -h krypton-filter-info(json serialized array-[{ "key":key, "value":value },...] )
    /// </summary>
    public class FilteredResult : ResponseMessageResult
    {
        public IQueryable<object> Items { get; }

        public FilteredResult(IQueryable<object> items, HttpActionContext actionContext)
            : base(new HttpResponseMessage(System.Net.HttpStatusCode.OK))
        {
            var filters = HeaderUtil.ResolveFilterInfoHeader(actionContext.Request.Headers);
            filters = filters.Where(f => !string.IsNullOrEmpty(f.Value) && string.Compare(f.Value, "all", true) != 0).ToArray();

            if (filters != null && filters.Length != 0)
            {
                StringBuilder filterClause = new StringBuilder(string.Empty);
                for (int i = 0; i < filters.Length; i++)
                {
                    var filter = filters[i];
                    filterClause.Append(filter.ToString());
                    if (i == filters.Length - 1) continue;
                    filterClause.Append(" and ");
                }
                items = items.Where(filterClause.ToString());

            }
            Items = items;
            this.Response.Content = new ObjectContent(Items.GetType(), items, actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
        }

    }
    internal class FilterInfo
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public override string ToString()
        {
            return $"{Key}.ToString() = \"{Value}\"";
        }
    }
}
