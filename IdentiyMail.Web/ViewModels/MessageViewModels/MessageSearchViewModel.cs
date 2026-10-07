using IdentiyMail.Web.ViewModels.CategoryViewModels;

namespace IdentiyMail.Web.ViewModels.MessageViewModels
{
    public class MessageSearchViewModel
    {
        public string Query { get; set; } = string.Empty;

        public List<MessageSearchItemViewModel> Messages { get; set; } = new();
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

        public CategoryBadgeViewModel? Category { get; set; }
    }
}