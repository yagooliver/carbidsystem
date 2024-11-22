using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarBidSystem.Common.Response
{
    public class PagedResponse<T>(T data, int pageNumber, int pageSize, int totalRecords)
    {
        public T Data { get; set; } = data;
        public int PageNumber { get; set; } = pageNumber;
        public int PageSize { get; set; } = pageSize;
        public int TotalRecords { get; set; } = totalRecords;
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    }
}
