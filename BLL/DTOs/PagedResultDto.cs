using System.Collections.Generic;

namespace BLL.DTOs
{
    public class PagedResultDto<T>
    {
        // Executes total count operation.
        public int TotalCount { get; set; }
        // Executes items operation.
        public IEnumerable<T> Items { get; set; } = new List<T>();

        // Initializes a new instance of PagedResultDto with dependencies: totalCount, items.
        // Assigns injected service and configuration instances to readonly fields for runtime operations.
        public PagedResultDto(int totalCount, IEnumerable<T> items)
        {
            TotalCount = totalCount;
            Items = items;
        }
    }
}
