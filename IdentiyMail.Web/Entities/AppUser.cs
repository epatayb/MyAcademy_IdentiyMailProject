using Microsoft.AspNetCore.Identity;

namespace IdentiyMail.Web.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }

        public List<UserMessage> SentMessages { get; set; } = new();
        public List<UserMessage> ReceivedMessages { get; set; } = new();
    }
}
