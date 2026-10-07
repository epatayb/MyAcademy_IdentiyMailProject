using IdentiyMail.Web.DTOs.CategoryDtos;

namespace IdentiyMail.Web.ViewModels.CategoryViewModels
{
    public class CategoryQuickSelectorViewModel
    {
        public int MessageId { get; set; }

        public int? SelectedCategoryId { get; set; }

        public List<CategoryOptionDto> Categories { get; set; } = new();

        public string ReturnUrl { get; set; } = string.Empty;
    }
}
