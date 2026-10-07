using IdentiyMail.Web.DTOs.CategoryDtos;

namespace IdentiyMail.Web.ViewModels.CategoryViewModels
{
    public class CategoryManagementViewModel
    {
        public List<CategoryListItemDto> Categories { get; set; } = new();

        public CategoryFormViewModel Form { get; set; } = new();

        public int TotalCategorizedMessages => Categories.Sum(x => x.MessageCount);

        public List<CategoryDistributionItemViewModel> Distribution
        {
            get
            {
                if (TotalCategorizedMessages == 0)
                {
                    return new();
                }

                return Categories
                    .Where(x => x.MessageCount > 0)
                    .Select(x => new CategoryDistributionItemViewModel
                    {
                        Name = x.Name,
                        ColorHex = x.ColorHex,
                        MessageCount = x.MessageCount,
                        Percentage = Math.Round(x.MessageCount * 100d / TotalCategorizedMessages, 1)
                    })
                    .ToList();
            }
        }
    }
}
