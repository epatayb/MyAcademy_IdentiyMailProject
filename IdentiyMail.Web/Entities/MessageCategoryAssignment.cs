namespace IdentiyMail.Web.Entities
{
    public class MessageCategoryAssignment
    {
        public int Id { get; set; }

        public int MessageId { get; set; }

        public int UserId { get; set; }

        public UserMessage Message { get; set; } = null!;

        public AppUser User { get; set; } = null!;

        public int CategoryId { get; set; }

        public Category Category { get; set; } = null!;
    }
}
