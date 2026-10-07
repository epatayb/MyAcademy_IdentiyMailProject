using IdentiyMail.Web.DTOs.CategoryDtos;

namespace IdentiyMail.Web.ViewModels.CategoryViewModels
{
    public class CategoryManagementViewModel
    {
        public List<CategoryListItemDto> Categories { get; set; } = new();

        public CategoryFormViewModel Form { get; set; } = new();

        public int TotalCategorizedMessages => Categories.Sum(x => x.MessageCount);
    }
}
