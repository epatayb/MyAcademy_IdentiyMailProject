namespace IdentiyMail.Web.ViewModels.MessageViewModels
{
    public class CategoryMessagesViewModel
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string ColorHex { get; set; } = string.Empty;

        public List<CategoryMessageItemViewModel> Messages { get; set; } = new();
    }

    public class CategoryMessageItemViewModel
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
    }
}
