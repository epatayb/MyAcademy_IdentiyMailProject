using IdentiyMail.Web.DTOs.CategoryDtos;

namespace IdentiyMail.Web.ViewModels.CategoryViewModels
{
    public class CategorySelectorViewModel
    {
        public string InputName { get; set; } = "CategoryId";

        public int? SelectedCategoryId { get; set; }

        public List<CategoryOptionDto> Categories { get; set; } = new();

        public string Label { get; set; } = "Kategori";
    }
}
