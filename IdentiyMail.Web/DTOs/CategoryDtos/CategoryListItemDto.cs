namespace IdentiyMail.Web.DTOs.CategoryDtos
{
    public class CategoryListItemDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string ColorHex { get; set; } = string.Empty;

        public int MessageCount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
