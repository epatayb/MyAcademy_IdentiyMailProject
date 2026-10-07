using IdentiyMail.Web.Entities;
using IdentiyMail.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentiyMail.Web.ViewComponents
{
    public class CategorySidebarViewComponent(UserManager<AppUser> _userManager, ICategoryService _categoryService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userIdValue = _userManager.GetUserId(HttpContext.User);

            if (!int.TryParse(userIdValue, out var userId))
            {
                return Content(string.Empty);
            }

            var categories = await _categoryService.GetCategoriesAsync(userId);

            return View(categories);
        }
    }
}
