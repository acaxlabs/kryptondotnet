using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using XenonExtensions;

namespace KryptonDotNet
{
    internal class HeaderUtil
    {
        internal static PageInfo ResolvePageInfoHeaders(HttpRequestHeaders headers)
        {
            PageInfo pageInfo = new PageInfo();
            IEnumerable<string> values = new List<string>();
            pageInfo.Page = headers.TryGetValues("page", out values) ? int.Parse(values.FirstOrDefault()) : pageInfo.Page;
            pageInfo.Total = headers.TryGetValues("total", out values) ? int.Parse(values.FirstOrDefault()) : pageInfo.Total;
            pageInfo.PageSize = headers.TryGetValues("pageSize", out values) ? int.Parse(values.FirstOrDefault()) : pageInfo.PageSize;
            return pageInfo;
        }

        internal static string ResolveSortHeader(HttpRequestHeaders headers)
        {
            IEnumerable<string> values = new List<string>();
            headers.TryGetValues("krypton-sort", out values);
            return values?.FirstOrDefault() ?? null;
        }

        internal static Dictionary<string,string> ResolveFilterInfoHeader(HttpRequestHeaders headers)
        {
            IEnumerable<string> values = new List<string>();
            if (!headers.TryGetValues("krypton-filter-info", out values)) return null;

            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(values.FirstOrDefault());
            return dict;

        }

    }
}
