namespace IdentiyMail.Web.Entities
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string ColorHex { get; set; } = "#1d4ed8";

        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }

        public AppUser User { get; set; } = null!;

        public List<MessageCategoryAssignment> Assignments { get; set; } = new();

        public List<DraftMessage> DraftMessages { get; set; } = new();
    }
}
