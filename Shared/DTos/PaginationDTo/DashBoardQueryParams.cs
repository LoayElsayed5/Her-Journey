using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTos.PaginationDTo
{
    public class DashBoardQueryParams
    {
        private const int DefaultPageSize = 5;
        private const int MaxPageSize = 10;
        public BoardSortingOptions sort { get; set; }
        //public string? search { get; set; }
        public int pageNumber { get; set; } = 1;

        private int pageSize = DefaultPageSize;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize ? MaxPageSize : value; }
        }
    }
}
