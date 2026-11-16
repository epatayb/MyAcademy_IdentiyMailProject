using IdentiyMail.Web.ViewModels.Common;

namespace IdentiyMail.Web.ViewModels.MessageViewModels
{
    public class DraftMessagesViewModel
    {
        public List<DraftMessageViewModel> Messages { get; set; } = new ();

        public MailPaginationViewModel Pagination { get; set; } = new ();
    }

    public class DraftMessageViewModel
    {
        public int Id { get; set; }

        public string ReceiverMail { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Preview { get; set; } = string.Empty;

        public DateTime UpdatedAt { get; set; }
    }
}
