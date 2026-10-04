namespace IdentiyMail.Web.Entities
{
    public class UserMessage
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime SendDate { get; set; }
        public bool IsRead { get; set; }
        public bool IsImportant { get; set; }

        public AppUser Sender { get; set; } = null!;
        public int SenderId { get; set; }
        public AppUser Receiver { get; set; } = null!;
        public int ReceiverId { get; set; }

        public bool IsDeletedBySender { get; set; }
        public DateTime? DeletedBySenderAt { get; set; }
        public bool IsPermanetlyDeletedBySender { get; set; }

        public bool IsDeletedByReceiver { get; set; }
        public DateTime? DeletedByReceiverAt { get; set; }
        public bool IsPermanetlyDeletedByReceiver { get; set; }
    }
}
