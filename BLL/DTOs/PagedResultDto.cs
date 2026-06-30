using System.Collections.Generic;

namespace BLL.DTOs
{
    /// <summary>
    /// Lớp chứa kết quả phân trang chuẩn cho các API danh sách.
    /// </summary>
    /// <typeparam name="T">Kiểu phần tử trong danh sách.</typeparam>
    public class PagedResultDto<T>
    {
        /// <summary>Tổng số bản ghi thỏa điều kiện.</summary>
        public int TotalCount { get; set; }
        /// <summary>Danh sách phần tử của trang hiện tại.</summary>
        public IEnumerable<T> Items { get; set; } = new List<T>();

        public PagedResultDto(int totalCount, IEnumerable<T> items)
        {
            TotalCount = totalCount;
            Items = items;
        }
    }
}