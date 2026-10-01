using IdentiyMail.Web.Entities;
using IdentiyMail.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentiyMail.Web.ViewComponents
{
    public class MailUserViewComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;

        public MailUserViewComponent(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            #region Giris yapan kullanıcı bilgilerini alma

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return Content(string.Empty);
            }
            #endregion

            #region Topbar kullanıcı modelini hazırlama

            var firstNameInitial = string.IsNullOrWhiteSpace(user.FirstName)
                ? string.Empty : user.FirstName[..1].ToUpper();

            var lastNameInitial = string.IsNullOrWhiteSpace(user.LastName)
                ? string.Empty : user.LastName[..1].ToUpper();

            var model = new MailUserViewModel
            {
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email ?? string.Empty,
                ProfileImageUrl = user.ProfileImageUrl,
                Initials = $"{firstNameInitial}{lastNameInitial}"
            };
            #endregion

            return View(model);
        }
    }
}
