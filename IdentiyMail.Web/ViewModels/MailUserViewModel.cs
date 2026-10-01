namespace IdentiyMail.Web.ViewModels
{
    public class MailUserViewModel
    {
        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? ProfileImageUrl { get; set; }

        public string Initials { get; set; } = string.Empty;
    }
}
