using IdentiyMail.Web.ViewModels.CategoryViewModels;

namespace IdentiyMail.Web.ViewModels.MessageViewModels
{
    public class SentMessageViewModel
    {
        public int Id { get; set; }

        public string ReceiverFullName { get; set; } = string.Empty;

        public string ReceiverEmail { get; set; } = string.Empty;

        public string? ReceiverProfileImageUrl { get; set; }

        public string ReceiverInitials { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Preview { get; set; } = string.Empty;

        public DateTime SendDate { get; set; }

        public bool IsRead { get; set; }

        public CategoryBadgeViewModel? Category { get; set; }        
    }
}
