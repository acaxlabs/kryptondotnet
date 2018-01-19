using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net.Http;
using System.Reflection;
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
    public class FilteredResult<T> : ResponseMessageResult
    {
        public IQueryable<T> Items { get; }

        public FilteredResult(IQueryable<T> items, HttpActionContext actionContext)
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
                    Type type = typeof(T);
                    PropertyInfo propertyInfo = type.GetProperty(item.Name, BindingFlags.IgnoreCase| BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo is null) continue;
                    filterClause.Append(BuildFilterClause(item.Path, item.Value.ToString(), propertyInfo));
                }
                items = string.IsNullOrEmpty(filterClause.ToString()) ? items : items.Where(filterClause.ToString());
            }
            Items = items;
            this.Response.Content = new ObjectContent(Items.GetType(), Items, actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
        }

        private static string BuildFilterClause(string key, string value, PropertyInfo propertyInfo)
        {
            Type propertyType = propertyInfo.PropertyType;
            return propertyType.IsEnum ? $"{key} = \"{value}\"" : $"{key}.ToString() = \"{value}\"";
        }
    }
}
