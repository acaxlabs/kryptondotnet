﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace KryptonDotNet
{
    public static class KryptonApiController
    {
        /// <summary>
        /// Creates a KryptonDotNet.ErrorResult with a status code of InternalServerError
        /// </summary>
        /// <param name="controller">the current Web Api controller</param>
        /// <param name="ex">an exception, all the messages in the exception will be in the ReasonPhrase of the response and also in the response's content</param>
        /// <returns>returns a KryptonDotNet.ErrorResult</returns>
        public static ErrorResult ErrorResult(this ApiController controller, Exception ex)
        {
            return new ErrorResult(ex);
        }

        /// <summary>
        /// Creates a KryptonDotNet.ErrorResult
        /// </summary>
        /// <param name="controller">the current Web Api controller</param>
        /// <param name="statusCode">A HttpStatusCode, ie InternalServerError, BadRequest, Forbidden</param>
        /// <param name="message">the message you want displayed to client</param>
        /// <param name="content">any data to be handled on the client</param>
        /// <param name="ex">an exception, all the messages in the exception will be in the ReasonPhrase of the response</param>
        /// <returns>returns a KryptonDotNet.ErrorResult</returns>
        public static ErrorResult ErrorResult(this ApiController controller, HttpStatusCode statusCode, string message, object content, Exception ex)
        {
            return new ErrorResult(statusCode, message, content, ex);
        }

        /// <summary>
        /// Creates a KryptonDotNet.ErrorResult
        /// </summary>
        /// <param name="controller">the current Web Api controller</param>
        /// <param name="statusCode">A HttpStatusCode, ie InternalServerError, BadRequest, Forbidden</param>
        /// <param name="error">an Error object that contains the message and data to be seen by and handled on the client respectively</param>
        /// <param name="ex">an exception, all the messages in the exception will be in the ReasonPhrase of the response</param>
        /// <returns>returns a KryptonDotNet.ErrorResult</returns>
        public static ErrorResult ErrorResult(this ApiController controller, HttpStatusCode statusCode, Error error, Exception ex)
        {
            return new ErrorResult(statusCode, error, ex);
        }

        /// <summary>
        /// Creates a KryptonDotNet.FilteredResult
        /// requires -H krypton-filter-info
        /// </summary>
        /// <param name="controller">the current Web Api controller</param>
        /// <param name="items">the list of objects to filter</param>
        /// <returns>KryptonDotNet.FilteredResult</returns>
        public static FilteredResult Filter(this ApiController controller, IQueryable<object> items)
        {
            return new FilteredResult(items, controller.ActionContext);
        }

        /// <summary>
        /// Creates a KryptonDotNet.SortedResult
        /// requires -H krypton-sort
        /// </summary>
        /// <param name="controller">the current Web Api controller</param>
        /// <param name="items">the list of objects to sort</param>
        /// <returns>KryptonDotNet.SortedResult</returns>
        public static SortedResult Sort(this ApiController controller, IQueryable<object> items)
        {
            return new SortedResult(items, controller.ActionContext);
        }

        /// <summary>
        /// Creates a KryptonDotNet.PaginatedResult
        /// requires -H krypton-page, krypton-page-size, krypton-total
        /// </summary>
        /// <param name="controller">the current Web Api controller</param>
        /// <param name="items">the list of objects to page</param>
        /// <returns>KryptonDotNet.PaginatedResult</returns>
        public static PaginatedResult Paginate(this ApiController controller, IQueryable<object> items)
        {
            return new PaginatedResult(items, controller.ActionContext);
        }

        /// <summary>
        /// Creates a KryptonDotNet.KrptonListResult, encapsulates functionality from
        /// FilteredResult, SortedResult, and PaginatedResult
        /// uses -H krypton-page, krypton-page-size, krypton-total, krypton-sort, krypton-filter-info
        /// </summary>
        /// <param name="controller">the current Web Api controller</param>
        /// <param name="items">the list of objects to process</param>
        /// <returns>KryptonDotNet.KrptonListResult</returns>
        public static KryptonListResult KryptonResult(this ApiController controller, IQueryable<object> items)
        {
            try
            {
                var filterRes = new FilteredResult(items, controller.ActionContext);
                var sortRes = new SortedResult(filterRes.Items, controller.ActionContext);
                var pagedRes = new PaginatedResult(sortRes.Items, controller.ActionContext);
                return new KryptonListResult(pagedRes.Response);
            }
            catch (Exception ex)
            {
                //
                return new KryptonListResult(items, controller.ActionContext);
            }

        }
    }
}
