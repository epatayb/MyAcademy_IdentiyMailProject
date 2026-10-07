using IdentiyMail.Web.DTOs.CategoryDtos;
using IdentiyMail.Web.Entities;

namespace IdentiyMail.Web.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryOptionDto>> GetOptionsAsync(int userId);

        Task<List<CategoryListItemDto>> GetCategoriesAsync(int userId);

        Task<int?> GetMessageCategoryIdAsync(int userId, int messageId);

        Task<CategoryServiceResult> CreateAsync(int userId, string name, string? description, string colorHex);

        Task<CategoryServiceResult> UpdateAsync(int userId, int  categoryId, string name, string? description, string colorHex);

        Task<CategoryServiceResult> DeleteAsync(int userId, int categoryId);

        Task<CategoryServiceResult> AssignToMessageAsync(int userId, int messageId, int? categoryId);

        Task<bool> IsOwnedByUserAsync(int userId, int categoryId);
    }
}
