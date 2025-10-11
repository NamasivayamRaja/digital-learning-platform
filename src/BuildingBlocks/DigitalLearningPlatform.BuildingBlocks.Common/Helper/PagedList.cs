using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalLearningPlatform.BuildingBlocks.Common.Helper
{
    public class PagedList<T> : List<T>
    {
        public PagedList(IEnumerable<T> items, int currentPage, int pageSize, int totalCount) 
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling(totalCount / (decimal)PageSize);
            AddRange(items);
        }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
