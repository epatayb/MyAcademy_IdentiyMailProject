using IdentiyMail.Web.ViewModels.Common;

namespace IdentiyMail.Web.ViewModels.MessageViewModels
{
    public class ImportantMessagesViewModel
    {
        public List<InboxMessageViewModel> Messages { get; set; } = new();

        public MailPaginationViewModel Pagination { get; set; } = new();
    }
}
