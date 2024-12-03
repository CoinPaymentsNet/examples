using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{

    public class Paged<T>
    {
        public T[] Items { get; set; }
    }

    public class PagedItemsDto<T>
    {
        /// <summary>
        /// current page of items
        /// </summary>
        public required T[] Items { get; set; }

        /// <summary>
        /// paging information
        /// </summary>
        public PagingDto? Paging { get; set; }
    }

    public sealed class PagingDto
    {
        /// <summary>
        /// the cursors for the current page
        /// </summary>
        public PagingCursorsDto? Cursors { get; set; }

        /// <summary>
        /// the limit that was specified, the query may have resulted in fewer items </summary>

        public int Limit { get; set; }

        /// <summary>
        /// the URI to the first page of results, if available
        /// </summary>
        public string? First { get; set; }

        /// <summary>
        /// the URI to the next page of results, if the next link doesn't exist then the current page is the last page
        /// of results
        /// </summary>
        /// <example>https://api.coinpayments.net/v1/...</example>
        public string? Next { get; set; }

        /// <summary>
        /// the URI to the previous page of results, if the previous link doesn't exist then the current page is the
        /// first page of results
        /// </summary>
        /// <example>https://api.coinpayments.net/v1/...</example>
        public string? Previous { get; set; }

        /// <summary>
        /// the URI to the last page of results, if available
        /// </summary>
        public string? Last { get; set; }
    }

    public sealed class PagingCursorsDto
    {
        /// <summary>
        /// cursor that points to the start of the page of data that has been returned
        /// </summary>
        public string? Before { get; set; }

        /// <summary>
        /// cursor that points to the end of the page of data that has been returned
        /// </summary>
        public string? After { get; set; }
    }
}
