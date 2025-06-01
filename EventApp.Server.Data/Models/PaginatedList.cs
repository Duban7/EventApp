using Microsoft.Identity.Client;

namespace Data.Models
{
    public class PaginatedList<T>
    {
        public List<T>? items { get;}
        public int? pageIndex { get;}
        public int? TotalPages { get; }
        public bool HasPreviousPage => pageIndex > 1;
        public bool HasNextPage => pageIndex < TotalPages;

        public PaginatedList(List<T>? items, int? pageIndex, int? totalPages)
        {
            this.items = items;
            this.pageIndex = pageIndex;
            TotalPages = totalPages;
        }
    }
}
