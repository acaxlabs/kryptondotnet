using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace KryptonDotNet
{
    public static class KryptonApiController
    {
        /// <summary>
        /// Creates a KryptonDotNet.FilteredResult
        /// </summary>
        /// <param name="controller">the current Web Api controller</param>
        /// <param name="items">the list of objects to page</param>
        /// <returns>KryptonDotNet.FilteredResult</returns>
        public static FilteredResult Filter(this ApiController controller, IQueryable<object> items)
        {
            return new FilteredResult(items, controller.ActionContext);
        }
        /// <summary>
        /// Creates a KryptonDotNet.SortedResult
        /// </summary>
        /// <param name="controller">the current Web Api controller</param>
        /// <param name="items">the list of objects to page</param>
        /// <returns>KryptonDotNet.SortedResult</returns>
        public static SortedResult Sort(this ApiController controller, IQueryable<object> items)
        {
            return new SortedResult(items, controller.ActionContext);
        }

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

        /// <summary>
        /// Creates a KryptonDotNet.KrptonListResult
        /// </summary>
        /// <param name="controller">the current Web Api controller</param>
        /// <param name="items">the list of objects to page</param>
        /// <returns>KryptonDotNet.KrptonListResult</returns>
        public static KryptonListResult KryptonResult(this ApiController controller, IQueryable<object> items)
        {
            return new KryptonListResult(items, controller.ActionContext);
        }
    }
}
