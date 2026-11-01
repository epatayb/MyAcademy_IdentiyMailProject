using IdentiyMail.Web.Models.Common;
using IdentiyMail.Web.ViewModels.MessageViewModels;

namespace IdentiyMail.Web.Services
{
    public interface IMessageQueryService
    {
        Task<PagedResult<MessageSearchItemViewModel>> SearchAsync(
            int userId, 
            string query,
            string scope,
            string status,
            int? categoryId,
            string sort,
            int page,
            int pageSize);
    }
}