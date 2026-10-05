namespace IdentiyMail.Web.ViewModels.MessageViewModels
{
    public class MessageDetailViewModel
    {
        public int Id { get; set; }

        public string ContactFullName { get; set; } = string.Empty;

        public string ContactEmail { get; set; } = string.Empty;

        public string? ContactProfileImageUrl { get; set; }

        public string ContactInitials { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;

        public DateTime SendDate { get; set; }

        public bool IsIncoming { get; set; }

        public bool IsRead { get; set; }

        public bool IsImportant { get; set; }
    }
}
