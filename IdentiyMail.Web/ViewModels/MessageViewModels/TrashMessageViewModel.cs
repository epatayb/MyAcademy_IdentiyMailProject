namespace IdentiyMail.Web.ViewModels.MessageViewModels
{
    public class TrashMessageViewModel
    {
        public int Id { get; set; }

        public string ContactFullName { get; set; } = string.Empty;

        public string ContactEmail { get; set; } = string.Empty;

        public string? ContactProfileImageUrl { get; set; }

        public string ContactInitials { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Preview { get; set; } = string.Empty;

        public DateTime SendDate { get; set; }

        public DateTime? DeletedAt { get; set; }

        // true = gelen mesaj - false = gönderilen mesaj
        public bool IsIncoming { get; set; }

        public bool IsRead { get; set; }
    }
}
