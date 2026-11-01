namespace IdentiyMail.Web.Models.Common
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();

        public int TotalCount { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalPages => TotalCount == 0
            ? 0
            : (int)Math.Ceiling(TotalCount / (double)PageSize);

        public bool HasPreviousPage => Page > 1;

        public bool HasNextPage => Page < TotalPages;
    }
}