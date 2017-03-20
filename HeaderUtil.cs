using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using XenonExtensions;

namespace KryptonDotNet
{
    public class HeaderValues
    {
        public static string KRYPTON_PAGE = "krypton-page";
        public static string KRYPTON_TOTAL = "krypton-total";
        public static string KRYPTON_PAGE_SIZE = "krypton-page-size";
        public static string KRYPTON_SORT = "krypton-sort";
        public static string KRYPTON_FILTER_INFO = "krypton-filter-info";
    }
    internal class HeaderUtil
    {
        internal static PageInfo ResolvePageInfoHeaders(HttpRequestHeaders headers)
        {
            PageInfo pageInfo = new PageInfo();
            IEnumerable<string> values = Enumerable.Empty<string>();
            pageInfo.Page = headers.TryGetValues(HeaderValues.KRYPTON_PAGE, out values) ? int.Parse(values.FirstOrDefault()) : pageInfo.Page;
            pageInfo.Total = headers.TryGetValues(HeaderValues.KRYPTON_TOTAL, out values) ? int.Parse(values.FirstOrDefault()) : pageInfo.Total;
            pageInfo.PageSize = headers.TryGetValues(HeaderValues.KRYPTON_PAGE_SIZE, out values) ? int.Parse(values.FirstOrDefault()) : pageInfo.PageSize;
            return pageInfo;
        }
        
        internal static string ResolveSortHeader(HttpRequestHeaders headers)
        {
            IEnumerable<string> values = Enumerable.Empty<string>();
            headers.TryGetValues(HeaderValues.KRYPTON_SORT, out values);
            return values?.FirstOrDefault() ?? null;
        }

        internal static JObject ResolveFilterInfoHeader(HttpRequestHeaders headers)
        {
            IEnumerable<string> values = Enumerable.Empty<string>();
            if (!headers.TryGetValues(HeaderValues.KRYPTON_FILTER_INFO, out values)) return null;
            return JsonConvert.DeserializeObject<JObject>(values.FirstOrDefault());

        }


    }
}
