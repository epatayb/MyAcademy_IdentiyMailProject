namespace IdentiyMail.Web.ViewModels.MessageViewModels
{
    public class InboxMessageViewModel
    {
        public int Id { get; set; }

        public string SenderFullName { get; set; } = string.Empty;

        public string SenderEmail { get; set; } = string.Empty;

        public string? SenderProfileImageUrl { get; set; } = string.Empty;

        public string SenderInitials { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Preview { get; set; } = string.Empty;

        public DateTime SendDate { get; set; }

        public bool IsRead { get; set; }

        public bool IsImportant { get; set; }
    }
}
