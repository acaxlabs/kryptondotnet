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
using Newtonsoft.Json.Linq;

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
            Items = FilteredResultHelpers.ProcessFilters<T>(items, filters);
            this.Response.Content = new ObjectContent(Items.GetType(), Items, actionContext.ControllerContext.Configuration.Formatters.JsonFormatter);
        }

    }

    public static class FilteredResultHelpers
    {
        public static IQueryable<T> ProcessFilters<T>(IQueryable<T> items, JObject filters)
        {
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

                    filterClause.Append(ProcessPropertyIntoFilterClause<T>(item.Name, item.Value));
                }

                return string.IsNullOrEmpty(filterClause.ToString()) ? items : items.Where(filterClause.ToString());
            }
            else
            {
                return items;
            }
        }


        private static string ProcessPropertyIntoFilterClause<T>(string propName, JToken propValue)
        {
            Type type = typeof(T);

            string postfix = null;

            if (PostFixOp("_before", ref propName)) postfix = "_before";
            else if (PostFixOp("_after", ref propName)) postfix = "_after";


            PropertyInfo propertyInfo = type.GetProperty(propName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propertyInfo is null) return "";

            return BuildFilterClause(propName, propValue.ToString(), propertyInfo.PropertyType, postfix);
        }

        /** This builds out the dyanmic linq query to filter for this property
         * ex. 
         * itemPath:        Contacts.status,
         * value:           Processing,
         * propertyType:    String
         */
        private static string BuildFilterClause(string itemPath, string value, Type propertyType, string postfix)
        {

            if (propertyType.IsEnum)
            {
                return $"{itemPath} = \"{value}\"";
            }
            else if (propertyType.FullName == typeof(DateTime).FullName && postfix != null)
            {

                var comparer = "=";
                if (postfix == "_before") comparer = "<=";
                else if (postfix == "_after") comparer = ">=";

                //normalize passed in date value to 12am
                var date = new DateTime(long.Parse(value)).Date;
                value = new DateTime(long.Parse(value)).Date.ToShortDateString();

                if (postfix == "_before") date = date.AddDays(1);

                //Date to get the date set to 12am, Ticks becuase we are comparing utc ticks
                return $"{itemPath} {comparer} DateTime({date.Year}, {date.Month}, {date.Day})";
            }

            return $"{itemPath}.toString() = \"{value}\"";
        }


        private static bool PostFixOp(string postfix, ref string name)
        {
            if (CheckPostfix(name, postfix))
            {
                name = ClipPostfix(name, postfix);
                return true;
            }
            else
            {
                return false;
            }
        }
        private static bool CheckPostfix(string toCheck, string postfix)
        {
            return toCheck.Length > postfix.Length && toCheck.Substring(toCheck.Length - postfix.Length).ToLower() == postfix;
        }
        private static string ClipPostfix(string toClip, string postfix)
        {
            if (CheckPostfix(toClip, postfix))
            {
                return toClip.Substring(0, toClip.Length - postfix.Length);
            }
            else
            {
                return toClip;
            }
        }
    }
}
