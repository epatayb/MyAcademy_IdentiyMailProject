namespace IdentiyMail.Web.ViewModels.Common
{
    public class MailPaginationViewModel
    {
        public string Controller { get; set; } = "Message";

        public string Action { get; set; } = string.Empty;

        public Dictionary<string, string> RouteValues { get; set; } = new();

        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalCount { get; set; }

        public int TotalPages =>
            TotalCount == 0 || PageSize <= 0 
                ? 0
                : (int)Math.Ceiling(TotalCount / (double)PageSize);

        public bool HasPreviousPage =>
            CurrentPage > 1;

        public bool HasNextPage =>
            CurrentPage < TotalPages;

        public int FirstItemNumber => 
            TotalCount == 0
                ? 0
                : ((CurrentPage - 1) * PageSize) + 1;

        public int LastItemNumber =>
            Math.Min(CurrentPage * PageSize, TotalCount);
    }
}
