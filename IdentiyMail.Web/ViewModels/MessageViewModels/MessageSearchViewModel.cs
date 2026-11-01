using IdentiyMail.Web.ViewModels.CategoryViewModels;
using IdentiyMail.Web.DTOs.CategoryDtos;

namespace IdentiyMail.Web.ViewModels.MessageViewModels
{
    public class MessageSearchViewModel
    {
        public string Query { get; set; } = string.Empty;

        public string Scope { get; set; } = "all";

        public string Status { get; set; } = "all";

        public string Sort { get; set; } = "newest";

        public int? CategoryId { get; set; }

        public List<CategoryOptionDto> Categories { get; set; } = new();

        public List<MessageSearchItemViewModel> Messages { get; set; } = new();

        public bool HasSearchCriteria =>
            !string.IsNullOrWhiteSpace(Query) ||
            Scope != "all" ||
            Status != "all" ||
            CategoryId.HasValue;

        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalCount { get; set; }

        public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;

        public int FirstItemNumber => TotalCount == 0
            ? 0
            : ((CurrentPage - 1) * PageSize) + 1;

        public int LastItemNumber => Math.Min(CurrentPage * PageSize, TotalCount);
    }

    public class MessageSearchItemViewModel
    {
        public int Id { get; set; }

        public string ContactFullName { get; set; } = string.Empty;

        public string ContactEmail { get; set; } = string.Empty;

        public string? ContactProfileImageUrl { get; set; }

        public string ContactInitials { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Preview { get; set; } = string.Empty;

        public DateTime SendDate { get; set; }

        public bool IsIncoming { get; set; }

        public bool IsRead { get; set; }

        public CategoryBadgeViewModel? Category { get; set; }
    }
}