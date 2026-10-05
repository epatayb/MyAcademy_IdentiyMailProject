namespace IdentiyMail.Web.Entities
{
    public class DraftMessage
    {
        public int Id { get; set; }

        public string? ReceiverMail { get; set; }

        public string? Subject { get; set; }

        public string? Body { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public AppUser Sender { get; set; } = null!;

        public int SenderId { get; set; }
    }
}
