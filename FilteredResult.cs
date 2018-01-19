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
    ///  : -H krypton-filter-info(json serialized array-[{ "key":key, "value":value },...] )
    /// </summary>
    public class FilteredResult : ResponseMessageResult
    {
        public IQueryable<object> Items { get; }

        public FilteredResult(IQueryable<object> items, HttpActionContext actionContext)
            : base(new HttpResponseMessage(System.Net.HttpStatusCode.OK))
        {
            var filters = HeaderUtil.ResolveFilterInfoHeader(actionContext.Request.Headers);
            if (filters != null)
            {
                StringBuilder filterClause = new StringBuilder(string.Empty);
                foreach (var item in filters.Properties())
                {
                    if (string.Compare(item.Value.ToString(), "all", true) == 0
                        || string.Compare(item.Value.ToString(), "any", true) == 0
                        || string.Compare(item.Value.ToString(), "*", true) == 0
                        || string.IsNullOrEmpty(item.Value.ToString())) continue;

                    if (!string.IsNullOrEmpty(filterClause.ToString()))
                        filterClause.Append(" and ");

                    filterClause.Append(BuildFilterClause(item.Path, item.Value.ToString()));
                }
                items = string.IsNullOrEmpty(filterClause.ToString()) ? items : items.Where(filterClause.ToString());
            }
            Items = items;
            this.Response.Content = new ObjectContent(Items.GetType(), Items, actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
        }

        private static string BuildFilterClause(string key, string value)
        {
            var term = key.ToString();
            return $"{term} = \"{value}\"";
        }
    }
}
