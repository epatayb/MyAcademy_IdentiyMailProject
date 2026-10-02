namespace IdentiyMail.Web.ViewModels.MessageViewModels
{
    public class InboxViewModel
    {
        public List<InboxMessageViewModel> Messages { get; set; } = new();

        public int TotalCount { get; set; }

        public int UnreadCount { get; set; }

        public string Status { get; set; } = "all";
    }
}
