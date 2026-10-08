using IdentiyMail.Web.ViewModels.MessageViewModels;

namespace IdentiyMail.Web.Services
{
    public interface IMessageQueryService
    {
        Task<List<MessageSearchItemViewModel>> SearchAsync(
            int userId, 
            string query,
            string scope,
            string status,
            int? categoryId,
            string sort);
    }
}