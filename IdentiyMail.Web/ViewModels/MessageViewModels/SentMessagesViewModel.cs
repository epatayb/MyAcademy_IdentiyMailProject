using IdentiyMail.Web.ViewModels.Common;

namespace IdentiyMail.Web.ViewModels.MessageViewModels
{
    public class SentMessagesViewModel
    {
        public List<SentMessageViewModel> Messages { get; set; } = new();

        public MailPaginationViewModel Pagination { get; set; } = new();
    }
}
